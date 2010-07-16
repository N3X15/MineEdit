using System;
using OpenMinecraft.Entities;

namespace MineEdit
{
    public interface IEntityEditor
    {
        event EventHandler EntityModified;
        Entity Entity { get; set; }
    }
}
