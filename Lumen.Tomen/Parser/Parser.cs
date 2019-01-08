using System;
using System.Collections.Generic;
using System.Text;

namespace Lumen.Tomen {
	internal sealed class Parser {
		private readonly List<Token> tokens;
		private readonly Int32 size;
		private Int32 position;
		private Int32 line;
		private readonly String fileName;

		internal Parser(List<Token> Tokens, String fileName) {
			this.fileName = fileName;
			this.tokens = Tokens;
			this.size = Tokens.Count;
			this.line = 0;
		}

		internal TomenTable Parsing(TomenTable table=null) {
			if (table == null) {
				table = new TomenTable(null);
			}

			while (!LookMatch(0, TokenType.EOF)) {
				if (LookMatch(0, TokenType.NAME)) {
					String name = Consume(TokenType.NAME).Text;
					if (Match(TokenType.DOT)) {

						TomenTable t;
						if (table.Contains(name)) {
							t = table[name] as TomenTable;
						}
						else {
							t = new TomenTable(name);
							table[name] = t;
						}

						name = GetId();
						if (Match(TokenType.ASSIGNMENT)) {
							ITomenValue value = Expression();
							t[name] = value;
						}

						while (Match(TokenType.DOT)) {
							if (t.Contains(name)) {
								t = t[name] as TomenTable;
							}
							else {
								t[name] = new TomenTable(name);
								t = t[name] as TomenTable;
							}
							name = GetId();
							if (Match(TokenType.ASSIGNMENT)) {
								ITomenValue value = Expression();
								t[name] = value;
							}
						}
						continue;
					}
					else {
						Match(TokenType.ASSIGNMENT);
						ITomenValue value = Expression();
						table[name] = value;
					}
				}
				else if (LookMatch(0, TokenType.TEXT)) {
					String name = Consume(TokenType.TEXT).Text;
					Match(TokenType.ASSIGNMENT);
					ITomenValue value = Expression();
					table[name] = value;
				}
			}

			return table;
		}

		private ITomenValue Expression() {
			if (LookMatch(0, TokenType.TEXT)) {
				return new TomenString(Consume(TokenType.TEXT).Text);
			}

			return TomenNull.NULL;
		}

		private String GetId() {
			if (LookMatch(0, TokenType.TEXT)) {
				return Consume(TokenType.TEXT).Text;
			}

			return Consume(TokenType.NAME).Text;
		}

		private Boolean Match(TokenType type) {
			Token current = GetToken(0);

			if (type != current.Type) {
				return false;
			}

			this.line = current.Line;
			this.position++;
			return true;
		}

		private Boolean LookMatch(Int32 pos, TokenType type) {
			return GetToken(pos).Type == type;
		}

		private Token GetToken(Int32 NewPosition) {
			Int32 position = this.position + NewPosition;

			if (position >= this.size) {
				return new Token(TokenType.EOF, "");
			}

			return this.tokens[position];
		}

		private Token Consume(TokenType type) {
			Token Current = GetToken(0);
			this.line = Current.Line;

			if (type != Current.Type) {
				throw new Exception();
			}

			this.position++;
			return Current;
		}
	}
}