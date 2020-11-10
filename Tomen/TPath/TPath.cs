using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Tomen {
	internal interface TPath {
		TomlValue Eval(TomlValue value);
	}
}
