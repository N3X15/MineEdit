using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft.Core
{
    public class MineserverLighter:ILightRecalculator
    {
        public override void DoBlockLighting(IMapHandler _mh, long X, long Y)
        {
            throw new NotImplementedException();
        }

        public override void DoSkyLighting(IMapHandler _mh, long X, long Y)
        {
            throw new NotImplementedException();
        }
    }
}
