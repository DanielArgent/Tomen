using System;
using System.Text.RegularExpressions;

namespace Tomen {
	public class TomlLocalTime : TomlValue {
		public Int32 Hours { get; }
		public Int32 Minutes { get; }
		public Int32 Seconds { get; }
		public Int32 Milliseconds { get; }

		public TomlLocalTime(Int32 hours = 0, Int32 minutes = 0, Int32 seconds = 0, Int32 milliseconds = 0) {
			this.Hours = hours;
			this.Minutes = minutes;
			this.Seconds = seconds;
			this.Milliseconds = milliseconds;
		}

		public override String ToString() {
			return $"{this.Hours}:{this.Minutes}:{this.Seconds}.{this.Milliseconds}";
		}
	}
}
