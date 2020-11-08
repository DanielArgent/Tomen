using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomen;

namespace UnitTest {
	[TestClass]
	public class UnitTest {
		[TestMethod]
		public void TestInvalidComments() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
		   Tomen.Tomen.ReadFile("toml\\invalid-comments.toml"));
		}

		[TestMethod]
		public void TestStringLiteral() {
			TomlTable table = Tomen.Tomen.ReadFile("toml\\strings.toml");

			Assert.AreEqual("valueE", (table["key"] as TomlString).Value);
			Assert.AreEqual("\r\nmulitilinevalue\r\n", (table["key-2"] as TomlString).Value);
			Assert.AreEqual("value", (table["key-3"] as TomlString).Value);
			Assert.AreEqual("\r\n\tmultiline literal\\\r\n\tstring\r\n", (table["key-4"] as TomlString).Value);
		}

		[TestMethod]
		public void TestKeyValuePair() {
			TomlTable table = Tomen.Tomen.ReadFile("toml\\key-value-pair.toml");

			Assert.AreEqual(1, (table["barekey"] as TomlInt).Value);
			Assert.AreEqual(2, (table["bare_key"] as TomlInt).Value);
			Assert.AreEqual(3, (table["bare-key"] as TomlInt).Value);
			Assert.AreEqual(4, (table["quoted"] as TomlInt).Value);
			Assert.AreEqual(5, (table["single \"quoted\""] as TomlInt).Value);
			Assert.AreEqual(6, (table["ʎǝʞ"] as TomlInt).Value);
			Assert.AreEqual(7, (table[""] as TomlInt).Value);
			Assert.AreEqual(8, (table["1102"] as TomlInt).Value);
		}

		[TestMethod]
		public void TestDates() {
			TomlTable table = Tomen.Tomen.ReadFile("toml\\dates.toml");

			Assert.AreEqual(28, (table["date"] as TomlLocalDate).Day);
		}

		[TestMethod]
		public void TestInvalidKeyValuePairEnding() {
			Assert.ThrowsException<TomlParsingException>(() =>
		   Tomen.Tomen.ReadFile("toml\\invalid-key-value-pair-ending.toml"));
		}

		[TestMethod]
		public void TestInvalidUnspecifiedKey() {
			Assert.ThrowsException<TomlParsingException>(() =>
		   Tomen.Tomen.ReadFile("toml\\invalid-unspecified-key.toml"));
		}

		[TestMethod]
		public void TestInvalidMissedKey() {
			Assert.ThrowsException<TomlParsingException>(() =>
		   Tomen.Tomen.ReadFile("toml\\invalid-missed-key.toml"));
		}

		[TestMethod]
		public void TestInvalidTable() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-table.toml"));
		}
	}
}
