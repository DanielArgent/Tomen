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

				if (this.LookMatch(0, TokenType.LBRACKET) && this.LookMatch(1, TokenType.LBRACKET)) {
					this.ParseArrayOfTables(rootTable);
				}
				else if (this.Match(TokenType.LBRACKET)) {
					this.ParseTableDefinition(rootTable);
				}
				else if (this.LookMatch(1, TokenType.DOT)) {
					this.ParseDottedKeyValuePair(rootTable);
				}
				else {
					this.ParseKeyValuePair(rootTable);
				}
			}

			return rootTable;
		}

		private void ParseArrayOfTables(TomlTable rootTable) {
			this.Match(TokenType.LBRACKET);
			this.Match(TokenType.LBRACKET);

			String key;
			TomlTable table = rootTable;

			while (this.LookMatch(1, TokenType.DOT)) {
				key = this.Consume(TokenType.BAREKEY).Text;
				table = this.GetTableOrCreateIfAbsent(key, table);
				this.Match(TokenType.DOT);
			}

			key = this.Consume(TokenType.BAREKEY).Text;

			this.Consume(TokenType.RBRACKET);
			this.Consume(TokenType.RBRACKET);
			this.Match(TokenType.NL);

			var newTable = new TomlTable(null);

			while (!this.LookMatch(0, TokenType.LBRACKET) && !this.LookMatch(0, TokenType.EOF)) {
				if (this.LookMatch(1, TokenType.DOT)) {
					this.ParseDottedKeyValuePair(newTable);
				}
				else {
					this.ParseKeyValuePair(newTable);
				}
			}

			if (table.Contains(key)) {
				if (table[key] is TomlArrayOfTables arrayOfTables) {
					arrayOfTables.Value.Add(newTable);
				}
				else {
					throw new TomlSemanticException($"key '{key}' is already exists and it is not an array of tables", this.currentFile, this.currentLine);
				}
			}
			else {
				table[key] = new TomlArrayOfTables(new List<TomlValue> { newTable });
			}
		}

		private void ParseTableDefinition(TomlTable parentTable) {
			String key = this.Consume(TokenType.BAREKEY).Text;
			TomlTable table = this.GetTableOrCreateIfAbsent(key, parentTable);

			while (this.Match(TokenType.DOT)) {
				key = this.Consume(TokenType.BAREKEY).Text;
				table = this.GetTableOrCreateIfAbsent(key, table);
			}

			this.Consume(TokenType.RBRACKET);
			this.Match(TokenType.NL);

			while (!this.LookMatch(0, TokenType.LBRACKET) && !this.LookMatch(0, TokenType.EOF)) {
				if (this.LookMatch(1, TokenType.DOT)) {
					this.ParseDottedKeyValuePair(table);
				}
				else {
					this.ParseKeyValuePair(table);
				}
			}

			if (table.pairs.Count != 0) {
				table.IsClosed = true;
			}
		}

		private void ParseDottedKeyValuePair(TomlTable parentTable) {
			String key;
			TomlTable table = parentTable;

			while (this.LookMatch(1, TokenType.DOT)) {
				key = this.Consume(TokenType.BAREKEY).Text;
				table = this.GetTableOrCreateDottedIfAbsent(key, table);
				this.Match(TokenType.DOT);
			}

			key = this.Consume(TokenType.BAREKEY).Text;

			this.Consume(TokenType.ASSIGNMENT);

			// There is no value
			if (this.Match(TokenType.NL) || this.Match(TokenType.EOF)) {
				throw new TomlSyntaxException("unspecified value", this.currentFile, this.currentLine);
			}

			var value = this.ParseValue();

			if (!this.Match(TokenType.NL) && !this.LookMatch(0, TokenType.EOF)) {
				throw new TomlSyntaxException("there must be a newline or end of file after a key/value pair", this.currentFile, this.currentLine);
			}

			if (parentTable.Contains(key)) {
				throw new TomlSemanticException($"key {key} is already defined in this table", this.currentFile, this.currentLine);
			}

			if (table.IsClosed) {
				throw new TomlSemanticException($"table '{table.Name}' is closed ", this.currentFile, this.currentLine);
			}

			table[key] = value;
		}

		private void ParseKeyValuePair(TomlTable parentTable) {
			// If there is no key
			if (this.Match(TokenType.ASSIGNMENT)) { // generalize it on any token except barekey
				throw new TomlSyntaxException("expected key, got '='", this.currentFile, this.currentLine);
			}

			String key = this.Consume(TokenType.BAREKEY).Text;

			this.Consume(TokenType.ASSIGNMENT);

			// There is no value
			if (this.Match(TokenType.NL) || this.Match(TokenType.EOF)) {
				throw new TomlSyntaxException("unspecified value", this.currentFile, this.currentLine);
			}

			var value = this.ParseValue();

			if (!this.Match(TokenType.NL) && !this.LookMatch(0, TokenType.EOF)) {
				throw new TomlSyntaxException("there must be a newline or end of file after a key/value pair", this.currentFile, this.currentLine);
			}

			if (parentTable.IsClosed) {
				throw new TomlSemanticException($"table '{parentTable.Name}' is closed ", this.currentFile, this.currentLine);
			}

			if (parentTable.Contains(key)) {
				throw new TomlSemanticException($"key {key} is already defined in this table", this.currentFile, this.currentLine);
			}

			parentTable[key] = value;
		}

		private TomlValue ParseValue() {
			if (this.Match(TokenType.PLUS)) {
				return ParsePlus();
			}

			if (this.Match(TokenType.MINUS)) {
				return ParseMinus();
			}

			if (this.LookMatch(0, TokenType.TEXT)) {
				return new TomlString(this.Consume(TokenType.TEXT).Text);
			}

			if (this.LookMatch(0, TokenType.NUMBER) && this.LookMatch(1, TokenType.COLON)) {
				return this.ParseLocalTime();
			}

			if (this.LookMatch(0, TokenType.NUMBER) && this.LookMatch(1, TokenType.MINUS)) {
				return this.ParseDate();
			}

			if (this.LookMatch(0, TokenType.NUMBER)) {
				String digits = this.Consume(TokenType.NUMBER).Text.Replace("_", "");

				if (digits.Contains("e") || digits.Contains(".")) {
					return new TomlDouble(Double.Parse(digits, CultureInfo.InvariantCulture));
				}

				return new TomlInt(Int64.Parse(digits, NumberStyles.Any));
			}

			if (this.LookMatch(0, TokenType.NUMBER_WITH_BASE)) {
				return ParseNumberWithBase();
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
				return ParseArray();
			}

			if (this.Match(TokenType.LBRACE)) {
				return ParseInineTable();
			}

			return TomlNull.NULL;
		}

		private TomlValue ParseInineTable() {
			TomlTable inlineTable = new TomlInlineTable();

			while (!this.Match(TokenType.RBRACE)) {
				// If there is no key
				if (!this.LookMatch(0, TokenType.BAREKEY)) {
					throw new TomlSyntaxException($"expected key, got '{GetToken(0)}'", this.currentFile, this.currentLine);
				}

				String key;
				TomlTable table = inlineTable;

				while (this.LookMatch(1, TokenType.DOT)) {
					key = this.Consume(TokenType.BAREKEY).Text;
					table = this.GetTableOrCreateDottedIfAbsent(key, table);
					this.Match(TokenType.DOT);
				}

				key = this.Consume(TokenType.BAREKEY).Text;

				this.Consume(TokenType.ASSIGNMENT);

				// There is no value
				if (this.Match(TokenType.NL) || this.Match(TokenType.EOF) || this.Match(TokenType.RBRACE)) {
					throw new TomlSyntaxException("unspecified value", this.currentFile, this.currentLine);
				}

				var value = this.ParseValue();

				if (table.Contains(key)) {
					throw new TomlSemanticException($"key {key} is already defined in this table", this.currentFile, this.currentLine);
				}

				table[key] = value;

				this.Match(TokenType.SPLIT);
			}

			return inlineTable;
		}

		private TomlValue ParseArray() {
			List<TomlValue> items = new List<TomlValue>();

			this.Match(TokenType.NL);
			while (!this.Match(TokenType.RBRACKET)) {
				items.Add(this.ParseValue());
				this.Match(TokenType.SPLIT);
				this.Match(TokenType.NL);
				this.Match(TokenType.SPLIT); // ?
			}

			return new TomlArray(items);
		}

		private TomlValue ParseNumberWithBase() {
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

			throw new TomlSemanticException("impossible base", this.currentFile, this.currentLine);
		}

		private TomlValue ParseMinus() {
			TomlValue value = this.ParseValue();

			if (value is TomlInt ti) {
				return new TomlInt(-ti.Value);
			}

			if (value is TomlDouble td) {
				return new TomlDouble(-td.Value);
			}

			throw new TomlSemanticException("expected numeric value", this.currentFile, this.currentLine);
		}

		private TomlValue ParsePlus() {
			TomlValue value = this.ParseValue();

			if (value is TomlInt || value is TomlDouble) {
				return value;
			}

			throw new TomlSemanticException("expected numeric value", this.currentFile, this.currentLine);
		}

		Tuple<Int32, Int32> ParseHoursAndMinutes() {
			Int32 hours = Convert.ToInt32(this.Consume(TokenType.NUMBER).Text);
			this.Consume(TokenType.COLON);
			Int32 minutes = Convert.ToInt32(this.Consume(TokenType.NUMBER).Text);

			return new Tuple<Int32, Int32>(hours, minutes);
		}

		TomlLocalTime ParseLocalTime() {
			Tuple<Int32, Int32> hoursAndMinutes = this.ParseHoursAndMinutes();
			this.Consume(TokenType.COLON);

			String secondsStr = this.Consume(TokenType.NUMBER).Text;
			if (secondsStr.Contains(".")) {
				String[] parts = secondsStr.Split('.');

				Int32 seconds = Convert.ToInt32(parts[0]);
				Int32 milliseconds = Convert.ToInt32(parts[1].Substring(0, 3));

				return new TomlLocalTime(hoursAndMinutes.Item1, hoursAndMinutes.Item2, seconds, milliseconds);
			}
			else {
				Int32 seconds = Convert.ToInt32(secondsStr);

				return new TomlLocalTime(hoursAndMinutes.Item1, hoursAndMinutes.Item2, seconds);
			}
		}

		TomlValue ParseDate() {
			Int32 year = Convert.ToInt32(this.Consume(TokenType.NUMBER).Text);
			this.Consume(TokenType.MINUS);
			Int32 month = Convert.ToInt32(this.Consume(TokenType.NUMBER).Text);
			this.Consume(TokenType.MINUS);
			Int32 day = Convert.ToInt32(this.Consume(TokenType.NUMBER).Text);


			if ((this.LookMatch(0, TokenType.BAREKEY)
				&& this.GetToken(0).Text.ToUpper() == "T") || this.GetToken(0).Type == TokenType.NUMBER) {
				return this.ParseDateTime(year, month, day);
			}
			else {
				return new TomlLocalDate(year, month, day);
			}


		}

		TomlValue ParseDateTime(Int32 year, Int32 month, Int32 day) {
			this.Match(TokenType.BAREKEY);

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

			return new TomlLocalDateTime(dateTime);
		}

		private TomlTable GetTableOrCreateDottedIfAbsent(String name, TomlTable parentTable) {
			TomlTable table;

			if (parentTable.Contains(name)) {
				if (parentTable[name] is TomlTable tomlTable) {
					table = tomlTable;
				}
				else if (parentTable[name] is TomlArrayOfTables tomlArrayOfTables) {
					return tomlArrayOfTables.Value[tomlArrayOfTables.Value.Count - 1] as TomlTable;
				}
				else {
					throw new TomlSemanticException($"value with key '{parentTable.Name}.{name}' is already exists and it is not a table", this.currentFile, this.currentLine);
				}
			}
			else {
				table = new TomlDottedTable(parentTable, name);
				parentTable[name] = table;
			}

			return table;
		}

		private TomlTable GetTableOrCreateIfAbsent(String name, TomlTable parentTable) {
			TomlTable table;

			if (parentTable.Contains(name)) {
				if (parentTable[name] is TomlTable tomlTable) {
					table = tomlTable;
				}
				else if (parentTable[name] is TomlArrayOfTables tomlArrayOfTables) {
					return tomlArrayOfTables.Value[tomlArrayOfTables.Value.Count - 1] as TomlTable;
				}
				else {
					throw new TomlSemanticException($"value with key '{parentTable.Name}.{name}' is already exists and it is not ITomlOpenTable", this.currentFile, this.currentLine);
				}
			}
			else {
				if (!(parentTable is TomlDottedTable) && parentTable.IsClosed) {
					throw new TomlSemanticException($"table '{parentTable.Name}' is closed ", this.currentFile, this.currentLine);
				}

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