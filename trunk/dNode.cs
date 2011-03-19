using System.Diagnostics;
using System.Windows.Forms;
using TorrentUtilities;

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
			TVal val = (TVal)Node.Tag;
			Debug.Assert(val != null);
			return val.Type;
		}

		public static string NodeVal(TreeNode Node)
		{
			if (Node == null)
				return "";
			TVal val = (TVal)Node.Tag;
			Debug.Assert(val != null);
			return val.GetValueString();
		}
	}
}
