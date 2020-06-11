using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ringing_Map_Framework {
	public abstract class XMLNodeBase {
		public abstract void GenerateStringRec (StringBuilder builder, string line_start);

		public override string ToString () {
			StringBuilder builder = new StringBuilder ();

			GenerateStringRec (builder, "");

			return builder.ToString ();
		}
	}

	public class XMLTextNode : XMLNodeBase {
		public string text;

		public override void GenerateStringRec (StringBuilder builder, string line_start) {
			builder.Append (line_start);
			builder.Append (text);
		}

		public XMLTextNode (string text) {
			this.text = text.Replace ("&", "&amp;");
		}
	}

	public class XMLNode : XMLNodeBase {
		public string name;
		private Dictionary<string, object> attributes;
		private List<XMLNodeBase> children;

		public void AddAttribute (string key, object value) => attributes.Add (key, value);
		public void AddChild (XMLNodeBase child) => children.Add (child);
		public void AddChildren (IEnumerable<XMLNodeBase> nodes) => children.AddRange (nodes);

		public override void GenerateStringRec (StringBuilder builder, string line_start) {
			// Opening tag
			builder.Append (line_start);
			builder.Append ("<");
			builder.Append (name);

			// Attributes
			if (attributes != null) {
				foreach (KeyValuePair<string, object> attribute in attributes) {
					builder.Append (" ");
					builder.Append (attribute.Key);
					builder.Append ("=\"");
					builder.Append (attribute.Value);
					builder.Append ("\"");
				}
			}

			// Close the tag or make children
			if (children == null || children.Count == 0) {
				builder.Append (" />");
			} else {
				builder.Append (">");

				string new_line_start = line_start + "\t";

				foreach (XMLNodeBase child in children) {
					builder.Append ("\n");

					child.GenerateStringRec (builder, new_line_start);
				}

				// Closing tag
				builder.Append ("\n");
				builder.Append (line_start);
				builder.Append ("</");
				builder.Append (name);
				builder.Append (">");
			}
		}

		public XMLNode (string name, Dictionary<string, object> attributes = null, IEnumerable<XMLNodeBase> children = null) {
			this.name = name;
			this.attributes = attributes ?? new Dictionary<string, object> ();
			this.children = children?.ToList () ?? new List<XMLNodeBase> ();
		}

		public XMLNode (string name, Dictionary<string, object> attributes, string text) {
			this.name = name;
			this.attributes = attributes ?? new Dictionary<string, object> ();
			children = new List<XMLNodeBase> { new XMLTextNode (text) };
		}
	}
}
