using System;
using System.Windows.Forms;

namespace TorrentPatcher
{
	public static class dNode
	{
		public static string NodePath(TreeNode Node)
		{
			string Path = Node.Name != "" ? Node.Name : Node.Index.ToString();
			if (Node.Parent != null)
			{
				Path = NodePath(Node.Parent) + "/" + Path;
			}
			return Path;
		}

		public static DataType NodeType(TreeNode Node)
		{
			string NodeDT = Node.Text.Substring(Node.Text.IndexOf('(') + 1, Node.Text.IndexOf(')', Node.Text.IndexOf('(')) - Node.Text.IndexOf('(') - 1);
			switch (NodeDT)
			{
				case "d":
					return DataType.Dictionary;
				case "l":
					return DataType.List;
				case "i":
					return DataType.Int;
				default:
					return DataType.String;
			}
		}

		public static string NodeVal(TreeNode Node)
		{
			if (Node == null)
				return "";
			int start = Node.Text.IndexOf('=') + 1;
			if (start < 2)
				return "";
			return Node.Text.Substring(Node.Text.IndexOf('=') + 1);
		}

		public static string NodeText(string Name, DataType Type, string Value)
		{
			string s = Name + "(";
			switch (Type)
			{
				case DataType.Dictionary:
					s += "d)";
					break;
				case DataType.List:
					s += "l)";
					break;
				case DataType.Int:
					s += "i)=" + Value;
					break;
				default:
					s += "s)[" + Value.Length.ToString() + "]=" + Value;
					break;
			}
			return s;
		}
	}
}
