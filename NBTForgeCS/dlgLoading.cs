using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using OpenMinecraft;
namespace MineEdit
{
    public partial class dlgLoading : Form
    {
        IMapHandler Map;
        System.Timers.Timer time = new System.Timers.Timer(100);
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
