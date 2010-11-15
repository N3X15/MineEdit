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
using System.Timers;
using System.Windows.Forms;
using OpenMinecraft;
namespace MineEdit
{
    public partial class dlgLoading : Form
    {
        IMapHandler Map;
        System.Timers.Timer time = new System.Timers.Timer(100);

        #region Windows Form Designer generated code

        private System.Windows.Forms.ProgressBar pb;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblFile;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pb = new System.Windows.Forms.ProgressBar();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pb
            // 
            this.pb.Location = new System.Drawing.Point(12, 46);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(391, 23);
            this.pb.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(110, 13);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Loading Chunks...";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(29, 30);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(35, 13);
            this.lblFile.TabIndex = 2;
            this.lblFile.Text = "label2";
            // 
            // dlgLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 81);
            this.ControlBox = false;
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pb);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgLoading";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Loading Shit";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.dlgLoading_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public dlgLoading(IMapHandler map)
        {
            InitializeComponent();
            Map = map;
            int xo = (int)Map.PlayerPos.X / 16;
            int yo = (int)Map.PlayerPos.Y / 16;
            pb.Maximum = 400;
            Text = string.Format("Loading 400 Chunks Around Chunk ({0},{1})", xo, yo);
            time.Elapsed += new ElapsedEventHandler(time_Elapsed);
            time.Start();
        }

        void time_Elapsed(object sender, ElapsedEventArgs e)
        {
            time.Stop();
            if (Map.HasMultipleChunks)
            {
                int xo = (int)Map.PlayerPos.X >> 4;
                int yo = (int)Map.PlayerPos.Z >> 4;
                for (int x = -10; x < 10; x++)
                {
                    for (int y = -10; y < 10; y++)
                    {
                        SetText(string.Format("{0}/{1} - Chunk ({2},{3})",(y+10)+((x+10)*20), 400, (xo + x), (yo + y)));
                        SetVal((y + 10) + ((x + 10) * 20));
                        Map.LoadChunk((xo + x), (yo + y));
                        Application.DoEvents();
                    }
                }
            }
            DoClose();
        }

        private void dlgLoading_Load(object sender, EventArgs e)
        {
        }

        delegate void durp();
        delegate void hurf(int val);
        delegate void setstring(string val);
        public void SetMax(int max)
        {
            if (pb.InvokeRequired)
            {
                pb.Invoke(new hurf(SetMax), max);
            }
            else
            {
                pb.Maximum = max;
            }
        }
        public void SetVal(int val)
        {
            if (pb.InvokeRequired)
            {
                pb.Invoke(new hurf(SetVal), val);
            }
            else
            {
                pb.Value = val;
            }
        }
        public void SetText(string c)
        {
            if (lblFile.InvokeRequired)
            {
                lblFile.Invoke(new setstring(SetText), c);
            }
            else
            {
                lblFile.Text = c;
            }
        }
        public void DoClose()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new durp(DoClose));
            }
            else
            {
                Close();
            }
        }
    }
}
