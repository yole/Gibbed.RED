using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gibbed.RED.FileFormats.Game;

namespace Gibbed.RED.ResourceEdit
{
    public partial class GenericObjectViewer : Form
    {
        public GenericObjectViewer()
        {
            InitializeComponent();
        }

        public void LoadResource(GenericObject data)
        {
            Text = data.Type;
            listView1.BeginUpdate();
            try
            {
                foreach (string name in data.PropertyValues)
                {
                    ListViewItem item = new ListViewItem(name);
                    item.SubItems.Add(data.GetDataType(name));
                    item.SubItems.Add(data.GetPropertyValueAsString(name));
                    listView1.Items.Add(item);
                }
            }
            finally
            {
                listView1.EndUpdate();
            }
        }
    }
}
