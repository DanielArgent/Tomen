using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomen;

namespace UnitTest {
	[TestClass]
	public class IntegersTest {
		[TestMethod]
		public void TestSigned() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\integers\\signed.toml");

			Assert.AreEqual(99, (root["int1"] as TomlInt).Value);
			Assert.AreEqual(42, (root["int2"] as TomlInt).Value);
			Assert.AreEqual(0, (root["int3"] as TomlInt).Value);
			Assert.AreEqual(-17, (root["int4"] as TomlInt).Value);
		}

		[TestMethod]
		public void TestUnderscores() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\integers\\underscores.toml");

			Assert.AreEqual(1_000, (root["int5"] as TomlInt).Value);
			Assert.AreEqual(5_349_221, (root["int6"] as TomlInt).Value);
			Assert.AreEqual(53_49_221, (root["int7"] as TomlInt).Value);
			Assert.AreEqual(1_2_3_4_5, (root["int8"] as TomlInt).Value);
		}

		[TestMethod]
		public void TestBased() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\integers\\based.toml");

			Assert.AreEqual(0xDEADBEEF, (root["hex1"] as TomlInt).Value);
			Assert.AreEqual(0xDEADBEEF, (root["hex2"] as TomlInt).Value);
			Assert.AreEqual(0xDEADBEEF, (root["hex3"] as TomlInt).Value);
			Assert.AreEqual(Convert.ToInt64("01234567", 8), (root["oct1"] as TomlInt).Value);
			Assert.AreEqual(Convert.ToInt64("755", 8), (root["oct2"] as TomlInt).Value);
			Assert.AreEqual(0b11010110, (root["bin1"] as TomlInt).Value);
		}
	}
}
