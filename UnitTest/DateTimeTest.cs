using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tomen;

namespace UnitTest {
	[TestClass]
	public class DateTimeTest {
		[TestMethod]
		public void TestLocalDate() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\dates\\local-date.toml");

			Assert.AreEqual(new DateTime(1999, 10, 28), root.Path<DateTime>("date"));
		}

		[TestMethod]
		public void TestLocalTime() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\dates\\local-time.toml");

			Assert.AreEqual(new DateTime(1, 1, 1, 5, 53, 45), root.Path<DateTime>("time1"));
			Assert.AreEqual(new DateTime(1, 1, 1, 5, 53, 45, 993), root.Path<DateTime>("time2"));
		}

		[TestMethod]
		public void TestOffsetDateTime() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\dates\\offset-date-time.toml");

			Assert.IsTrue(new DateTimeOffset(new DateTime(1979, 05, 27, 07, 32, 0), new TimeSpan()).EqualsExact( 
				root.Path<DateTimeOffset>("odt1")));
			Assert.IsTrue(new DateTimeOffset(new DateTime(1979, 05, 27, 0, 32, 0), new TimeSpan(-7, 0, 0)).EqualsExact(
				root.Path<DateTimeOffset>("odt2")));
			Assert.IsTrue(new DateTimeOffset(new DateTime(1979, 05, 27, 0, 32, 0, 999), new TimeSpan(-7, 0, 0)).EqualsExact(
				root.Path<DateTimeOffset>("odt3")));
		}


		[TestMethod]
		public void TestOffsetDateTimeSpaced() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\dates\\offset-date-time-spaced.toml");

			Assert.IsTrue(new DateTimeOffset(new DateTime(1979, 05, 27, 07, 32, 0), new TimeSpan()).EqualsExact(
				root.Path<DateTimeOffset>("odt1")));
			Assert.IsTrue(new DateTimeOffset(new DateTime(1979, 05, 27, 0, 32, 0), new TimeSpan(-7, 0, 0)).EqualsExact(
				root.Path<DateTimeOffset>("odt2")));
			Assert.IsTrue(new DateTimeOffset(new DateTime(1979, 05, 27, 0, 32, 0, 999), new TimeSpan(-7, 0, 0)).EqualsExact(
				root.Path<DateTimeOffset>("odt3")));
		}

		[TestMethod]
		public void TestDateTime() {
			TomlTable root = Tomen.Tomen.ReadFile("toml\\dates\\date-time.toml");

			Assert.AreEqual(new DateTime(1979, 05, 27, 07, 32, 0),
				root.Path<DateTime>("ldt1"));
			Assert.AreEqual(new DateTime(1979, 05, 27, 0, 32, 0, 999), 
				root.Path<DateTime>("ldt2"));
		}
	}
}
