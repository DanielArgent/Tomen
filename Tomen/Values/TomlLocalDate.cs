using System;
using System.Text.RegularExpressions;

namespace Tomen {
	public class TomlLocalDate : TomlValue {
		public Int32 Year { get; }
		public Int32 Month { get; }
		public Int32 Day { get; }

		public TomlLocalDate(Int32 year = 0, Int32 month = 0, Int32 day = 0) {
			this.Year = year;
			this.Month = month;
			this.Day = day;
		}

		public override String ToString() {
			return $"{this.Year}-{this.Month}-{this.Day}";
		}
	}
}
