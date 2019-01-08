using System;

namespace Lumen.Tomen {
	internal enum TokenType {
		NAME,
		TEXT,

		LBRACKET,
		RBRACKET,

		ASSIGNMENT,
		EOF,
		DOT
	}
}
