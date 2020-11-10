using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomen;

namespace UnitTest {
	[TestClass]
	public class TPathTest {
		[TestMethod]
		public void TestSimplePath() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\tpath\\empty-tables.toml");

			Assert.AreEqual("Hello!", root.Path<String>("x.y.z.w.value"));
		}

		[TestMethod]
		public void TestIndexPath() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\tpath\\index-path.toml");

			Assert.AreEqual(true, root.Path<Boolean>("fruit.apple[2].smooth"));
			Assert.AreEqual(true, root.Path<Boolean>("fruit[0][2].smooth"));
			Assert.AreEqual(true, root.Path<Boolean>("[0][0][2][0]"));
		}

		[TestMethod]
		public void TestStarPath() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\tpath\\star-path.toml");

			Assert.AreEqual(3, root.Path<Int32[]>("a.*").Length);
		}
	}
}
