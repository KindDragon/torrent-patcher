using System.IO;
using System.Windows.Forms;

namespace TorrentPatcher
{
    public class StructureExport
    {
    	StreamWriter _file;
    	int _NodeCount;
    	int c;

    	public StructureExport(string Name,TreeNode Node,string Path,int NodeCount)
    	{
    		_NodeCount=NodeCount;
    		c=0;
    		_file=new StreamWriter(File.Create(Path));
    		WriteHeading(Name);
    		WriteData(Node);
    		_file.Close();
    	}

    	private void WriteData(TreeNode Node)
    	{
    		c++;
    		_file.Write(str.GetBetween(Node.Text,"(",1));
    		switch (dNode.NodeType(Node))
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
            if (Node.Name == "") { return Node.Index.ToString(); }
            else { return Node.Name; }
    	}

    	private void WriteTabs(int n)
    	{
    		_file.Write("".PadRight(n).Replace(" ",".\t"));
    	}
    }
}

