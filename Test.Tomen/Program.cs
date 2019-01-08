using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lumen.Tomen;

namespace Test.Tomen {
	internal static class Program {
		internal static void Main(string[] args) {
			Console.WriteLine(Test1());
			Console.WriteLine(Test2());
			Console.WriteLine(Test3());

			Console.ReadKey();
		}

		private static Boolean Test1() {
			TomenTable table = Lumen.Tomen.Tomen.ReadFile("data1.toml");

			if (table.Contains("key")) {
				ITomenValue value = table["key"];

				if (value is TomenString tomenString) {
					return tomenString.Value == "value";
				}
			}

			return false;
		}

		private static Boolean Test2() {
			TomenTable table = Lumen.Tomen.Tomen.ReadFile("data2.toml");

			return table.Contains("invalid key");
		}

		private static Boolean Test3() {
			TomenTable table = Lumen.Tomen.Tomen.ReadFile("data3.toml");

			if (table.Contains("t")) {
				ITomenValue value = table["t"];

				if (value is TomenTable tomenTable) {
					if(!tomenTable.Contains("v1") || !tomenTable.Contains("v2")) {
						return false;
					}
				} else {
					return false;
				}
			}

			if (table.Contains("t1")) {
				ITomenValue value = table["t1"];

				if (value is TomenTable tomenTable) {
					if (tomenTable.Contains("43")) {
						value = tomenTable["43"];
						return (value as TomenTable).Contains("k");
					}
				}
				else {
					return false;
				}
			}

			return false;
		}
	}
}
