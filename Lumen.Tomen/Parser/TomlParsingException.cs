using System;

namespace Tomen {
	public class TomlParsingException : Exception {
		public TomlParsingException(String message, String file, Int32 line) : 
			base($"{message} ({file}; {line})") {
		}
	}

	public class TomlSyntaxException : TomlParsingException {
		public TomlSyntaxException(String message, String file, Int32 line) :
			base(message, file, line) {
		}
	}

	public class TomlSemanticException : TomlParsingException {
		public TomlSemanticException(String message, String file, Int32 line) :
			base(message, file, line) {
		}
	}
}
