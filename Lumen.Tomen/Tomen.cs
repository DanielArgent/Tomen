using System;
using System.IO;

namespace Lumen.Tomen {
	/// <summary> Main Tomen class </summary>
	public static class Tomen {
		/// <summary> Reads file into Tomen object </summary>
		/// <param name="path">Path to file</param>
		/// <returns> Root table </returns>
		public static TomlTable ReadFile(String path) {
			return new Parser(new Lexer(File.ReadAllText(path), path).Tokenization(), path).Parsing();
		}

		/// <summary> Writes Tomen object into file </summary>
		/// <param name="path"> Path to file </param>
		/// <param name="value"> Root table </param>
		public static void WriteFile(String path, ITomlValue value) {
			File.WriteAllText(path, value.ToString());
		}
	}
}
