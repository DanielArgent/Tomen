using System;
using System.Collections.Generic;
using System.Text;

namespace Lumen.Tomen {
	internal sealed class Lexer {
		private const String operatorsString = "+-\\*/%(){}=<>!&|;:?,[]^.@~#";
		private const String validSymbols = "0123456789abcdef";
		private static IDictionary<String, Token> operatorsDictionary = new Dictionary<String, Token>() {
			["="] = new Token(TokenType.ASSIGNMENT, "="),
			["["] = new Token(TokenType.LBRACKET, "["),
			["]"] = new Token(TokenType.RBRACKET, "]"),
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

		public List<Token> Tokenization() {
			while (this.position < this.length) {
				Char current = Peek(0);

				if (current == '"') {
					String();
				}
				else if (Char.IsLetter(current) || current == '_' || current == '$') {
					Word();
				}
				else if (operatorsString.IndexOf(current) != -1) {
					Operator();
				}
				else if (current == '\n') {
					Tabs();
				}
				else {
					Next();
				}
			}

			return this.tokens;
		}

		private void String() {
			Next();

			StringBuilder builder = new StringBuilder();

			Int32 x = 0;

			Char current = Peek(0);

			List<String> substitutes = new List<String>();

			while (true) {
				if (current == '\\') {
					current = Next();
					switch (current) {
						case '#':
							current = Next();
							builder.Append('#');
							continue;
						case '"':
							current = Next();
							builder.Append('"');
							continue;
						case '\r':
							Next();
							current = Next();
							continue;
						case 'f':
							current = Next();
							builder.Append('\f');
							continue;
						case '\\':
							current = Next();
							builder.Append('\\');
							continue;
						case '0':
							current = Next();
							builder.Append('\0');
							continue;
						case 'a':
							current = Next();
							builder.Append('\a');
							continue;
						case 'b':
							current = Next();
							builder.Append('\b');
							continue;
						case 'r':
							current = Next();
							builder.Append('\r');
							continue;
						case 'v':
							current = Next();
							builder.Append('\v');
							continue;
						case 'e':
							current = Next();
							builder.Append(Environment.NewLine);
							continue;
						case 'n':
							current = Next();
							builder.Append('\n');
							continue;
						case 't':
							current = Next();
							builder.Append('\t');
							continue;
						default:
							builder.Append('\\');
							continue;
					}
				}

				if (current == '{') {
					current = Next();
					builder.Append("{{");
				}

				if (current == '}') {
					current = Next();
					builder.Append("}}");
				}

				if (current == '#') {
					current = Next();

					if (current == '"') {
						builder.Append('#');
						break;
					}

					StringBuilder buffer = new StringBuilder();
					String s = buffer.ToString();
					Int32 position = substitutes.IndexOf(s);

					if (position == -1) {
						substitutes.Add(s);
						//if(builder.ToString().EndsWith("}"))
						//	builder.Append("\b ");
						builder.Append($"{{{x}}}");
						x++;
					}
					else {
						builder.Append($"{{{position}}}");
					}
					continue;
				}

				if (current == '"') {
					break;
				}

				CheckOutOfRange();

				builder.Append(current);

				current = Next();
			}

			current = Next();

			AddToken(TokenType.TEXT, builder.ToString());
		}

		private void CheckOutOfRange() {
			if (this.position >= this.length) {
				throw new Exception("consumed symbol '\"'");
			}
		}

		private void Tabs() {
			this.line++;
			if (this.tokens.Count > 0) {
				Char current = Next();

				while (Char.IsWhiteSpace(current)) {
					if (current == '\n') {
						this.line++;
					}

					current = Next();
				}
			}
		}

		private void Word() {
			StringBuilder buffer = new StringBuilder();

			Char current = Peek(0);

			while (true) {
				if (!Char.IsLetterOrDigit(current)
					&& current != '_') {
					break;
				}

				buffer.Append(current);

				current = Next();
			}

			String word = buffer.ToString();

			switch (word) {
				default:
					AddToken(TokenType.NAME, word);
					break;
			}
		}

		private void Operator() {
			// Берём текущий символ.
			Char current = Peek(0);

			// Комменты.
			if (current == '#') {
				Next();
				Comment();
				return;
			}

			// А тут у нас собираются операторы.
			StringBuilder buffer = new StringBuilder(current + "");
			current = Next();

			while (true) {
				if (!operatorsDictionary.ContainsKey(buffer.ToString() + current)) {
					AddToken(operatorsDictionary[buffer.ToString()]);
					return;
				}
				buffer.Append(current);
				current = Next();
			}
		}

		private void Comment() {
			Char current = Peek(0);

			StringBuilder sb = new StringBuilder();

			while (true) {
				if ("\r\n\0".IndexOf(current) != -1) {
					break;
				}
				current = Next();
				sb.Append(current);
			}

			Next();
		}

		private Char Next() {
			this.position++;
			return Peek(0);
		}

		private Char Peek(Int32 relativePosition) {
			// Да  нет-нет.
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
			AddToken(type, "");
		}

		private void AddToken(TokenType type, String text) {
			this.tokens.Add(new Token(type, text, this.line));
		}
	}
}
