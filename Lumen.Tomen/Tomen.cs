using System;
using System.IO;

namespace Lumen.Tomen {
	public static class Tomen {
		public static ITomenValue ReadFile(String path) {
			return new Parser(new Lexer(File.ReadAllText(path), path).Tokenization(), path).Parsing();
		}

		public static void WriteFile(String path, ITomenValue value) {
			File.WriteAllText(path, value.ToString());
		}
	}
}
