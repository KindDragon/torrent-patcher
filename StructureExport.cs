namespace TorrentPatcher
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public class StructureExport
    {
        private StreamWriter _file;
        private int _NodeCount;
        private int c;

        public StructureExport(string Name, TreeNode Node, string Path, int NodeCount)
        {
            _NodeCount = NodeCount;
            c = 0;
            _file = new StreamWriter(File.Create(Path));
            WriteHeading(Name);
            WriteData(Node);
            _file.Close();
        }

        private string NodeName(TreeNode Node)
        {
            if (Node.Name == "")
            {
                return Node.Index.ToString();
            }
            return Node.Name;
        }

        private void WriteData(TreeNode Node)
        {
            c++;
            _file.Write(str.GetBetween(Node.Text, "(", 1));
            switch (dNode.NodeType(Node))
            {
                case DataType.List:
                case DataType.Dictionary:
                    foreach (TreeNode node in Node.Nodes)
                    {
                        _file.WriteLine();
                        WriteTabs(node.Level);
                        _file.Write(NodeName(node) + "\t");
                        WriteData(node);
                    }
                    break;

                default:
                    _file.Write("\t" + dNode.NodeVal(Node));
                    break;
            }
        }

        private void WriteHeading(string Name)
        {
            _file.WriteLine("# " + Name + " Structure.");
            _file.WriteLine("# Created using Torrent Loader " + Application.ProductVersion);
            _file.WriteLine("# " + _NodeCount + " Elements Overall");
            _file.WriteLine();
            _file.WriteLine();
        }

        private void WriteTabs(int n)
        {
            _file.Write("".PadRight(n).Replace(" ", ".\t"));
        }
    }
}

