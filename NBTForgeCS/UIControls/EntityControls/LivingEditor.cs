using System;
using System.Windows.Forms;
using OpenMinecraft.Entities;
namespace MineEdit
{
    public partial class LivingEditor : UserControl,IEntityEditor
    {
        public Entity Entity { 
            get { return (Entity)propEnts.SelectedObject; }
            set
            {
                propEnts.SelectedObject = value;
                propEnts.SelectedObjectsChanged += new EventHandler(propEnts_SelectedObjectsChanged);
            }
        }

        void propEnts_SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        public event EventHandler EntityModified;

        public LivingEditor(Entity e)
        {
            InitializeComponent();
            Entity = e;
        }

        private void cmdKill_Click(object sender, EventArgs e)
        {
            if(propEnts.SelectedObject is LivingEntity)
                (propEnts.SelectedObject as LivingEntity).Health = 0;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            Entity=(LivingEntity)propEnts.SelectedObject;
            if (EntityModified != null)
                EntityModified(this, EventArgs.Empty);
        }

    }
}
