using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MineEdit
{
    public partial class dlgChunk : Form
    {
        IMapHandler Map;
        Chunk MyChunk;
        public dlgChunk(IMapHandler m,Vector3i pos)
        {
            Map = m;
            InitializeComponent();
        }

        private void dlgChunk_Load(object sender, EventArgs e)
        {

        }
    }
}
