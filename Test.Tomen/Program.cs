using System;

using Lumen.Tomen;

namespace Test.Tomen {
	internal static class Program {
		internal static void Main(String[] args) {
			Console.WriteLine(Test1());
			Console.WriteLine(Test2());
			Console.WriteLine(Test3());
			Console.WriteLine(Test4());

			Console.ReadKey();
		}

		private static Boolean Test1() {
			TomlTable table = Lumen.Tomen.Tomen.ReadFile("data1.toml");

			if (table.Contains("key")) {
				ITomlValue value = table["key"];

				if (value is TomlString tomenString) {
					return tomenString.Value == "value";
				}
			}

			return false;
		}

		private static Boolean Test2() {
			TomlTable table = Lumen.Tomen.Tomen.ReadFile("data2.toml");

			Console.WriteLine((table["str"] as TomlString).Value);
			Console.WriteLine((table["strl"] as TomlString).Value);
			Console.WriteLine((table["newStr"] as TomlString).Value);

			return table.Contains("invalid key");
		}

		private static Boolean Test3() {
			TomlTable table = Lumen.Tomen.Tomen.ReadFile("data3.toml");

			if (table.Contains("t")) {
				ITomlValue value = table["t"];

				if (value is TomlTable tomenTable) {
					if(!tomenTable.Contains("v1") || !tomenTable.Contains("v2")) {
						return false;
					}
				} else {
					return false;
				}
			}

			if (table.Contains("t1")) {
				ITomlValue value = table["t1"];

				if (value is TomlTable tomenTable) {
					if (tomenTable.Contains("43")) {
						value = tomenTable["43"];
						return (value as TomlTable).Contains("k");
					}
				}
				else {
					return false;
				}
			}

			return false;
		}

		private static Boolean Test4() {
			TomlTable myTable = new TomlTable(null) {
				["x"] = new TomlInt(5),
				["y"] = new TomlInt(6)
			};

			Lumen.Tomen.Tomen.WriteFile("x.toml", myTable);

			Lumen.Tomen.Tomen.ToXml(myTable, "x.xml");

			return true;
		}
	}
}
