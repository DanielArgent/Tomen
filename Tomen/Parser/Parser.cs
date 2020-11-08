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
				throw new TomlParsingException("expected key, got '='", this.currentFile, this.currentLine);
			}

			String key = this.ParseKey();

			if (this.Match(TokenType.DOT)) {
				TomlTable table = this.GetTableOrCreateIfAbsent(key, parentTable);

				this.ParseKeyValuePair(table);
			}
			else {
				this.Consume(TokenType.ASSIGNMENT);

				// There is no value
				if (this.Match(TokenType.NL) || this.Match(TokenType.EOF)) {
					throw new TomlParsingException("unspecified value", this.currentFile, this.currentLine);
				}

				if(parentTable.Contains(key)) {
					throw new TomlSemanticException($"key {key} is already defined", this.currentFile, this.currentLine);
				}

				parentTable[key] = this.ParseValue();

				if (!this.Match(TokenType.NL) && !this.LookMatch(0, TokenType.EOF)) {
					throw new TomlParsingException("there must be a newline or end of file after a key/value pair", this.currentFile, this.currentLine);
				}
			}
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

			if (this.LookMatch(0, TokenType.DIGITS) && this.LookMatch(1, TokenType.COLON)) {
				return this.ParseLocalTime();
			}

			if (this.LookMatch(0, TokenType.DIGITS) && this.LookMatch(1, TokenType.MINUS)) {
				return this.ParseDate();
			}

			if (this.LookMatch(0, TokenType.DIGITS)) {
				String digits = this.Consume(TokenType.DIGITS).Text;

				// float
				if (this.Match(TokenType.DOT)) {
					digits += '.';

					if (this.LookMatch(0, TokenType.DIGITS)) {
						digits += this.Consume(TokenType.DIGITS).Text.Replace("_", "");
					}
					else {
						digits += '0';
					}

					return new TomlDouble(Double.Parse(digits, CultureInfo.InvariantCulture));
				}

				return new TomlInt(Int64.Parse(digits, NumberStyles.Any));
			}

			if (this.LookMatch(0, TokenType.NUMBER_WITH_BASE)) {
				String number = this.Consume(TokenType.NUMBER_WITH_BASE).Text.Replace("_", "");

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

				// else?
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

		Tuple<Int32, Int32> ParseHoursAndMinutes() {
			Int32 hours = Convert.ToInt32(this.Consume(TokenType.DIGITS).Text);
			this.Consume(TokenType.COLON);
			Int32 minutes = Convert.ToInt32(this.Consume(TokenType.DIGITS).Text);

			return new Tuple<Int32, Int32>(hours, minutes);
		}

		TomlLocalTime ParseLocalTime() {
			Tuple<Int32, Int32> hoursAndMinutes = this.ParseHoursAndMinutes();
			this.Consume(TokenType.COLON);
			Int32 seconds = Convert.ToInt32(this.Consume(TokenType.DIGITS).Text);

			Int32 milliseconds = 0;

			if (this.Match(TokenType.DOT)) {
				milliseconds = Convert.ToInt32(this.Consume(TokenType.DIGITS).Text.Substring(0, 3));
			}

			return new TomlLocalTime(hoursAndMinutes.Item1, hoursAndMinutes.Item2, seconds, milliseconds);
		}

		ITomlValue ParseDate() {
			Int32 year = Convert.ToInt32(this.Consume(TokenType.DIGITS).Text);
			this.Consume(TokenType.MINUS);
			Int32 month = Convert.ToInt32(this.Consume(TokenType.DIGITS).Text);
			this.Consume(TokenType.MINUS);
			Int32 day = Convert.ToInt32(this.Consume(TokenType.DIGITS).Text);


			if (this.LookMatch(0, TokenType.BAREKEY) && this.GetToken(0).Text.ToUpper() == "T") {
				return this.ParseDateTime(year, month, day);
			}
			else {
				return new TomlLocalDate(year, month, day);
			}


		}

		ITomlValue ParseDateTime(Int32 year, Int32 month, Int32 day) {
			this.Consume(this.GetToken(0).Type);

			TomlLocalTime time = this.ParseLocalTime();

			DateTime dateTime = new DateTime(year, month, day, time.Hours, time.Minutes, time.Seconds, time.Milliseconds);

			if (this.LookMatch(0, TokenType.BAREKEY) && this.GetToken(0).Text.ToUpper() == "Z") {
				this.Consume(TokenType.BAREKEY);
				return new TomlDateTimeOffset(new DateTimeOffset(dateTime, TimeSpan.FromMilliseconds(0)));
			}

			if (this.Match(TokenType.PLUS)) {
				Tuple<Int32, Int32> offset = this.ParseHoursAndMinutes();
				return new TomlDateTimeOffset(new DateTimeOffset(dateTime, new TimeSpan(offset.Item1, offset.Item2, 0)));
			}

			if (this.Match(TokenType.MINUS)) {
				Tuple<Int32, Int32> offset = this.ParseHoursAndMinutes();
				return new TomlDateTimeOffset(new DateTimeOffset(dateTime, new TimeSpan(-offset.Item1, -offset.Item2, 0)));
			}

			return new TomlDateTime(dateTime);
		}

		private String ParseKey() {
			String key = "";

			while (!this.LookMatch(0, TokenType.DOT) && !this.LookMatch(0, TokenType.RBRACKET)
				&& !this.LookMatch(0, TokenType.ASSIGNMENT)) {
				Token currentToken = this.GetToken(0);
				key += this.Consume(currentToken.Type).Text;
			}

			return key;
		}

		private TomlTable GetTableOrCreateIfAbsent(String name, TomlTable parentTable) {
			TomlTable table;

			if (parentTable.Contains(name)) {
				if (parentTable[name] is TomlTable tomlTable) {
					table = tomlTable;
				}
				else {
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