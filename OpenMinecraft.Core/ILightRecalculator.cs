using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft
{
    public abstract class ILightRecalculator
    {
        public virtual void ResetLightingData(ref IMapHandler mh)
        {
            mh.ForEachChunk(delegate(IMapHandler _mh, long X, long Y)
            {
                Chunk c = _mh.GetChunk(X, Y);
                c.UpdateOverview();
                c.SkyLight = new byte[c.Size.X, c.Size.Y, c.Size.Z];
                c.BlockLight = new byte[c.Size.X, c.Size.Y, c.Size.Z];
                _mh.SetChunk(X, Y, c);
            });
        }
        public virtual void BlocklightGlobal(ref IMapHandler mh)
        {
            mh.ForEachChunk(DoBlockLighting);
        }
        public virtual void SkylightGlobal(ref IMapHandler mh)
        {
            mh.ForEachChunk(DoSkyLighting);
        }
        
        public abstract void DoBlockLighting(IMapHandler _mh, long X, long Y);
        public abstract void DoSkyLighting(IMapHandler _mh, long X, long Y);
    }
}
