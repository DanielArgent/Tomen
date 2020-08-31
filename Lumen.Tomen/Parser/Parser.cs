using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Tomen {
	internal sealed class Parser {
		private readonly List<Token> tokens;
		private readonly Int32 size;
		private Int32 position;
		private Int32 currentLine;
		private readonly String currentFile;

		internal Parser(List<Token> Tokens, String fileName) {
			this.currentFile = fileName;
			this.tokens = Tokens;
			this.size = Tokens.Count;
			this.position = 0;
			this.currentLine = 0;
		}

		internal TomlTable Parse() {
			TomlTable rootTable = new TomlTable(null);

			while (!this.LookMatch(0, TokenType.EOF)) {
				this.Match(TokenType.NL);

				if (this.Match(TokenType.LBRACKET)) {
					this.ParseTable(rootTable);
				}
				else {
					this.ParseKeyValuePair(rootTable);
				}
			}

			return rootTable;
		}

		private void ParseTable(TomlTable parentTable) {
			String key = this.ParseKey();

			TomlTable table = this.GetTableOrCreateIfAbsent(key, parentTable);

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
			// If there is no key
			if (this.Match(TokenType.ASSIGNMENT)) {
				throw new TomlSyntaxException("expected key, got '='", this.currentFile, this.currentLine);
			}

			String key = this.ParseKey();

			if (this.Match(TokenType.DOT)) {
				TomlTable table = this.GetTableOrCreateIfAbsent(key, parentTable);

				this.ParseKeyValuePair(table);
			}
			else {
				this.Consume(TokenType.ASSIGNMENT);

				// There is no value
				if (this.Match(TokenType.NL)) {
					throw new TomlParsingException("unspecified value", this.currentFile, this.currentLine);
				}

				parentTable[key] = this.ParseValue();
			}

			this.Match(TokenType.NL);
		}

		private ITomlValue ParseValue() {
			if (this.Match(TokenType.PLUS)) {
				ITomlValue value = this.ParseValue();
				// TODO: what if value is not a number?
				return value;
			}

			if (this.Match(TokenType.MINUS)) {
				ITomlValue value = this.ParseValue();

				if (value is TomlInt ti) {
					return new TomlInt(-ti.Value);
				}

				if (value is TomlDouble td) {
					return new TomlDouble(-td.Value);
				}

				// TODO: what if value is not a number?

				return value;
			}

			if (this.LookMatch(0, TokenType.TEXT)) {
				return new TomlString(this.Consume(TokenType.TEXT).Text);
			}

			if (this.LookMatch(0, TokenType.LOCAL_TIME)) {
				return TomlLocalTime.Parse(this.Consume(TokenType.LOCAL_TIME).Text);
			}

			if (this.LookMatch(0, TokenType.DATE_TIME)) {
				var str = this.Consume(TokenType.DATE_TIME).Text;

				return new TomlDateTime(DateTime.ParseExact(str, "yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture));
			}

			if (this.LookMatch(0, TokenType.DATE_TIME_OFFSET)) {
				String str = this.Consume(TokenType.DATE_TIME_OFFSET).Text;

				DateTimeOffset offset =
					DateTimeOffset.ParseExact(str, "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz", CultureInfo.InvariantCulture);
				return new TomlDateTimeOffset(offset);
			}

			if (this.LookMatch(0, TokenType.BAREKEY)) {
				String number = this.Consume(TokenType.BAREKEY).Text.Replace("_", "");

				// hexadecimal number
				if (number.StartsWith("0x")) {
					return new TomlInt(Convert.ToInt64(number.Substring(2), 16));
				}

				// octal number
				if (number.StartsWith("0o")) {
					return new TomlInt(Convert.ToInt64(number.Substring(2), 8));
				}

				// binary number
				if (number.StartsWith("0b")) {
					return new TomlInt(Convert.ToInt64(number.Substring(2), 2));
				}

				// float
				if (this.Match(TokenType.DOT)) {
					number += '.';

					if (this.LookMatch(0, TokenType.BAREKEY)) {
						number += this.Consume(TokenType.BAREKEY).Text.Replace("_", "");
					}
					else {
						number += '0';
					}

					return new TomlDouble(Double.Parse(number, System.Globalization.CultureInfo.InvariantCulture));
				}

				// It's local date
				if (Regex.IsMatch(number, @"^\d{4}-\d{2}-\d{2}$")) {
					return new TomlDateTime(DateTime.Parse(number));
				}

				return new TomlInt(Int64.Parse(number, NumberStyles.Any));
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
					items.Add(this.ParseValue());
					this.Match(TokenType.SPLIT);
					this.Match(TokenType.NL);
					this.Match(TokenType.SPLIT); // ?
				}
				return new TomlArray(items);
			}

			return TomlNull.NULL;
		}

		private String ParseKey() {
			if (this.LookMatch(0, TokenType.BAREKEY)) {
				return this.Consume(TokenType.BAREKEY).Text;
			}

			if (this.LookMatch(0, TokenType.TEXT)) {
				return this.Consume(TokenType.TEXT).Text;
			}

			throw new TomlParsingException("unexpected key", this.currentFile, this.currentLine);
		}

		private TomlTable GetTableOrCreateIfAbsent(String name, TomlTable parentTable) {
			TomlTable table;

			if (parentTable.Contains(name)) {
				if (parentTable[name] is TomlTable tomlTable) {
					table = tomlTable;
				} else {
					throw new TomlSemanticException($"value with key '{parentTable.Name}.{name}' is already exists and it is not ITomlOpenTable", this.currentFile, this.currentLine);
				}
			}
			else {
				table = new TomlTable(name);
				parentTable[name] = table;
			}

			return table;
		}

		private Boolean Match(TokenType type) {
			Token current = this.GetToken(0);

			if (type != current.Type) {
				return false;
			}

			this.currentLine = current.Line;
			this.position++;
			return true;
		}

		private Boolean LookMatch(Int32 pos, TokenType type) {
			return this.GetToken(pos).Type == type;
		}

		private Token GetToken(Int32 offset) {
			Int32 position = this.position + offset;

			if (position >= this.size) {
				return new Token(TokenType.EOF, "");
			}

			return this.tokens[position];
		}

		private Token Consume(TokenType type) {
			Token currentToken = this.GetToken(0);
			this.currentLine = currentToken.Line;

			if (type != currentToken.Type) {
				throw new TomlSyntaxException($"excepted {type}, got {currentToken.Type}", 
					this.currentFile, this.currentLine);
			}

			this.position++;
			return currentToken;
		}
	}
}