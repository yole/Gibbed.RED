using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gibbed.RED.ResourceEdit
{
    public partial class ObjectViewer : Form
    {
        public ObjectViewer()
        {
            InitializeComponent();
        }

        public void LoadResource(FileFormats.IFileObject obj)
        {
            this.propertyGrid1.SelectedObject = obj;
        }
    }
}
