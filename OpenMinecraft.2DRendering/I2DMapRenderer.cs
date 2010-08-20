using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing;

namespace OpenMinecraft._2DRendering
{
    public interface I2DMapRenderer
    {
        bool RenderChunk(Chunk c, out Bitmap img);
    }
}
