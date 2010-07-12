using System;
using System.Collections.Generic;

using System.Text;

namespace MineEdit
{
    public interface ITileEntityEditor
    {
        event EventHandler EntityModified;
        TileEntity TileEntity { get; set; }
    }
}
