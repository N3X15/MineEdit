using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenMinecraft;
using System.IO;

namespace MineEdit
{
    public partial class dlgChunk : Form
    {
        IMapHandler Map;
        Chunk MyChunk;
        Vector3i ChunkPos;
        public dlgChunk(IMapHandler m,Vector3i pos)
        {
            InitializeComponent();
            if (m == null)
            {
                MessageBox.Show("m==null");
                Environment.Exit(-1);
            }
            Map = m;
            //mcc.Map = m;
            //mcc.Render();
            //mcc.Refresh();
            ChunkPos = pos;
        }

        void Reload()
        {
            MyChunk = Map.GetChunk(ChunkPos);
            if (MyChunk == null) return;
            txtChunkCoords.Text = string.Format("({0},{1})", ChunkPos.X, ChunkPos.Y);
            txtChunkFile.Text = MyChunk.Filename;
            txtChunkSz.Text = MyChunk.Size.ToString();
            txtCreation.Text = MyChunk.CreationDate.ToShortDateString();
            txtMaxMin.Text = string.Format("{0}m max, {1}m min", MyChunk.MaxHeight, MyChunk.MinHeight);
        }
        private void dlgChunk_Load(object sender, EventArgs e)
        {
            Reload();
        }

        private void cmdReload_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you SURE you wish to delete this chunk?  All data on this chunk (terrain, placed blocks, items, chests, etc) will be PERMANENTLY REMOVED!", "Are you nuts?", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                File.Delete(MyChunk.Filename);
                Close();
            }
        }

        private void cmdRedoLighting_Click(object sender, EventArgs e)
        {

        }
    }
}
