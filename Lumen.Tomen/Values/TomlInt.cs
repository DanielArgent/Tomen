using System;

namespace Lumen.Tomen {
	/// <summary> Toml integer value </summary>
	public class TomlInt : ITomlValue {
		public Int64 Value { get; }

		/// <summary> Creates a new Toml integer value </summary>
		public TomlInt(Byte value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml integer value </summary>
		public TomlInt(Int16 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml integer value </summary>
		public TomlInt(Int32 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml integer value </summary>
		public TomlInt(Int64 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml integer value </summary>
		public TomlInt(SByte value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml integer value </summary>
		public TomlInt(UInt16 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml integer value </summary>
		public TomlInt(UInt32 value) {
			this.Value = value;
		}

		/// <summary> Convert to valid literal </summary>
		public override String ToString() {
			return this.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
		}
	}
}
