using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMinecraft;
using System.IO;

namespace MineEdit.MapMonitor
{
    public partial class frmMain : Form
    {
        List<IMapHandler> FileHandlers = new List<IMapHandler>();
        protected String folder;
        protected Image MapImage;
        protected IMapHandler Map;

        Color nullchunk = Color.Transparent;
        Color newchunk = Color.Red;
        Color okchunk = Color.Green;
        Color modifiedchunk = Color.YellowGreen;

        Dictionary<Vector3i, FileSystemWatcher> ChunkWatchers = new Dictionary<Vector3i, FileSystemWatcher>();
        FileSystemWatcher FolderObserver;
        public frmMain(string folder)
        {
            InitializeComponent();
            FolderObserver = new FileSystemWatcher(Path.GetDirectoryName(folder),"c.*.dat");
            FolderObserver.Created +=new FileSystemEventHandler(FolderObserver_Created);
            FileHandlers.Add(new InfdevHandler()); // infdev
        }

        void  FolderObserver_Created(object sender, FileSystemEventArgs e)
        {
 	        string filename = Path.GetFileName(e.FullPath);
        }
        private bool GetFileHandler(string FileName, out IMapHandler mh)
        {
            mh = null;
            foreach (IMapHandler _mh in FileHandlers)
            {
                if (_mh.IsMyFiletype(FileName))
                {
                    mh = _mh;
                    return true;
                }
            }
            return false;
        }
        private void Open(string FileName)
        {


            IMapHandler mh;
            if (!GetFileHandler(FileName, out mh))
            {
                MessageBox.Show(string.Format("Unable to open file {0}: Unrecognised format", Path.GetFileName(FileName)), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            mh.Load(FileName);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void picMapStatus_Resize(object sender, EventArgs e)
        {
           // Render();
        }
    }
}
