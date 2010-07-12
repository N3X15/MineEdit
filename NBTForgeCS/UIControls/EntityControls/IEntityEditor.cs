using System;
using System.Collections.Generic;

using System.Text;

namespace MineEdit
{
    public interface IEntityEditor
    {
        event EventHandler EntityModified;
        Entity Entity { get; set; }
    }
}
