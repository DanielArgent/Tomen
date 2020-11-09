using System;
using System.Text.RegularExpressions;

namespace Tomen {
	/// <summary> Toml double value </summary>
	public class TomlDateTime : TomlValue {
		/// <summary> Internal value </summary>
		public DateTime Value { get; }

		/// <summary> Creates a new Toml double value </summary>
		public TomlDateTime(DateTime value) {
			this.Value = value;
		}

		/// <summary> Convert to valid literal </summary>
		public override String ToString() {
			return this.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + "." + this.Value.Millisecond;
		}
	}

	/// <summary> Toml double value </summary>
	public class TomlDateTimeOffset : TomlValue {
		/// <summary> Internal value </summary>
		public DateTimeOffset Value { get; }

		/// <summary> Creates a new Toml double value </summary>
		public TomlDateTimeOffset(DateTimeOffset value) {
			this.Value = value;
		}

		/// <summary> Convert to valid literal </summary>
		public override String ToString() {
			return this.Value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
		}
	}

	public class TomlLocalDate : TomlValue {
		public Int32 Year { get; }
		public Int32 Month { get; }
		public Int32 Day { get; }

		public TomlLocalDate(Int32 year = 0, Int32 month = 0, Int32 day = 0) {
			this.Year = year;
			this.Month = month;
			this.Day = day;	
		}

		/// <summary> Convert to valid literal </summary>
		public override String ToString() {
			return $"{this.Year}-{this.Month}-{this.Day}";
		}
	}

	public class TomlLocalTime : TomlValue {
		public Int32 Hours { get; }
		public Int32 Minutes { get; }
		public Int32 Seconds { get; }
		public Int32 Milliseconds { get; }

		public TomlLocalTime(Int32 hours = 0, Int32 minutes = 0, Int32 seconds = 0, Int32 milliseconds=0) {
			this.Hours = hours;
			this.Minutes = minutes;
			this.Seconds = seconds;
			this.Milliseconds = milliseconds;
		}

		/// <summary> Convert to valid literal </summary>
		public override String ToString() {
			return $"{this.Hours}:{this.Minutes}:{this.Seconds}.{this.Milliseconds}";
		}
	}
}
