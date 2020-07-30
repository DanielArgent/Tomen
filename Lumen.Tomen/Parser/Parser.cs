using System;
using System.Collections.Generic;
using System.Text;

namespace Tomen {
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

		internal TomlTable Parse(TomlTable table = null) {
			table = table ?? new TomlTable(null);

			while (!this.LookMatch(0, TokenType.EOF)) {
				if (this.Match(TokenType.LBRACKET)) {
					this.ParseTable(table);
				}
				else {
					this.ParseKeyValuePair(table);
				}
			}

			return table;
		}

		private void ParseTable(TomlTable parentTable) {
			String key = this.GetId();

			TomlTable table;

			if (parentTable.Contains(key)) {
				table = parentTable[key] as TomlTable; // unsafe
			}
			else {
				table = new TomlTable(key);
				parentTable[key] = table;
			}

			if (this.Match(TokenType.DOT)) {
				this.ParseTable(table);
			}
			else {
				this.Consume(TokenType.RBRACKET);
				this.Match(TokenType.NL);

				while (!this.LookMatch(0, TokenType.LBRACKET) && !this.LookMatch(0, TokenType.EOF)) {
					this.ParseKeyValuePair(table);
				}
			}
		}

		private void ParseKeyValuePair(TomlTable parentTable) {
			if (this.Match(TokenType.ASSIGNMENT)) {
				throw new TomlParsingException("empty bare key", this.fileName, this.line);
			}

			String key = this.GetId();

			if (this.Match(TokenType.DOT)) {
				TomlTable table;

				if (parentTable.Contains(key)) {
					table = parentTable[key] as TomlTable; // unsafe
				}
				else {
					table = new TomlTable(key);
					parentTable[key] = table;
				}

				this.ParseKeyValuePair(table);
			}
			else {
				this.Consume(TokenType.ASSIGNMENT);

				if (this.Match(TokenType.NL)) {
					throw new TomlParsingException("unspecified value", this.fileName, this.line);
				}

				parentTable[key] = this.Expression();
			}

			this.Match(TokenType.NL);
		}

		private ITomlValue Expression() {
			if (this.Match(TokenType.PLUS)) {
				ITomlValue value = this.Expression();
				return value;
			}

			if (this.Match(TokenType.MINUS)) {
				ITomlValue value = this.Expression();

				if (value is TomlInt ti) {
					return new TomlInt(-ti.Value);
				}

				if (value is TomlDouble td) {
					return new TomlDouble(-td.Value);
				}

				return value;
			}

			if (this.LookMatch(0, TokenType.TEXT)) {
				return new TomlString(this.Consume(TokenType.TEXT).Text);
			}

			if (this.LookMatch(0, TokenType.INT)) {
				var z = this.Consume(TokenType.INT).Text;
				return new TomlInt(Int64.Parse(z, System.Globalization.NumberStyles.Any));
			}

			if (this.LookMatch(0, TokenType.DOUBLE)) {
				var z = this.Consume(TokenType.DOUBLE).Text;
				return new TomlDouble(Double.Parse(z, System.Globalization.CultureInfo.InvariantCulture));
			}

			if (this.Match(TokenType.INF)) {
				return new TomlDouble(Double.PositiveInfinity);
			}

			if (this.Match(TokenType.NAN)) {
				return new TomlDouble(Double.NaN);
			}

			if (this.Match(TokenType.TRUE)) {
				return new TomlBool(true);
			}

			if (this.Match(TokenType.FALSE)) {
				return new TomlBool(false);
			}

			if (this.Match(TokenType.LBRACKET)) {
				List<ITomlValue> items = new List<ITomlValue>();
				while (!this.Match(TokenType.RBRACKET)) {
					items.Add(this.Expression());
					this.Match(TokenType.SPLIT);
				}
				return new TomlArray(items);
			}

			return TomlNull.NULL;
		}

		private String GetId() {
			if (this.LookMatch(0, TokenType.NAME)) {
				return this.Consume(TokenType.NAME).Text;
			}

			if (this.LookMatch(0, TokenType.TEXT)) {
				return this.Consume(TokenType.TEXT).Text;
			}

			if (this.LookMatch(0, TokenType.INT)) {
				return this.Consume(TokenType.INT).Text;
			}

			throw new TomlParsingException("unexpected key", this.fileName, this.line);
		}

		private Boolean Match(TokenType type) {
			Token current = this.GetToken(0);

			if (type != current.Type) {
				return false;
			}

			this.line = current.Line;
			this.position++;
			return true;
		}

		private Boolean LookMatch(Int32 pos, TokenType type) {
			return this.GetToken(pos).Type == type;
		}

		private Token GetToken(Int32 NewPosition) {
			Int32 position = this.position + NewPosition;

			if (position >= this.size) {
				return new Token(TokenType.EOF, "");
			}

			return this.tokens[position];
		}

		private Token Consume(TokenType type) {
			Token Current = this.GetToken(0);
			this.line = Current.Line;

			if (type != Current.Type) {
				throw new Exception();
			}

			this.position++;
			return Current;
		}
	}
}