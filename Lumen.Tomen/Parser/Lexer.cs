using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lumen.Tomen {
	internal sealed class Lexer {
		private const String operatorsString = "+-\\*/%(){}=<>!&|;:?,[]^.@~#";
		private static IDictionary<String, Token> operatorsDictionary = new Dictionary<String, Token>() {
			["="] = new Token(TokenType.ASSIGNMENT, "="),
			["["] = new Token(TokenType.LBRACKET, "["),
			["]"] = new Token(TokenType.RBRACKET, "]"),
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

				if (Char.IsDigit(current)) {
					this.Number();
				}
				else if (current == '"') {
					this.String();
				}
				else if (current == '\'') {
					this.LiteralString();
				}
				else if (Char.IsLetter(current) || current == '_' || current == '$') {
					this.Word();
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
			Int32 line = this.line; // remember position

			this.Next();

			Boolean isMultiline = false;
			if(this.Peek(0) == '"' && this.Peek(1) == '"') {
				this.Next();
				this.Next();
				isMultiline = true;
			}

			StringBuilder builder = new StringBuilder();
			Char current = this.Peek(0);

			while (true) {
				if (current == '\\') {
					current = this.Next();
					switch (current) {
						case '"':
							current = this.Next();
							builder.Append('"');
							continue;

						case '\r':
							this.Next();
							current = this.Next();
							continue;
						case '\n':
							current = this.Next();
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
						case 't':
							current = this.Next();
							builder.Append('\t');
							continue;

						default:
							builder.Append('\\');
							continue;
					}
				}

				if (!isMultiline && current == '"') {
					break;
				}

				if(isMultiline && current == '"' && this.Peek(1) == '"' && this.Peek(2) == '"') {
					Next();
					Next();
					break;
				}

				if (this.position >= this.length) {
					throw new TomlParsingException($"unclosed{(isMultiline ? " mutiline " : " ")}string literal", file, line);
				}

				builder.Append(current);

				current = this.Next();
			}

			this.Next();

			this.AddToken(TokenType.TEXT, builder.ToString());
		}

		private void LiteralString() {
			Int32 line = this.line; // remember position

			this.Next();

			Boolean isMultiline = false;
			if (this.Peek(0) == '\'' && this.Peek(1) == '\'') {
				this.Next();
				this.Next();
				isMultiline = true;
			}

			StringBuilder builder = new StringBuilder();
			Char current = this.Peek(0);

			while (true) {
				if (!isMultiline && current == '\'') {
					break;
				}

				if (isMultiline && current == '\'' && this.Peek(1) == '\'' && this.Peek(2) == '\'') {
					Next();
					Next();
					break;
				}

				if (this.position >= this.length) {
					throw new TomlParsingException($"unclosed{(isMultiline ? " mutiline " : " ")}string literal", file, line);
				}

				builder.Append(current);

				current = this.Next();
			}

			this.Next();

			this.AddToken(TokenType.TEXT, builder.ToString());
		}

		private void Number() {
			StringBuilder buffer = new StringBuilder();
			Char current = this.Peek(0);

			Boolean isScientic = false;

			while (true) {
				if (current == '.') {
					// Не, ну логично же.
					if (buffer.ToString().IndexOf('.') != -1) {
						throw new Exception("лишняя точка < литерал num");
					}
				}
				else if (current == '_') {
					current = this.Next();
					continue;
				}
				else if (!Char.IsDigit(current)) {
					if (current == 'e') {
						isScientic = true;
						buffer.Append(current);
						current = this.Next();
						if (current == '-') {
							buffer.Append(current);
							current = this.Next();
						}
						else if (current == '+') {
							buffer.Append(current);
							current = this.Next();
						}
						continue;
					}
					break;
				}
				buffer.Append(current);
				current = this.Next();
			}


			if (isScientic) {
				this.AddToken(TokenType.DOUBLE, Double.Parse(buffer.ToString(), System.Globalization.NumberStyles.Any).ToString());
			}
			else {
				String val = buffer.ToString();
				if(val.Contains(".")) {
					this.AddToken(TokenType.DOUBLE, val);
				} else {
					this.AddToken(TokenType.INT, val);
				}
			}
		}

		private void Tabs() {
			this.line++;
			if (this.tokens.Count > 0) {
				Char current = this.Next();

				while (Char.IsWhiteSpace(current)) {
					if (current == '\n') {
						this.line++;
					}

					current = this.Next();
				}
			}
		}

		private void Word() {
			StringBuilder buffer = new StringBuilder();

			Char current = this.Peek(0);

			while (true) {
				if (!Char.IsLetterOrDigit(current)
					&& current != '_') {
					break;
				}

				buffer.Append(current);

				current = this.Next();
			}

			String word = buffer.ToString();

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
					this.AddToken(TokenType.NAME, word);
					break;
			}
		}

		private void Operator() {
			Char current = this.Peek(0);

			// Comments
			if (current == '#') {
				this.Next();
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
			Char current = this.Peek(0);

			StringBuilder sb = new StringBuilder();

			while (true) {
				if ("\r\n\0".IndexOf(current) != -1) {
					break;
				}
				current = this.Next();
				sb.Append(current);
			}

			this.Next();
		}

		private Char Next() {
			this.position++;
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
			if (Regex.IsMatch(id, "^[A-Za-z][A-Za-z0-9._]*$")) {
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
