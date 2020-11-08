using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomen {
	/// <summary> Toml null value </summary>
	public class TomlNull : ITomlValue {
		public static TomlNull NULL { get; } = new TomlNull();

		private TomlNull() {

		}
	}
}
