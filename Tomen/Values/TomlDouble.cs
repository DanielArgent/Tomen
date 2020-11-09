using System;

namespace Tomen {
	/// <summary> Toml double value </summary>
	public class TomlDouble : TomlValue {
		/// <summary> Internal value </summary>
		public Double Value { get; }

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(Single value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(Double value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(Byte value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(Int16 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(Int32 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(Int64 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(SByte value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(UInt16 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(UInt32 value) {
			this.Value = value;
		}

		/// <summary> Creates a new Toml double value </summary>
		public TomlDouble(UInt64 value) {
			this.Value = value;
		}

		/// <summary> Convert to valid literal </summary>
		public override String ToString() {
			if (Double.IsNegativeInfinity(this.Value)) {
				return "-inf";
			}

			if (Double.IsPositiveInfinity(this.Value)) {
				return "+inf";
			}

			if (Double.IsNaN(this.Value)) {
				return "nan";
			}

			return this.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
		}
	}
}
