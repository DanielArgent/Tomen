using System;

using Tomen;

namespace Test.Tomen {
	internal static class Program {
		internal static void Main(String[] args) {
			Console.WriteLine(Test1());
			Console.WriteLine(TestStrings());
			Console.WriteLine(Test2());
			Console.WriteLine(Test3());
			Console.WriteLine(Test4());
			Console.WriteLine(Test5());

			Console.ReadKey();
		}

		private static Boolean Test1() {
			Console.WriteLine("=============== TEST-1 ==================");

			TomlTable table = global::Tomen.Tomen.ReadFile("data1.toml");

			if (table.Contains("key")) {
				ITomlValue value = table["key"];

				if (value is TomlString tomenString) {
					return tomenString.Value == "value";
				}
			}

			return false;
		}

		private static Boolean TestStrings() {
			Console.WriteLine("=============== STRINGS ==================");

			TomlTable table = global::Tomen.Tomen.ReadFile("strings.toml");

			Console.WriteLine(table);

			global::Tomen.Tomen.ToXml(table, "strings.xml");

			return false;
		}

		private static Boolean Test2() {
			Console.WriteLine("=============== TEST-2 ==================");

			TomlTable table = global::Tomen.Tomen.ReadFile("data2.toml");

			Console.WriteLine((table["str"] as TomlString).Value);
			Console.WriteLine((table["strl"] as TomlString).Value);
			Console.WriteLine((table["newStr"] as TomlString).Value);

			Console.WriteLine(table["342"]);

			return table.Contains("invalid key");
		}

		private static Boolean Test3() {
			Console.WriteLine("=============== TEST-3 ==================");

			TomlTable table = global::Tomen.Tomen.ReadFile("data3.toml");

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
						//return (value as TomlTable).Contains("k");
					}
				}
				else {
					return false;
				}
			}

			if (table.Contains("x")) {
				Console.WriteLine(table["x"]);
			}

			global::Tomen.Tomen.WriteFile("data3.txt", table); // does not work

			return false;
		}

		private static Boolean Test4() {
			Console.WriteLine("=============== TEST-4 ==================");

			TomlTable root = global::Tomen.Tomen.ReadFile("data4.toml");

			Console.WriteLine(root["name"]);

			return true;
		}

		private static Boolean Test5() {
			Console.WriteLine("=============== TEST-5 ==================");

			TomlTable myTable = new TomlTable(null) {
				["x"] = new TomlInt(5),
				["y"] = new TomlInt(6)
			};

			global::Tomen.Tomen.WriteFile("x.toml", myTable);

			global::Tomen.Tomen.ToXml(myTable, "x.xml");

			return true;
		}
	}
}
