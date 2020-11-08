using System;

namespace Tomen {
	public class TomlException : Exception {
		public TomlException(String message, String file, Int32 line) : 
			base($"{message} ({file}; {line})") {
		}
	}

	public class TomlSyntaxException : TomlException {
		public TomlSyntaxException(String message, String file, Int32 line) :
			base(message, file, line) {
		}
	}

	public class TomlSemanticException : TomlException {
		public TomlSemanticException(String message, String file, Int32 line) :
			base(message, file, line) {
		}
	}
}
