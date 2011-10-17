/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

namespace Gibbed.RED.ResourceEdit
{
    partial class ResourceViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceViewer));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.hintLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.saveResourceButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewObjectButton = new System.Windows.Forms.ToolStripButton();
            this.entryTreeView = new System.Windows.Forms.TreeView();
            this.typeImageList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hintLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 218);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(480, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // hintLabel
            // 
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Size = new System.Drawing.Size(30, 17);
            this.hintLabel.Text = "Hint";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveResourceButton,
            this.toolStripSeparator1,
            this.viewObjectButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(480, 25);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Text = "toolStrip1";
            // 
            // saveResourceButton
            // 
            this.saveResourceButton.Enabled = false;
            this.saveResourceButton.Image = ((System.Drawing.Image)(resources.GetObject("saveResourceButton.Image")));
            this.saveResourceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveResourceButton.Name = "saveResourceButton";
            this.saveResourceButton.Size = new System.Drawing.Size(102, 22);
            this.saveResourceButton.Text = "Save Resource";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // viewObjectButton
            // 
            this.viewObjectButton.Image = ((System.Drawing.Image)(resources.GetObject("viewObjectButton.Image")));
            this.viewObjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewObjectButton.Name = "viewObjectButton";
            this.viewObjectButton.Size = new System.Drawing.Size(90, 22);
            this.viewObjectButton.Text = "View Object";
            this.viewObjectButton.Click += new System.EventHandler(this.OnViewObject);
            // 
            // entryTreeView
            // 
            this.entryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryTreeView.ImageIndex = 0;
            this.entryTreeView.ImageList = this.typeImageList;
            this.entryTreeView.Location = new System.Drawing.Point(0, 25);
            this.entryTreeView.Name = "entryTreeView";
            this.entryTreeView.SelectedImageIndex = 0;
            this.entryTreeView.Size = new System.Drawing.Size(480, 193);
            this.entryTreeView.TabIndex = 5;
            // 
            // typeImageList
            // 
            this.typeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("typeImageList.ImageStream")));
            this.typeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.typeImageList.Images.SetKeyName(0, "Unknown");
            this.typeImageList.Images.SetKeyName(1, "RESOURCE");
            this.typeImageList.Images.SetKeyName(2, "CBitmapTexture");
            // 
            // ResourceViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 240);
            this.Controls.Add(this.entryTreeView);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.statusStrip);
            this.Name = "ResourceViewer";
            this.Text = "Resource";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel hintLabel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton saveResourceButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton viewObjectButton;
        private System.Windows.Forms.TreeView entryTreeView;
        private System.Windows.Forms.ImageList typeImageList;
    }
}