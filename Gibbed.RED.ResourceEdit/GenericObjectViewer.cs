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
        private readonly Explorer _explorer;

        public GenericObjectViewer(Explorer explorer)
        {
            _explorer = explorer;
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
                    var item = new ListViewItem(name);
                    var dataType = data.GetDataType(name);
                    item.SubItems.Add(dataType);
                    var isArray = dataType.StartsWith("@") && !dataType.StartsWith("@*");
                    item.SubItems.Add(isArray ? "" : data.GetPropertyValueAsString(name, _explorer.StringsFile));
                    listView1.Items.Add(item);

                    if (isArray)
                    {
                        var items = (List<object>) data.GetPropertyValue(name);
                        for(int i=0; i<items.Count; i++)
                        {
                            var childItem = new ListViewItem("[" + i + "]");
                            childItem.SubItems.Add(dataType.Substring(1));
                            childItem.SubItems.Add(items[i].ToString());
                            childItem.IndentCount = 1;
                            listView1.Items.Add(childItem);
                        }
                    }
                }
                if (data.UndecodedData != null)
                {
                    var item = new ListViewItem("<undecoded data>");
                    item.SubItems.Add("byte[]");
                    item.SubItems.Add(data.UndecodedData.Length + " bytes");
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
