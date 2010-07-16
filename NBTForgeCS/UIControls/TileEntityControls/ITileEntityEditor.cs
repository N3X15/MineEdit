using System;

using OpenMinecraft.TileEntities;

namespace MineEdit
{
    public interface ITileEntityEditor
    {
        event EventHandler EntityModified;
        TileEntity TileEntity { get; set; }
    }
}
