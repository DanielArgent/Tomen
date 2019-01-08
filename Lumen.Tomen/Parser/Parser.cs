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

		internal TomlTable Parsing(TomlTable table = null) {
			if (table == null) {
				table = new TomlTable(null);
			}

			while (!LookMatch(0, TokenType.EOF)) {
				if (LookMatch(0, TokenType.NAME) || LookMatch(0, TokenType.TEXT)) {
					ITomenKey name = GetId();
					if (Match(TokenType.DOT)) {

						TomlTable t;
						if (table.Contains(name.Value)) {
							t = table[name] as TomlTable;
						}
						else {
							t = new TomlTable(name.Value);
							table[name] = t;
						}

						name = GetId();
						if (Match(TokenType.ASSIGNMENT)) {
							ITomlValue value = Expression();
							t[name] = value;
						}

						while (Match(TokenType.DOT)) {
							if (t.Contains(name.Value)) {
								t = t[name] as TomlTable;
							}
							else {
								t[name] = new TomlTable(name.Value);
								t = t[name] as TomlTable;
							}
							name = GetId();
							if (Match(TokenType.ASSIGNMENT)) {
								ITomlValue value = Expression();
								t[name] = value;
							}
						}
						continue;
					}
					else {
						Match(TokenType.ASSIGNMENT);
						ITomlValue value = Expression();
						table[name] = value;
					}
				}

				if (LookMatch(0, TokenType.LBRACKET) && table.Name != null) {
					return table;
				}

				if (Match(TokenType.LBRACKET)) {
					ITomenKey key = GetId();
					Match(TokenType.RBRACKET);

					table[key.Value] = new TomlTable(key.Value);
					Parsing(table[key.Value] as TomlTable);
				}
			}

			return table;
		}

		private ITomlValue Expression() {
			if (Match(TokenType.PLUS)) {
				ITomlValue value = Expression();
				return value;
			}

			if (Match(TokenType.MINUS)) {
				ITomlValue value = Expression();

				if (value is TomlInt ti) {
					return new TomlInt(-ti.Value);
				}

				if (value is TomlDouble td) {
					return new TomlDouble(-td.Value);
				}

				return value;
			}

			if (LookMatch(0, TokenType.TEXT)) {
				return new TomlString(Consume(TokenType.TEXT).Text);
			}

			if (LookMatch(0, TokenType.INT)) {
				var z = Consume(TokenType.INT).Text;
				return new TomlInt(Int64.Parse(z, System.Globalization.NumberStyles.Any));
			}

			if (LookMatch(0, TokenType.DOUBLE)) {
				var z = Consume(TokenType.DOUBLE).Text;
				return new TomlDouble(Double.Parse(z, System.Globalization.CultureInfo.InvariantCulture));
			}

			if (Match(TokenType.INF)) {
				return new TomlDouble(Double.PositiveInfinity);
			}

			if (Match(TokenType.NAN)) {
				return new TomlDouble(Double.NaN);
			}

			if (Match(TokenType.TRUE)) {
				return new TomlBool(true);
			}

			if (Match(TokenType.FALSE)) {
				return new TomlBool(false);
			}

			if (Match(TokenType.LBRACKET)) {
				List<ITomlValue> items = new List<ITomlValue>();
				while (!Match(TokenType.RBRACKET)) {
					items.Add(Expression());
					Match(TokenType.SPLIT);
				}
				return new TomlArray(items);
			}

			return TomlNull.NULL;
		}

		private ITomenKey GetId() {
			if (LookMatch(0, TokenType.TEXT)) {
				return new QuotedKey(Consume(TokenType.TEXT).Text);
			}

			return new TomlString(Consume(TokenType.NAME).Text);
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