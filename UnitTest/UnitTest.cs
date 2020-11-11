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

			Assert.AreEqual("valueE", table["key"].AsString());
			Assert.AreEqual("mulitilinevalue\r\n", table["key-2"].AsString());
			Assert.AreEqual("value", table["key-3"].AsString());
			Assert.AreEqual("\tmultiline literal\\\r\n\tstring\r\n", table["key-4"].AsString());
		}

		[TestMethod]
		public void TestStringEscapeWs() {
			TomlTable table = Tomen.Tomen.ReadFile("toml\\strings-escape-ws.toml");

			String str1 = table["str1"].AsString();
			String str2 = table["str2"].AsString();
			String str3 = table["str3"].AsString();

			Assert.AreEqual(str1, str2);
			Assert.AreEqual(str2, str3);
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
		public void TestInvalidStringLiteral() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-string-literal.toml"));
		}

		[TestMethod]
		public void TestInvalidStringLiteral2() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-string-literal-2.toml"));
		}

		[TestMethod]
		public void TestInvalidStringLiteral3() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-string-literal-3.toml"));
		}

		[TestMethod]
		public void TestInvalidStringLiteral4() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-string-literal-4.toml"));
		}

		[TestMethod]
		public void TestInvalidUnicodeSeq() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-unicode-seq.toml"));
		}

		[TestMethod]
		public void TestInvalidKeyValuePairEnding() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
		   Tomen.Tomen.ReadFile("toml\\invalid-key-value-pair-ending.toml"));
		}

		[TestMethod]
		public void TestInvalidUnspecifiedKey() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
		   Tomen.Tomen.ReadFile("toml\\invalid-unspecified-key.toml"));
		}

		[TestMethod]
		public void TestInvalidMissedKey() {
			Assert.ThrowsException<TomlSyntaxException>(() =>
		   Tomen.Tomen.ReadFile("toml\\invalid-missed-key.toml"));
		}

		[TestMethod]
		public void TestDottedKeys() {
			TomlTable table = Tomen.Tomen.ReadFile("toml\\dotted-keys.toml");

			Assert.AreEqual("Orange", (table["name"] as TomlString).Value);

			var physical = table["physical"] as TomlTable;

			Assert.AreEqual("orange", (physical["color"] as TomlString).Value);
			Assert.AreEqual("round", (physical["shape"] as TomlString).Value);

			var site = table["site"] as TomlTable;
			Assert.AreEqual(true, (site["google.com"] as TomlBool).Value);
		}

		[TestMethod]
		public void TestNumericDottedKeys() {
			TomlTable table = Tomen.Tomen.ReadFile("toml\\numeric-dotted-keys.toml");

			var physical = table["3"] as TomlTable;

			Assert.AreEqual("pi", (physical["14159"] as TomlString).Value);
		}

		[TestMethod]
		public void TestInvalidDottedKeys() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-dotted-keys.toml"));
		}

		[TestMethod]
		public void TestInvalidDefiningSameKey() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-defining-same-key.toml"));
		}

		[TestMethod]
		public void TestInvalidDefiningSameKeyDiffWay() {
			Assert.ThrowsException<TomlSemanticException>(() =>
				Tomen.Tomen.ReadFile("toml\\invalid-defining-same-key-diff-way.toml"));
		}
	}
}
