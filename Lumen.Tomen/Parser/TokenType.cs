using System;

namespace Tomen {
	internal enum TokenType {
		BAREKEY,
		TEXT,

		LBRACKET,
		RBRACKET,

		ASSIGNMENT,
		EOF,
		DOT,
		DOUBLE,
		INT,
		PLUS,
		MINUS,
		INF,
		NAN,
		TRUE,
		FALSE,
		SPLIT,
		LBRACE,
		RBRACE,
		NL,
		DATE_TIME,
		LOCAL_TIME,
		DATE_TIME_OFFSET
	}
}
