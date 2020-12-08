using System;
using System.IO;
using System.Xml;

namespace Tomen {
	/// <summary> Main Tomen class </summary>
	public static class Tomen {
		/// <summary> Reads file into TomlTable </summary>
		/// <param name="path">Path to file</param>
		/// <returns> Root table </returns>
		public static TomlTable ReadFile(String path) {
			return new Parser(new Lexer(File.ReadAllText(path), path).GetTokens(), path).Parse();
		}

		/// <summary> Reads string into TomlTable </summary>
		/// <param name="path">String with TOML code</param>
		/// <returns>Root table</returns>
		public static TomlTable ReadString(String content) {
			return new Parser(new Lexer(content, "").GetTokens(), "").Parse();
		}

		/// <summary> Writes Toml value into file </summary>
		/// <param name="path"> Path to file </param>
		/// <param name="value"> Toml value </param>
		public static void WriteFile(String path, TomlValue value) {
			File.WriteAllText(path, value.ToString());
		}

		public static void ToXml(TomlValue value, String resultFile) {
			XmlDocument document = new XmlDocument();
			document.LoadXml($"<?xml version=\"1.0\" encoding=\"UTF-8\"?><root></root>");

			TomlValueToXml(value, document.DocumentElement, document);

			document.Save(resultFile);
		}

		private static void TomlValueToXml(TomlValue value, XmlElement node, XmlDocument document) {
			if (value is TomlTable table) {
				XmlElement tableNode = document.CreateElement(table.Name ?? "table");
				node.AppendChild(tableNode);
				foreach (System.Collections.Generic.KeyValuePair<String, TomlValue> i in table.pairs) {
					TomlValueToXml(i.Key, i.Value, tableNode, document);
				}
			}
			else if (value is TomlArray array) {
				XmlElement arrayNode = document.CreateElement("items");
				node.AppendChild(arrayNode);
				foreach (TomlValue i in array.Value) {
					TomlValueToXml("item", i, arrayNode, document);
				}
			} else {
				XmlAttribute attr = document.CreateAttribute("value");
				attr.Value = ExtractString(value);
				node.Attributes.Append(attr);
			}
		}

		private static void TomlValueToXml(String name, TomlValue value, XmlElement node, XmlDocument document) {
			XmlElement newNode;

			if (Lexer.IsValidId(name)) {
				newNode = document.CreateElement(name);
			} else {
				newNode = document.CreateElement("pair");

				XmlAttribute attr = document.CreateAttribute("name");
				attr.Value = name;
				newNode.Attributes.Append(attr);
			}

			if (value is TomlTable table) {
				foreach (System.Collections.Generic.KeyValuePair<String, TomlValue> i in table.pairs) {
					TomlValueToXml(i.Key, i.Value, newNode, document);
				}
			}
			else if (value is TomlArray array) {
				foreach(TomlValue i in array.Value) {
					TomlValueToXml("item", i, newNode, document);
				}
			}
			else {
				XmlAttribute attr = document.CreateAttribute("value");
				attr.Value = ExtractString(value);
				newNode.Attributes.Append(attr);
			}

			node.AppendChild(newNode);
		}

		private static String ExtractString(TomlValue value) {
			if(value is TomlString tomlString) {
				return tomlString.Value;
			}

			return value.ToString();
		}
	}
}
