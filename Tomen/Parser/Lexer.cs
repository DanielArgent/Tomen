using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tomen {
	internal sealed class Lexer {
		private const String operatorsString = "+-\\*/%(){}=<>!&|;:?,[]^.@~#";
		private static IDictionary<String, Token> operatorsDictionary = new Dictionary<String, Token>() {
			["="] = new Token(TokenType.ASSIGNMENT, "="),
			["["] = new Token(TokenType.LBRACKET, "["),
			["]"] = new Token(TokenType.RBRACKET, "]"),
			["{"] = new Token(TokenType.LBRACE, "{"),
			["}"] = new Token(TokenType.RBRACE, "}"),
			["+"] = new Token(TokenType.PLUS, "+"),
			["-"] = new Token(TokenType.MINUS, "-"),
			["."] = new Token(TokenType.DOT, "."),
			[":"] = new Token(TokenType.COLON, ":"),
			[","] = new Token(TokenType.SPLIT, "."),
			["*"] = new Token(TokenType.STAR, "*"),
		};
		private readonly String source;
		private readonly Int32 length;
		private readonly List<Token> tokens;
		private readonly String currentFile;
		private Int32 position;
		private Int32 currentLine;
		private Boolean isValueSide;
		private Boolean isArrayContext;

		internal Lexer(String source, String file) {
			this.currentFile = file;
			this.source = source;
			this.length = source.Length;
			this.tokens = new List<Token>();
			this.position = 0;
			this.currentLine = 1;
			this.isValueSide = false;
		}

		internal List<Token> GetTokens() {
			while (this.position < this.length) {
				Char current = this.Peek(0);

				if (current == '"') {
					this.String();
				}
				else if (current == '\'') {
					this.LiteralString();
				}
				else if (isValueSide && Char.IsDigit(current)) {
					this.Number();
				}
				else if (Char.IsLetterOrDigit(current) || current == '_' || (!isValueSide && current == '-')) {
					this.BareKey();
				}
				else if (operatorsString.IndexOf(current) != -1) {
					this.Operator();
				}
				else if (current == '\n') {
					this.Tabs();
				}
				else {
					this.Next();
				}
			}

			return this.tokens;
		}

		private void Number() {
			StringBuilder result = new StringBuilder();

			Char current = this.Peek();
			Char previous = '\0';

			while (Char.IsDigit(current) || "_boxeE.".IndexOf(current) != -1) {
				if ("box".IndexOf(current) != -1) {
					if (previous == '0') {
						switch (current) {
							case 'b':
								this.ParseWithBase(2, current);
								return;
							case 'o':
								this.ParseWithBase(8, current);
								return;
							case 'x':
								this.ParseWithBase(16, current);
								return;
						}
					}
					else {
						this.position--;
						break;
					}
				}

				if (Char.ToLower(current) == 'e') {
					result.Append(Char.ToLower(current));
					previous = current;
					current = this.Next();

					if(current == '-' || current == '+') {
						result.Append(current);
						previous = current;
						current = this.Next();
					}
				}
				else {
					result.Append(current);
					previous = current;
					current = this.Next();
				}
			}

			this.AddToken(TokenType.DIGITS, result.ToString());
		}

		const String VALID_CHARS = "0123456789abcedf";

		private void ParseWithBase(Int32 requiredBase, Char prefixChar) {
			String validCharsForBase = VALID_CHARS.Substring(0, requiredBase);

			StringBuilder result = new StringBuilder("0" + prefixChar);

			Char current = this.Next();
			while (validCharsForBase.IndexOf(Char.ToLower(current)) != -1 || current == '_') {
				result.Append(current);
				current = this.Next();
			}

			this.AddToken(TokenType.NUMBER_WITH_BASE, result.ToString());
		}

		private void String() {
			// Reads hexadecimal unicode code with length from current position 
			// and returns appropriate string
			String UnicodeCode(Int32 length) {
				// Valid chars are only [0-9a-fA-F]
				Boolean IsValidHexChar(Char ch) {
					return "0123456789abcedfABCDEF".IndexOf(ch) != -1;
				}

				Char currentChar = this.Next();
				String hexCode = "";

				for (Int32 i = 0; i < length; i++) {
					if (!IsValidHexChar(currentChar)) {
						throw new TomlSyntaxException($"character '{currentChar}' is invalid for hexadecimal code", this.currentFile, this.currentLine);
					}

					hexCode += currentChar;
					currentChar = this.Next();
				}

				try {
					return Char.ConvertFromUtf32(Convert.ToInt32(hexCode, 16));
				}
				catch {
					throw new TomlSyntaxException($"value '{hexCode}' is invalid hexadecimal code of character", this.currentFile, this.currentLine);
				}
			}

			Int32 startLiteralLine = this.currentLine; // Remember position

			this.Next(); // "

			Boolean isMultiline = this.CheckIsMultiline('"');
			if (isMultiline) {
				this.Next(2);

				// "A newline immediately following the opening delimiter will be trimmed"
				Char currentChar = this.Peek(0);
				if (currentChar == '\r' && this.Peek(1) == '\n') {
					this.Next(2);
				}
				else if (currentChar == '\n') {
					this.Next();
				}
			}

			StringBuilder builder = new StringBuilder();
			Char current = this.Peek(0);

			while (true) {
				// Escape sequences
				if (current == '\\') {
					current = this.Next();
					switch (current) {
						case '"':
							current = this.Next();
							builder.Append('"');
							continue;
						case 'f':
							current = this.Next();
							builder.Append('\f');
							continue;
						case '\\':
							current = this.Next();
							builder.Append('\\');
							continue;
						case 'b':
							current = this.Next();
							builder.Append('\b');
							continue;
						case 'r':
							current = this.Next();
							builder.Append('\r');
							continue;
						case 'n':
							current = this.Next();
							builder.Append('\n');
							continue;
						case 'u':
							builder.Append(UnicodeCode(4));
							current = this.Peek(0);
							continue;
						case 'U':
							builder.Append(UnicodeCode(8));
							current = this.Peek(0);
							continue;
						case 't':
							current = this.Next();
							builder.Append('\t');
							continue;

						// It works only if it is multiline string
						case '\r':
							if (isMultiline) {
								if (Environment.NewLine.Length == 2) {
									// If ends on \r\n - ignore \r
									this.Next();
								}

								// Ignore \n
								current = this.Next();

								while (Char.IsWhiteSpace(current)) {
									current = this.Next();
								}
							}
							continue;
						case '\n':
							if (isMultiline) {
								this.Next();
								current = this.Next();

								while (Char.IsWhiteSpace(current)) {
									current = this.Next();
								}
							}
							continue;

						default:
							throw new TomlSyntaxException($"unknown escape sequence '\\{current}'", this.currentFile, startLiteralLine);
					}
				}

				if (!isMultiline) {
					if (current == '\n') {
						throw new TomlSyntaxException($"unclosed string", this.currentFile, startLiteralLine);
					}

					if (current == '"') {
						this.Next();
						break;
					}
				}
				else {
					// Mutiline stop sequence is """
					if (current == '"' && this.Peek(1) == '"' && this.Peek(2) == '"') {
						this.Next(3);
						break;
					}
				}

				// End of input, but literal is not closed
				if (this.position >= this.length) {
					throw new TomlSyntaxException($"unclosed{(isMultiline ? " mutiline " : " ")}string", this.currentFile, startLiteralLine);
				}

				builder.Append(current);

				current = this.Next();
			}

			if (this.isValueSide) {
				this.AddToken(TokenType.TEXT, builder.ToString());
			} else {
				this.AddToken(TokenType.BAREKEY, builder.ToString());
			}
		}

		private void LiteralString() {
			Int32 line = this.currentLine; // remember position

			this.Next();

			Boolean isMultiline = this.CheckIsMultiline('\'');
			if (isMultiline) {
				this.Next(2);

				// "A newline immediately following the opening delimiter will be trimmed"
				Char currentChar = this.Peek(0);
				if (currentChar == '\r' && this.Peek(1) == '\n') {
					this.Next(2);
				}
				else if (currentChar == '\n') {
					this.Next();
				}
			}

			StringBuilder builder = new StringBuilder();
			Char current = this.Peek(0);

			while (true) {
				if (!isMultiline && current == '\'') {
					break;
				}

				if (isMultiline && current == '\'' && this.Peek(1) == '\'' && this.Peek(2) == '\'') {
					this.Next(2);
					break;
				}

				if (this.position >= this.length) {
					throw new TomlSyntaxException($"unclosed{(isMultiline ? " mutiline " : " ")}literal string", this.currentFile, line);
				}

				builder.Append(current);

				current = this.Next();
			}

			this.Next();

			if (this.isValueSide) {
				this.AddToken(TokenType.TEXT, builder.ToString());
			}
			else {
				this.AddToken(TokenType.BAREKEY, builder.ToString());
			}
		}

		private Boolean CheckIsMultiline(Char border) {
			return this.Peek(0) == border && this.Peek(1) == border;
		}

		private void Tabs() {
			this.currentLine++;

			if(!isArrayContext)
				this.isValueSide = false;

			while (Char.IsWhiteSpace(this.Next())) { }

			this.AddToken(TokenType.NL);
		}

		private void BareKey() {
			StringBuilder buffer = new StringBuilder();

			Char current = this.Peek(0);

			while (true) {
				if ((isValueSide && !Char.IsLetter(current)) || (!isValueSide && !Char.IsLetterOrDigit(current)
					&& current != '_' && current != '-')) {
					break;
				}

				buffer.Append(current);

				current = this.Next();
			}

			String word = buffer.ToString();

			if (isValueSide) {
				switch (word) {
					case "inf":
						this.AddToken(TokenType.INF, word);
						break;
					case "nan":
						this.AddToken(TokenType.NAN, word);
						break;
					case "true":
						this.AddToken(TokenType.TRUE, word);
						break;
					case "false":
						this.AddToken(TokenType.FALSE, word);
						break;
					default:
						this.AddToken(TokenType.BAREKEY, word);
						break;
				}
			} else {
				this.AddToken(TokenType.BAREKEY, word);
			}
		}

		private void Operator() {
			Char current = this.Peek(0);

			// Comments
			if (current == '#') {
				this.Comment();
				return;
			}

			StringBuilder buffer = new StringBuilder(current.ToString());
			current = this.Next();

			while (true) {
				if (!operatorsDictionary.ContainsKey(buffer.ToString() + current)) {
					var token = operatorsDictionary[buffer.ToString()];

					if(token.Type == TokenType.ASSIGNMENT) {
						this.isValueSide = true;
					}

					if (isValueSide && token.Type == TokenType.LBRACKET) {
						this.isArrayContext = true;
					}

					if (isValueSide && token.Type == TokenType.RBRACKET) {
						this.isArrayContext = false;
					}

					this.AddToken(token);
					return;
				}

				buffer.Append(current);
				current = this.Next();
			}
		}

		private void Comment() {
			Char current = this.Next();
			while ("\n\0".IndexOf(current) == -1) {
				// According to v1.0.0-rc.2 character \r (U+000D) also not permitted
				// But in fact, it appears on some platfroms on endlines, so we allow it.
				if (current < 8 || (10 < current && current < 13) || (13 < current && current < 31) || current == 127) {
					throw new TomlSyntaxException($"control characters U+0000 to U+0008, U+000A to U+000C, U+000E to U+001F, U+007F are not permitted in comments, got char (U+{(Int32)current:X4})", this.currentFile, this.currentLine);
				}

				current = this.Next();
			}

			this.Tabs();
		}

		private Char Next(Int32 times = 1) {
			this.position += times;
			return this.Peek();
		}

		private Char Peek(Int32 relativePosition = 0) {
			Int32 position = this.position + relativePosition;
			if (position >= this.length) {
				return '\0';
			}

			return this.source[position];
		}

		private void AddToken(Token token) {
			token.Line = this.currentLine;
			this.tokens.Add(token);
		}

		private void AddToken(TokenType type) {
			this.AddToken(type, "");
		}

		private void AddToken(TokenType type, String text) {
			this.tokens.Add(new Token(type, text, this.currentLine));
		}

		internal static Boolean IsValidId(String id) {
			if (Regex.IsMatch(id, "^[A-Za-z0-9_][A-Za-z0-9_-]*$")) {
				return true;
			}

			return false;
		}

		public static String NormalizeKey(String s) {
			if (IsValidId(s)) {
				return s;
			}

			return $@"""{s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\f", "\\f").Replace("\t", "\\t").Replace("\n", "\\n").Replace("\b", "\\b").Replace(Environment.NewLine, "\\r\\n")}""";
		}

		public static String Normalize(String s) {
			return $@"""{s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\f", "\\f").Replace("\t", "\\t").Replace("\n", "\\n").Replace("\b", "\\b").Replace(Environment.NewLine, "\\r\\n")}""";
		}
	}
}
