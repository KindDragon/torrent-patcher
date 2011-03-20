using System;
using System.IO;
using System.Windows.Forms;
using TorrentUtilities;

namespace TorrentPatcher
{
	public class StructureExport : IDisposable
	{
		StreamWriter _file;
		int _NodeCount;
		int c;

		public StructureExport()
		{
		}

		public void WriteToFile(string Name, TreeNode Node, string Path, int NodeCount)
		{
			_NodeCount = NodeCount;
			c = 0;
			using (_file = new StreamWriter(File.Create(Path)))
			{
				WriteHeading(Name);
				WriteData(Node);
				_file.Close();
			}
		}

		private void WriteData(TreeNode Node)
		{
			c++;
			TVal val = (TVal)Node.Tag;
			_file.Write(val.GetTypeStr());
			switch (val.Type)
			{
			case DataType.Dictionary:
			case DataType.List:
				foreach (TreeNode SubNode in Node.Nodes)
				{
					_file.WriteLine();
					WriteTabs(SubNode.Level);
					_file.Write(NodeName(SubNode)+"\t");
					WriteData(SubNode);
				}
				break;
			default:
				_file.Write("\t"+dNode.NodeVal(Node));
				break;
			}
		}

		private void WriteHeading(string Name)
		{
			_file.WriteLine("# " +Name+ " Structure.");
			_file.WriteLine("# Created using Torrent Loader " + Application.ProductVersion);
			_file.WriteLine("# "+_NodeCount+" Elements Overall");
			_file.WriteLine();
			_file.WriteLine();
		}

		private string NodeName(TreeNode Node)
		{
			if (Node.Name == "") 
				return Node.Index.ToString();
			else
				return Node.Name;
		}

		private void WriteTabs(int n)
		{
			_file.Write("".PadRight(n).Replace(" ",".\t"));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				_file.Close();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}

