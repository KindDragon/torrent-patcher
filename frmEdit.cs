using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TorrentUtilities;

namespace TorrentPatcher
{
	public delegate void dSructureUpdate(string path, string name, TVal value);

	public partial class frmEdit : Form
	{
		private bool _ParentList;
		private string _path;
		private dSructureUpdate _StructUpdateCallback;
		private DataType _Type;
		private Dictionary<DataType, string> dTable;

		public frmEdit(bool edit, string path, DataType type, string value, bool parentList, dSructureUpdate callback)
		{
			InitializeComponent();
			LoadWordDict();
			_StructUpdateCallback = callback;
			_path = path;
			grbEdit.Text = edit ? "Edit" : "Add";
			btnOK.Text = edit ? "Edit" : "Add";
			txtKeyName.Text = path.Substring(path.LastIndexOf('/') + 1);
			_ParentList = parentList;
			txtKeyName.ReadOnly = _ParentList;
			_Type = type;
			CheckForBlank(this, new EventArgs());
			cboKeyType.SelectedIndex = TypeToIndex(type);
			txtKeyValue.Text = value;
			lblPlace.Text = path.Substring(0, path.LastIndexOf('/'));
			Text = edit ? ("Editing " + path) : ("Adding To " + lblPlace.Text);
			base.ShowDialog();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Dispose(true);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if ((((_Type != DataType.Dictionary) && (_Type != DataType.List)) || ((IndexToType(cboKeyType.SelectedIndex) != DataType.String) && (IndexToType(cboKeyType.SelectedIndex) != DataType.Int))) || (MessageBox.Show("Converting from " + dTable[_Type] + " to " + dTable[IndexToType(cboKeyType.SelectedIndex)] + " will result the lost of all the node childs, are you sure?", Application.ProductName + Application.ProductVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.No))
			{
				TVal val = new TVal(IndexToType(cboKeyType.SelectedIndex), txtKeyValue.Text);
				_StructUpdateCallback(_path, txtKeyName.Text, val);
				Dispose(true);
			}
		}

		private void CheckForBlank(object sender, EventArgs e)
		{
			if (txtKeyName.Text == "")
			{
				btnOK.Enabled = false;
			}
			else
			{
				btnOK.Enabled = true;
			}
		}

		private void cmbKeyType_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (IndexToType(cboKeyType.SelectedIndex))
			{
				case DataType.String:
					txtKeyValue.Enabled = true;
					return;

				case DataType.Int:
					try
					{
						int.Parse(txtKeyValue.Text);
					}
					catch
					{
						txtKeyValue.Text = "";
					}
					txtKeyValue.Enabled = true;
					return;

				case DataType.List:
				case DataType.Dictionary:
					txtKeyValue.Text = "";
					txtKeyValue.Enabled = false;
					return;
			}
		}

		private DataType IndexToType(int index)
		{
			switch (index)
			{
				case 0:
					return DataType.Dictionary;

				case 1:
					return DataType.List;

				case 2:
					return DataType.String;
			}
			return DataType.Int;
		}


		private void LoadWordDict()
		{
			dTable = new Dictionary<DataType, string>();
			dTable.Add(DataType.Dictionary, "dictionary");
			dTable.Add(DataType.List, "list");
			dTable.Add(DataType.String, "string");
			dTable.Add(DataType.Int, "integer");
		}

		private void txtKeyNameKey_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (_ParentList)
			{
				e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
			}
		}

		private void txtKeyValue_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (cboKeyType.SelectedIndex == 3)
			{
				e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
			}
		}

		private int TypeToIndex(DataType Type)
		{
			switch (Type)
			{
				case DataType.String:
					return 2;

				case DataType.List:
					return 1;

				case DataType.Dictionary:
					return 0;
			}
			return 3;
		}
	}
}
