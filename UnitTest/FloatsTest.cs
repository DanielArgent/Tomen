using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomen;

namespace UnitTest {
	[TestClass]
	public class FloatsTest {
		[TestMethod]
		public void TestBasics() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\floats\\basics.toml");

			Assert.AreEqual(9.3, root.Path<Double>("f1"));
			Assert.AreEqual(9.0, root.Path<Double>("f2"));
			Assert.AreEqual(0.0, root.Path<Double>("f3"));
		}

		[TestMethod]
		public void TestSigned() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\floats\\signed.toml");

			Assert.AreEqual(-2.0, root.Path<Double>("f1"));
			Assert.AreEqual(0.0, root.Path<Double>("f2"));
			Assert.AreEqual(-0.0, root.Path<Double>("f3"));
			Assert.AreEqual(2, root.Path<Double>("f4"));
		}

		[TestMethod]
		public void TestUnderscores() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\floats\\underscores.toml");

			Assert.AreEqual(5.3254, root.Path<Double>("f1"));
			Assert.AreEqual(3432.5623, root.Path<Double>("f2"));
		}

		[TestMethod]
		public void TestExponential() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\floats\\special-values.toml");

			Assert.IsTrue(Double.IsInfinity(root.Path<Double>("sf1")));
			Assert.IsTrue(Double.IsPositiveInfinity(root.Path<Double>("sf2")));
			Assert.IsTrue(Double.IsNegativeInfinity(root.Path<Double>("sf3")));

			Assert.IsTrue(Double.IsNaN(root.Path<Double>("sf4")));
			Assert.IsTrue(Double.IsNaN(root.Path<Double>("sf5")));
			Assert.IsTrue(Double.IsNaN(root.Path<Double>("sf6")));
		}
	}
}
