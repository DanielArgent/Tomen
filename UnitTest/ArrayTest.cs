using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomen;

namespace UnitTest {
	[TestClass]
	public class ArrayTest {
		[TestMethod]
		public void TestMultiline() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\arrays\\multiline-array.toml");

			Int64[] ma = root.Path<Int64[]>("ma");

			Assert.AreEqual(3, ma.Length);
			Assert.AreEqual(1, ma[0]);
			Assert.AreEqual(2, ma[1]);
			Assert.AreEqual(3, ma[2]);
		}
	}
}
