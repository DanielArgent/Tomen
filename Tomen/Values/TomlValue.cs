using System;

namespace Tomen {
	/// <summary> Base class for all Toml values</summary>
	public abstract class TomlValue {
		public static implicit operator TomlValue(string value) =>
		   new TomlString(value);

		public static implicit operator TomlValue(float value) =>
		   new TomlDouble(value);

		public static implicit operator TomlValue(double value) =>
			new TomlDouble(value);

		public static implicit operator TomlValue(int value) =>
		   new TomlInt(value);

		public static implicit operator TomlValue(long value) =>
		   new TomlInt(value);

		public static implicit operator TomlValue(byte value) =>
		   new TomlInt(value);

		public static implicit operator TomlValue(bool value) =>
			new TomlBool(value);

		public static implicit operator TomlValue(DateTime value) =>
			new TomlDateTime(value);

		public static implicit operator TomlValue(TomlValue[] nodes) {
			var list = new System.Collections.Generic.List<TomlValue>();

			list.AddRange(nodes);

			return new TomlArray(list);
		}

		public static implicit operator string(TomlValue value) => value.ToString();

		public static implicit operator int(TomlValue value) => (int)value.AsInt().Value;

		public static implicit operator long(TomlValue value) => value.AsInt().Value;

		public static implicit operator float(TomlValue value) => (float)value.AsDouble().Value;

		public static implicit operator double(TomlValue value) => value.AsDouble().Value;

		public static implicit operator bool(TomlValue value) => value.AsBoolean().Value;
	}
}
