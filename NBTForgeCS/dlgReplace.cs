/**
 * Copyright (c) 2010, Rob "N3X15" Nelson <nexis@7chan.org>
 *  All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without 
 *  modification, are permitted provided that the following conditions are met:
 *
 *    * Redistributions of source code must retain the above copyright notice, 
 *      this list of conditions and the following disclaimer.
 *    * Redistributions in binary form must reproduce the above copyright 
 *      notice, this list of conditions and the following disclaimer in the 
 *      documentation and/or other materials provided with the distribution.
 *    * Neither the name of MineEdit nor the names of its contributors 
 *      may be used to endorse or promote products derived from this software 
 *      without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenMinecraft;

namespace MineEdit
{
    public partial class dlgReplace : Form
    {

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabModes = new System.Windows.Forms.TabControl();
            this.tabBlocks = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numBlkEndZ = new System.Windows.Forms.NumericUpDown();
            this.numAreaZ = new System.Windows.Forms.NumericUpDown();
            this.numBlkEndY = new System.Windows.Forms.NumericUpDown();
            this.numBlkEndX = new System.Windows.Forms.NumericUpDown();
            this.numAreaY = new System.Windows.Forms.NumericUpDown();
            this.numAreaX = new System.Windows.Forms.NumericUpDown();
            this.radArea = new System.Windows.Forms.RadioButton();
            this.numBlkCY = new System.Windows.Forms.NumericUpDown();
            this.numBlkCX = new System.Windows.Forms.NumericUpDown();
            this.radSingleChunk = new System.Windows.Forms.RadioButton();
            this.radAllChunks = new System.Windows.Forms.RadioButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLoadBlockReplacement = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbSaveBlockReplacement = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdClear = new System.Windows.Forms.Button();
            this.cmdRemove = new System.Windows.Forms.Button();
            this.Replacements = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdAdd = new System.Windows.Forms.Button();
            this.blkWith = new MineEdit.BlockSelector();
            this.blkReplace = new MineEdit.BlockSelector();
            this.tabEntities = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.tabModes.SuspendLayout();
            this.tabBlocks.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkEndZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAreaZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkEndY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkEndX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAreaY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAreaX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkCY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkCX)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabModes
            // 
            this.tabModes.Controls.Add(this.tabBlocks);
            this.tabModes.Controls.Add(this.tabEntities);
            this.tabModes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabModes.Location = new System.Drawing.Point(0, 0);
            this.tabModes.Name = "tabModes";
            this.tabModes.SelectedIndex = 0;
            this.tabModes.Size = new System.Drawing.Size(640, 385);
            this.tabModes.TabIndex = 0;
            // 
            // tabBlocks
            // 
            this.tabBlocks.Controls.Add(this.groupBox3);
            this.tabBlocks.Controls.Add(this.toolStrip1);
            this.tabBlocks.Controls.Add(this.groupBox2);
            this.tabBlocks.Controls.Add(this.groupBox1);
            this.tabBlocks.Location = new System.Drawing.Point(4, 22);
            this.tabBlocks.Name = "tabBlocks";
            this.tabBlocks.Padding = new System.Windows.Forms.Padding(3);
            this.tabBlocks.Size = new System.Drawing.Size(632, 359);
            this.tabBlocks.TabIndex = 0;
            this.tabBlocks.Text = "Blocks";
            this.tabBlocks.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numBlkEndZ);
            this.groupBox3.Controls.Add(this.numAreaZ);
            this.groupBox3.Controls.Add(this.numBlkEndY);
            this.groupBox3.Controls.Add(this.numBlkEndX);
            this.groupBox3.Controls.Add(this.numAreaY);
            this.groupBox3.Controls.Add(this.numAreaX);
            this.groupBox3.Controls.Add(this.radArea);
            this.groupBox3.Controls.Add(this.numBlkCY);
            this.groupBox3.Controls.Add(this.numBlkCX);
            this.groupBox3.Controls.Add(this.radSingleChunk);
            this.groupBox3.Controls.Add(this.radAllChunks);
            this.groupBox3.Location = new System.Drawing.Point(8, 149);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(621, 154);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Replacement Options";
            // 
            // numBlkEndZ
            // 
            this.numBlkEndZ.Location = new System.Drawing.Point(237, 94);
            this.numBlkEndZ.Name = "numBlkEndZ";
            this.numBlkEndZ.Size = new System.Drawing.Size(58, 20);
            this.numBlkEndZ.TabIndex = 4;
            // 
            // numAreaZ
            // 
            this.numAreaZ.Location = new System.Drawing.Point(237, 68);
            this.numAreaZ.Name = "numAreaZ";
            this.numAreaZ.Size = new System.Drawing.Size(58, 20);
            this.numAreaZ.TabIndex = 4;
            // 
            // numBlkEndY
            // 
            this.numBlkEndY.Location = new System.Drawing.Point(173, 94);
            this.numBlkEndY.Name = "numBlkEndY";
            this.numBlkEndY.Size = new System.Drawing.Size(58, 20);
            this.numBlkEndY.TabIndex = 4;
            // 
            // numBlkEndX
            // 
            this.numBlkEndX.Location = new System.Drawing.Point(109, 94);
            this.numBlkEndX.Name = "numBlkEndX";
            this.numBlkEndX.Size = new System.Drawing.Size(58, 20);
            this.numBlkEndX.TabIndex = 4;
            // 
            // numAreaY
            // 
            this.numAreaY.Location = new System.Drawing.Point(173, 68);
            this.numAreaY.Name = "numAreaY";
            this.numAreaY.Size = new System.Drawing.Size(58, 20);
            this.numAreaY.TabIndex = 4;
            // 
            // numAreaX
            // 
            this.numAreaX.Location = new System.Drawing.Point(109, 68);
            this.numAreaX.Name = "numAreaX";
            this.numAreaX.Size = new System.Drawing.Size(58, 20);
            this.numAreaX.TabIndex = 4;
            // 
            // radArea
            // 
            this.radArea.AutoSize = true;
            this.radArea.Location = new System.Drawing.Point(21, 65);
            this.radArea.Name = "radArea";
            this.radArea.Size = new System.Drawing.Size(50, 17);
            this.radArea.TabIndex = 3;
            this.radArea.Text = "Area:";
            this.radArea.UseVisualStyleBackColor = true;
            // 
            // numBlkCY
            // 
            this.numBlkCY.Location = new System.Drawing.Point(173, 42);
            this.numBlkCY.Name = "numBlkCY";
            this.numBlkCY.Size = new System.Drawing.Size(58, 20);
            this.numBlkCY.TabIndex = 2;
            // 
            // numBlkCX
            // 
            this.numBlkCX.Location = new System.Drawing.Point(109, 42);
            this.numBlkCX.Name = "numBlkCX";
            this.numBlkCX.Size = new System.Drawing.Size(58, 20);
            this.numBlkCX.TabIndex = 2;
            // 
            // radSingleChunk
            // 
            this.radSingleChunk.AutoSize = true;
            this.radSingleChunk.Location = new System.Drawing.Point(21, 42);
            this.radSingleChunk.Name = "radSingleChunk";
            this.radSingleChunk.Size = new System.Drawing.Size(82, 17);
            this.radSingleChunk.TabIndex = 1;
            this.radSingleChunk.Text = "One Chunk:";
            this.radSingleChunk.UseVisualStyleBackColor = true;
            // 
            // radAllChunks
            // 
            this.radAllChunks.AutoSize = true;
            this.radAllChunks.Checked = true;
            this.radAllChunks.Location = new System.Drawing.Point(21, 19);
            this.radAllChunks.Name = "radAllChunks";
            this.radAllChunks.Size = new System.Drawing.Size(75, 17);
            this.radAllChunks.TabIndex = 0;
            this.radAllChunks.TabStop = true;
            this.radAllChunks.Text = "All Chunks";
            this.radAllChunks.UseVisualStyleBackColor = true;
            this.radAllChunks.CheckedChanged += new System.EventHandler(this.radAllChunks_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator1,
            this.tsbLoadBlockReplacement,
            this.tsbSaveBlockReplacement});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(626, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(62, 22);
            this.toolStripLabel1.Text = "Templates";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbLoadBlockReplacement
            // 
            this.tsbLoadBlockReplacement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLoadBlockReplacement.Image = global::MineEdit.Properties.Resources.document_open;
            this.tsbLoadBlockReplacement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLoadBlockReplacement.Name = "tsbLoadBlockReplacement";
            this.tsbLoadBlockReplacement.Size = new System.Drawing.Size(32, 22);
            this.tsbLoadBlockReplacement.Text = "toolStripButton1";
            // 
            // tsbSaveBlockReplacement
            // 
            this.tsbSaveBlockReplacement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveBlockReplacement.Image = global::MineEdit.Properties.Resources.document_save;
            this.tsbSaveBlockReplacement.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsbSaveBlockReplacement.Name = "tsbSaveBlockReplacement";
            this.tsbSaveBlockReplacement.Size = new System.Drawing.Size(23, 22);
            this.tsbSaveBlockReplacement.Text = "toolStripButton1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdClear);
            this.groupBox2.Controls.Add(this.cmdRemove);
            this.groupBox2.Controls.Add(this.Replacements);
            this.groupBox2.Location = new System.Drawing.Point(337, 31);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(292, 112);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Replacements";
            // 
            // cmdClear
            // 
            this.cmdClear.Location = new System.Drawing.Point(210, 48);
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(75, 23);
            this.cmdClear.TabIndex = 3;
            this.cmdClear.Text = "Clear";
            this.cmdClear.UseVisualStyleBackColor = true;
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // cmdRemove
            // 
            this.cmdRemove.Location = new System.Drawing.Point(210, 19);
            this.cmdRemove.Name = "cmdRemove";
            this.cmdRemove.Size = new System.Drawing.Size(75, 23);
            this.cmdRemove.TabIndex = 5;
            this.cmdRemove.Text = "Remove";
            this.cmdRemove.UseVisualStyleBackColor = true;
            this.cmdRemove.Click += new System.EventHandler(this.cmdRemove_Click);
            // 
            // Replacements
            // 
            this.Replacements.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.Replacements.Location = new System.Drawing.Point(6, 19);
            this.Replacements.Name = "Replacements";
            this.Replacements.Size = new System.Drawing.Size(198, 82);
            this.Replacements.TabIndex = 2;
            this.Replacements.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.Replacements_DrawItem);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmdAdd);
            this.groupBox1.Controls.Add(this.blkWith);
            this.groupBox1.Controls.Add(this.blkReplace);
            this.groupBox1.Location = new System.Drawing.Point(8, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(323, 112);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Replace Blocks";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "With:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Replace:";
            // 
            // cmdAdd
            // 
            this.cmdAdd.Location = new System.Drawing.Point(235, 72);
            this.cmdAdd.Name = "cmdAdd";
            this.cmdAdd.Size = new System.Drawing.Size(75, 23);
            this.cmdAdd.TabIndex = 4;
            this.cmdAdd.Text = "Add";
            this.cmdAdd.UseVisualStyleBackColor = true;
            this.cmdAdd.Click += new System.EventHandler(this.cmdAdd_Click);
            // 
            // blkWith
            // 
            this.blkWith.BlocksOnly = false;
            this.blkWith.DisplayMember = "Name";
            this.blkWith.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.blkWith.FormattingEnabled = true;
            this.blkWith.Location = new System.Drawing.Point(74, 45);
            this.blkWith.Name = "blkWith";
            this.blkWith.Size = new System.Drawing.Size(236, 21);
            this.blkWith.TabIndex = 0;
            this.blkWith.ValueMember = "ID";
            // 
            // blkReplace
            // 
            this.blkReplace.BlocksOnly = false;
            this.blkReplace.DisplayMember = "Name";
            this.blkReplace.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.blkReplace.FormattingEnabled = true;
            this.blkReplace.Location = new System.Drawing.Point(74, 18);
            this.blkReplace.Name = "blkReplace";
            this.blkReplace.Size = new System.Drawing.Size(236, 21);
            this.blkReplace.TabIndex = 0;
            this.blkReplace.ValueMember = "ID";
            // 
            // tabEntities
            // 
            this.tabEntities.Location = new System.Drawing.Point(4, 22);
            this.tabEntities.Name = "tabEntities";
            this.tabEntities.Padding = new System.Windows.Forms.Padding(3);
            this.tabEntities.Size = new System.Drawing.Size(632, 359);
            this.tabEntities.TabIndex = 1;
            this.tabEntities.Text = "Entities";
            this.tabEntities.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdOK);
            this.panel1.Controls.Add(this.cmdCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 331);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(640, 54);
            this.panel1.TabIndex = 1;
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(477, 19);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(558, 19);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 0;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // dlgReplace
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(640, 385);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabModes);
            this.Name = "dlgReplace";
            this.Text = "Replace Stuff";
            this.tabModes.ResumeLayout(false);
            this.tabBlocks.ResumeLayout(false);
            this.tabBlocks.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkEndZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAreaZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkEndY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkEndX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAreaY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAreaX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkCY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBlkCX)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabModes;
        private System.Windows.Forms.TabPage tabBlocks;
        private System.Windows.Forms.TabPage tabEntities;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private BlockSelector blkWith;
        private BlockSelector blkReplace;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdClear;
        private System.Windows.Forms.Button cmdRemove;
        private System.Windows.Forms.Button cmdAdd;
        private System.Windows.Forms.ListBox Replacements;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numBlkEndZ;
        private System.Windows.Forms.NumericUpDown numAreaZ;
        private System.Windows.Forms.NumericUpDown numBlkEndY;
        private System.Windows.Forms.NumericUpDown numBlkEndX;
        private System.Windows.Forms.NumericUpDown numAreaY;
        private System.Windows.Forms.NumericUpDown numAreaX;
        private System.Windows.Forms.RadioButton radArea;
        private System.Windows.Forms.NumericUpDown numBlkCY;
        private System.Windows.Forms.NumericUpDown numBlkCX;
        private System.Windows.Forms.RadioButton radSingleChunk;
        private System.Windows.Forms.RadioButton radAllChunks;
        private System.Windows.Forms.ToolStripButton tsbSaveBlockReplacement;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private ToolStripSplitButton tsbLoadBlockReplacement;
        #endregion

        IMapHandler _Map;
        public dlgReplace(IMapHandler mh)
        {
            _Map=mh;
            InitializeComponent();
        }

        private void Replacements_DrawItem(object sender, DrawItemEventArgs e)
        {

            Graphics g = e.Graphics;
            Rectangle area = e.Bounds;
            Rectangle iconArea = area;
            iconArea.Width = 16;
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Block enta = Blocks.Get((short)((KeyValuePair<byte, byte>)Replacements.Items[e.Index]).Key);
                Block entb = Blocks.Get((short)((KeyValuePair<byte, byte>)Replacements.Items[e.Index]).Value);

                // Draw block icon A
                g.DrawImage(enta.Image, iconArea);

                // Block Name A
                SizeF idaSz = g.MeasureString(enta.ToString(), this.Font);
                Rectangle idAreaA = area;
                idAreaA.X = iconArea.Right + 3;
                idAreaA.Width = (int)idaSz.Width + 1;
                g.DrawString(enta.ToString(), this.Font, new SolidBrush(Color.FromArgb(128, e.ForeColor)), idAreaA);

                // Arrow
                SizeF arrowsz = g.MeasureString("->", this.Font);
                Rectangle ctxt = area;
                ctxt.X = idAreaA.Right + 3;
                ctxt.Width = (int)arrowsz.Width + 1;
                g.DrawString("->", this.Font, new SolidBrush(e.ForeColor), ctxt);


                // Draw block icon B
                iconArea.X = ctxt.Right + 3;
                g.DrawImage(entb.Image, iconArea);

                // Block Name B
                SizeF idbSz = g.MeasureString(entb.ToString(), this.Font);
                Rectangle idAreaB = area;
                idAreaB.X = iconArea.Right + 3;
                idAreaB.Width = (int)idbSz.Width + 1;
                g.DrawString(entb.ToString(), this.Font, new SolidBrush(Color.FromArgb(128, e.ForeColor)), idAreaB);
            }
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            byte a = (byte)(blkReplace.SelectedItem as Block).ID;
            byte b = (byte)(blkWith.SelectedItem as Block).ID;
            Replacements.Items.Add(new KeyValuePair<byte, byte>(a, b));
        }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            if (Replacements.SelectedItem != null)
            {
                Replacements.Items.Remove(Replacements.SelectedItem);
            }
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            Replacements.Items.Clear();
        }

        private void radAllChunks_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void DoReplaceBlocks(long X, long Y)
        {
        }
        private void ReplaceBlocks()
        {
            this.Enabled = false;
            string q = "Are you sure you want to do the following replacements:\n\n\t{0}\n\nTHIS WILL TAKE A VERY LONG TIME!";
            List<string> reps = new List<string>();
            foreach (KeyValuePair<byte, byte> rep in Replacements.Items)
            {
                reps.Add(string.Format("{0} to {1}", Blocks.Get((short)rep.Key).Name, Blocks.Get((short)rep.Value).Name));
            }
            DialogResult dr = MessageBox.Show(string.Format(q, string.Join("\n\t", reps.ToArray())), "Clear snow?", MessageBoxButtons.YesNo);

            long nchunks = (_Map.MapMax.X - _Map.MapMin.X) * (_Map.MapMax.Y - _Map.MapMin.Y);
            int NumChunks = 0;
            int ProcessedChunks = 0;
            if (dr == DialogResult.Yes)
            {
                dlgLongTask dlt = new dlgLongTask();
                dlt.Title = "Replacing blocks";
                dlt.Subtitle = "This will take a long time.  Take a break.";
                dlt.VocabSubtask = "subtask";
                dlt.VocabSubtasks = "subtasks";
                dlt.VocabTask = "chunk";
                dlt.VocabTasks = "chunks";
                dlt.CurrentSubtask = "";
                dlt.CurrentTask = "Counting chunks...";
                dlt.Start(delegate() {
                    _Map.ForEachChunk(delegate(long X, long Y)
                    {
                        if (dlt.STOP) return;
                        ++dlt.TasksTotal;
                        dlt.CurrentSubtask="Counted "+dlt.TasksTotal.ToString()+" chunks so far...";
                    });

                    Dictionary<byte, byte> durr = new Dictionary<byte, byte>();
                    foreach (KeyValuePair<byte, byte> derp in Replacements.Items)
                    {
                        durr.Add(derp.Key, derp.Value);
                    }
                    _Map.ForEachChunk(delegate(long X, long Y) {
                        if (dlt.STOP) return;
                        Chunk c = _Map.GetChunk(X, Y);
                        dlt.SubtasksTotal = (int)c.Size.Z;
                        for (int z = 0; z < c.Size.Z; z++)
                        {
                            dlt.SubtasksComplete = z+1;
                            for (int y = 0; y < c.Size.Y; y++)
                            {
                                for (int x = 0; x < c.Size.X; x++)
                                {
                                    if (durr.ContainsKey(c.Blocks[x, y, z]))
                                    {
                                        c.Blocks[x, y, z] = durr[c.Blocks[x, y, z]];
                                    }
                                }
                            }
                        }
                        c.Save();
                    });
                });
                if (dlt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MessageBox.Show("Done, lighting was removed so minecraft may freeze while it recalculates lighting.", "Done.");
                }
            }
            this.Enabled = true;
        }
        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (tabModes.SelectedTab == tabBlocks)
            {
                ReplaceBlocks();
                Close();
            }
        }
    }
}
