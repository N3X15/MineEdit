using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;

namespace OpenMinecraft.Rendering2D
{
    public interface I2DMapRenderer
    {
        bool RenderChunk(Chunk c, out Bitmap img);
    }
}
