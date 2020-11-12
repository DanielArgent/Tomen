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

	[TestClass]
	public class TableArrayTest {
		[TestMethod]
		public void TestBasics() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\array_tables\\basics.toml");

			Assert.AreEqual("Hammer", root.Path<String>("products[0].name"));
		}

		[TestMethod]
		public void TestSubtables() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\array_tables\\subtables.toml");

			Assert.AreEqual("apple", root.Path<String>("fruit[0].name"));
			Assert.AreEqual("red", root.Path<String>("fruit[0].physical.color"));
			Assert.AreEqual("plantain", root.Path<String>("fruit[1].variety[0].name"));
		}

		[TestMethod]
		public void TestInvalidSubtables() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\array_tables\\invalid-subtables.toml"));
		}

		[TestMethod]
		public void TestInvalidSubtables2() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\array_tables\\invalid-subtables.toml"));
		}
	}
}
