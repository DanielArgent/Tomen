using System;

namespace Lumen.Tomen {
	public class TomlParsingException : Exception {
		public TomlParsingException(String message, String file, Int32 line) : 
			base($"{message} ({file}; {line})") {

		}
	}
}
