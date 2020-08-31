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
			[","] = new Token(TokenType.SPLIT, "."),
		};
		private readonly String source;
		private readonly Int32 length;
		private readonly List<Token> tokens;
		private Int32 position;
		private Int32 line;
		private readonly String file;

		internal Lexer(String source, String file) {
			this.file = file;
			this.source = source;
			this.length = source.Length;
			this.tokens = new List<Token>();
			this.position = 0;
			this.line = 1;
		}

		internal List<Token> Tokenization() {
			while (this.position < this.length) {
				Char current = this.Peek(0);

				if (current == '"') {
					this.String();
				}
				else if (current == '\'') {
					this.LiteralString();
				}
				else if (Char.IsLetterOrDigit(current) || current == '_') {
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

		private void String() {
			// Reads hexadecimal unicode code with length from current position 
			// and returns appropriate string
			String UnicodeCode(Int32 length) {
				// Valid chars are only [0-9a-fA-F]
				Boolean IsValidChar(Char ch) {
					return "0123456789abcedfABCDEF".IndexOf(ch) != -1;
				}

				Char currentChar = this.Next();
				String hexCode = "";

				for (Int32 i = 0; i < length; i++) {
					if (!IsValidChar(currentChar)) {
						throw new TomlParsingException($"char '{currentChar}' is invalid for hex code", this.file, this.line);
					}

					hexCode += currentChar;
					currentChar = this.Next();
				}

				return Char.ConvertFromUtf32(Convert.ToInt32(hexCode, 16));
			}

			Int32 line = this.line; // remember position

			this.Next(); // "

			Boolean isMultiline = this.CheckIsMultiline('"');
			if (isMultiline) {
				this.Next(2);
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
							}
							continue;
						case '\n':
							if (isMultiline) {
								this.Next();
								current = this.Next();
							}
							continue;

						default:
							throw new TomlParsingException("unknown escape sequence", this.file, line);
					}
				}

				if (!isMultiline) {
					if (current == '\n') {
						throw new TomlParsingException("unclosed string", this.file, line);
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
					throw new TomlParsingException($"unclosed{(isMultiline ? " mutiline " : " ")}string", this.file, line);
				}

				builder.Append(current);

				current = this.Next();
			}

			this.AddToken(TokenType.TEXT, builder.ToString());
		}

		private void LiteralString() {
			Int32 line = this.line; // remember position

			this.Next();

			Boolean isMultiline = this.CheckIsMultiline('\'');
			if (isMultiline) {
				this.Next(2);
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
					throw new TomlParsingException($"unclosed{(isMultiline ? " mutiline " : " ")}literal string", this.file, line);
				}

				builder.Append(current);

				current = this.Next();
			}

			this.Next();

			this.AddToken(TokenType.TEXT, builder.ToString());
		}

		private Boolean CheckIsMultiline(Char border) {
			return this.Peek(0) == border && this.Peek(1) == border;
		}

		private void Tabs() {
			this.line++;

			while (Char.IsWhiteSpace(this.Next())) { }

			this.AddToken(TokenType.NL);
		}

		private void BareKey() {
			StringBuilder buffer = new StringBuilder();

			Char current = this.Peek(0);

			while (true) {
				if (!Char.IsLetterOrDigit(current)
					&& current != '_' && current != '-') {
					break;
				}

				buffer.Append(current);

				current = this.Next();
			}

			String word = buffer.ToString();

			// It's not barekey - it's LocalTime or DateTime or DateTimeOffset
			if (current == ':') {
				this.ParseDateTime(word);
				return;
			}

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
		}

		private void ParseDateTime(String word) {
			String ExpectDigits() {
				String result = "";

				Char current = this.Peek(0);
				while (Char.IsDigit(current)) {
					result += current;
					current = this.Next();
				}

				return result;
			}

			String ExpectNDigits(Int32 length) {
				String result = "";

				Char current = this.Peek(0);
				while (length > 0) {
					if (Char.IsDigit(current)) {
						result += current;
						current = this.Next();
					}
					else {
						throw new Exception("WTF");
					}

					length--;
				}

				return result;
			}

			Char ExpectChar(Char chr) {
				if (this.Peek(0) == chr) {
					this.Next();
				}
				else {
					throw new Exception("invalidChar");
				}

				return chr;
			}

			if (Regex.IsMatch(word, @"^\d{4}-\d{2}-\d{2}T\d{2}$")) {
				word += ExpectChar(':') + ExpectNDigits(2) + ExpectChar(':') + ExpectNDigits(2);

				Char current = this.Peek(0);

				if (current == '.') {
					this.Next();
					word += '.' + ExpectDigits();
				}

				current = this.Peek(0);

				if(current == 'Z') {
					this.Next();
					this.AddToken(TokenType.DATE_TIME_OFFSET, word + "+00:00");
					return;
				}

				if(current == '+' || current == '-') {
					this.Next();

					word += current + ExpectNDigits(2) + ExpectChar(':') + ExpectNDigits(2);
					this.AddToken(TokenType.DATE_TIME_OFFSET, word);
					return;
				}

				this.AddToken(TokenType.DATE_TIME, word);
			}
			else {
				String localTime = word + ExpectChar(':') + ExpectNDigits(2) + ExpectChar(':') + ExpectNDigits(2);

				Char current = this.Peek(0);
				if (current == '.') {
					this.Next();
					localTime += '.' + ExpectDigits();
				}

				AddToken(TokenType.LOCAL_TIME, localTime);
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
					this.AddToken(operatorsDictionary[buffer.ToString()]);
					return;
				}
				buffer.Append(current);
				current = this.Next();
			}
		}

		private void Comment() {
			while ("\n\0".IndexOf(this.Next()) == -1) { }

			this.Next();
		}

		private Char Next(Int32 times = 1) {
			this.position += times;
			return this.Peek(0);
		}

		private Char Peek(Int32 relativePosition) {
			Int32 position = this.position + relativePosition;
			if (position >= this.length) {
				return '\0';
			}

			return this.source[position];
		}

		private void AddToken(Token token) {
			token.Line = this.line;
			this.tokens.Add(token);
		}

		private void AddToken(TokenType type) {
			this.AddToken(type, "");
		}

		private void AddToken(TokenType type, String text) {
			this.tokens.Add(new Token(type, text, this.line));
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
