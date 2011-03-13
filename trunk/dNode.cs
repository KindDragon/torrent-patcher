using System;
using System.Windows.Forms;

namespace TorrentPatcher
{
    public static class dNode
    {
        public static string NodePath(TreeNode Node)
        {
            string str = (Node.Name != "") ? Node.Name : Node.Index.ToString();
            if (Node.Parent != null)
            {
                str = NodePath(Node.Parent) + "/" + str;
            }
            return str;
        }

        public static string NodeText(string Name, DataType Type, string Value)
        {
            string str = Name + "(";
            switch (Type)
            {
                case DataType.Int:
                    return (str + "i)=" + Value);

                case DataType.List:
                    return (str + "l)");

                case DataType.Dictionary:
                    return (str + "d)");
            }
            string str2 = str;
            return (str2 + "s)[" + Value.Length.ToString() + "]=" + Value);
        }

        public static DataType NodeType(TreeNode Node)
        {
            switch (Node.Text.Substring(Node.Text.IndexOf('(') + 1, (Node.Text.IndexOf(')', Node.Text.IndexOf('(')) - Node.Text.IndexOf('(')) - 1))
            {
                case "d":
                    return DataType.Dictionary;

                case "l":
                    return DataType.List;

                case "i":
                    return DataType.Int;
            }
            return DataType.String;
        }

        public static string NodeVal(TreeNode Node)
        {
            if (Node == null)
            {
                return "";
            }
            int num = Node.Text.IndexOf('=') + 1;
            if (num < 2)
            {
                return "";
            }
            return Node.Text.Substring(Node.Text.IndexOf('=') + 1);
        }
    }
}

