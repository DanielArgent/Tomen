using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomen;

namespace UnitTest {
	[TestClass]
	public class TablesTest {
		[TestMethod]
		public void TestExplicitlyCorrect() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\tables\\explicitly-correct.toml");

			Assert.AreEqual(5, root.Path<Int64>("a.y"));
			Assert.AreEqual(9, root.Path<Int64>("a.b.x"));
		}

		[TestMethod]
		public void TestExplicitlyCorrect2() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\tables\\explicitly-correct-2.toml");

			Assert.AreEqual(9, root.Path<Int64>("a.b.x"));
		}

		[TestMethod]
		public void TestExplicitlyNotCorrect() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\tables\\explicitly-not-correct.toml"));
		}

		[TestMethod]
		public void TestInvalidRedefinition() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\tables\\invalid-redefinition.toml"));
		}

		[TestMethod]
		public void TestTableExtension() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\tables\\table-extension.toml");

			Assert.AreEqual(true, root.Path<Boolean>("fruit.apple.texture.smooth"));
		}

		[TestMethod]
		public void TestInvalidTableExtension() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\tables\\invalid-table-extension.toml"));
		}

		[TestMethod]
		public void TestEmptyTables() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\tables\\empty-tables.toml");

			Assert.AreEqual("Hello!", root.Path<String>("x.y.z.w.value"));
		}
	}
}
