using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMinecraft;

namespace MineEdit
{
    public partial class dlgReplace : Form
    {
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
