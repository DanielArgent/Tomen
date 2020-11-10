using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Tomen {
	internal class TPathParser {
		private readonly List<Token> tokens;
		private readonly Int32 size;
		private Int32 position;
		private Int32 currentLine;

		internal TPathParser(List<Token> Tokens) {
			this.tokens = Tokens;
			this.size = Tokens.Count;
			this.position = 0;
			this.currentLine = 0;
		}

		internal TPath Parse() {
			return Dot();
		}

		private TPath Dot() {
			var result = Index();

			while (this.Match(TokenType.DOT)) {
				result = new TPathDot(result, this.Dot());
			}

			return result;
		}

		private TPath Index() {
			var result = Primary();

			while (this.Match(TokenType.LBRACKET)) {
				var index = Int32.Parse(this.Consume(TokenType.BAREKEY).Text);
				this.Match(TokenType.RBRACKET);

				result = new TPathIndex(result, index);
			}

			return result;
		}

		private TPath Primary() {
			if (this.LookMatch(0, TokenType.BAREKEY)) {
				return new TPathKey(this.Consume(TokenType.BAREKEY).Text, this.currentLine);
			}

			if (this.Match(TokenType.LBRACKET)) {
				var index = Int32.Parse(this.Consume(TokenType.BAREKEY).Text);
				this.Match(TokenType.RBRACKET);

				return new TPathIndex(null, index);
			}

			if (this.Match(TokenType.STAR)) {
				TPath result = new TPathAllValues();
				while(this.Match(TokenType.STAR)) {
					result = new TPathDot(result, new TPathAllValues());
				}

				return result;
			}

			throw new Exception();
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
					"", this.currentLine);
			}

			this.position++;
			return currentToken;
		}
	}
}
