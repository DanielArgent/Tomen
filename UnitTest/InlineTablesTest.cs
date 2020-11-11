using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomen;

namespace UnitTest {
	[TestClass]
	public class InlineTablesTest {
		/*[TestMethod]
		public void TestInline() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\inline\\inline.toml");

			Assert.AreEqual("Tom", root.Path<String>("name.first"));
			Assert.AreEqual(1, root.Path<Int64>("point.x"));
			Assert.AreEqual(2, root.Path<Int64>("point.y"));
			Assert.AreEqual("pug", root.Path<String>("animal.type.name"));
		}

		[TestMethod]
		public void TestInvalidInline() {
			Assert.ThrowsException<TomlSyntaxException>(() => 
			Tomen.Tomen.ReadFile("toml\\inline\\invalid-inline.toml"));
		}

		[TestMethod]
		public void TestInvalidInlineRedefKey() {
			Assert.ThrowsException<TomlSemanticException>(() =>
			Tomen.Tomen.ReadFile("toml\\inline\\invalid-inline-redef-key.toml"));
		}

		[TestMethod]
		public void TestInvalidInlineRedefKeyInSubtable() {
			Assert.ThrowsException<TomlSemanticException>(() =>
			Tomen.Tomen.ReadFile("toml\\inline\\invalid-inline-redef-key-insub.toml"));
		}

		[TestMethod]
		public void TestInvalidInlineClosed() {
			Assert.ThrowsException<TomlSemanticException>(() =>
			Tomen.Tomen.ReadFile("toml\\inline\\invalid-inline-closed.toml"));
		}
		*/
		[TestMethod]
		public void TestInvalidInlineClosed2() {
			Assert.ThrowsException<TomlSemanticException>(() =>
			Tomen.Tomen.ReadFile("toml\\inline\\invalid-inline-closed-2.toml"));
		}
		/*
		[TestMethod]
		public void TestInvalidInlineAlreadyDef() {
			Assert.ThrowsException<TomlSemanticException>(() =>
			Tomen.Tomen.ReadFile("toml\\inline\\invalid-inline-already-def.toml"));
		}*/
	}
}
