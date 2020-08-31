using System;
using System.Text.RegularExpressions;

namespace Tomen {
	/// <summary> Toml double value </summary>
	public class TomlDateTime : ITomlValue {
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
	public class TomlDateTimeOffset : ITomlValue {
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


	/// <summary> Toml double value </summary>
	public class TomlLocalTime : ITomlValue {
		public Int32 Hours { get; }
		public Int32 Minutes { get; }
		public Int32 Seconds { get; }
		public Int32 Milliseconds { get; }

		/// <summary> Creates a new Toml double value </summary>
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

		internal static ITomlValue Parse(String text) {
			Match match = Regex.Match(text, @"^(?<h>\d{2}):(?<m>\d{2}):(?<s>\d{2})(\.(?<ms>\d{3}))?$");

			if(match.Success) {
				Int32 hours = Convert.ToInt32(match.Groups["h"].Value);
				Int32 minutes = Convert.ToInt32(match.Groups["m"].Value);
				Int32 seconds = Convert.ToInt32(match.Groups["s"].Value);
				Int32 milliseconds = match.Groups["ms"].Success ? Convert.ToInt32(match.Groups["ms"].Value) : 0;

				return new TomlLocalTime(hours, minutes, seconds, milliseconds);
			}

			throw new Exception("can not parse local time");
		}
	}
}
