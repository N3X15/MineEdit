using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft
{
    public class ShittyLighter:ILightRecalculator
    {

        public override void DoBlockLighting(IMapHandler _mh, long X, long Y)
        {
            int csx = (int)_mh.ChunkScale.X;
            int csy = (int)_mh.ChunkScale.Y;
            for (int i = 0; i < 15; i++)
            {
                for (int _x = 0; _x < csx; _x++)
                {
                    for (int _y = 0; _y < csy; _y++)
                    {
                        int x = (int)(X * _mh.ChunkScale.X) + _x;
                        int y = (int)(Y * _mh.ChunkScale.Y) + _y;
                        for (int z = 0; z < _mh.GetHeightAt(x, y); z++)
                        {
                            byte currentBlockLight,currentSkyLight;
                            _mh.GetLightAt(x, y, z, out currentSkyLight, out currentBlockLight);

                            byte currentBlock = _mh.GetBlockAt(x, y, z);

                            Block currentBlockInfo = OpenMinecraft.Blocks.Get(currentBlock);

                            // SUNLIGHT
                            currentSkyLight = (byte)(currentSkyLight - currentBlockInfo.Stop - 1);

                            if (currentBlockInfo.Emit > 0)
                                currentBlockLight = currentBlockInfo.Emit;
                            // Get brightest neighbor
                            if (x < csx - 1 && currentBlockLight < _mh.GetBlockLightAt(x + 1, y, z))
                                currentBlockLight = _mh.GetBlockLightAt(x + 1, y, z);
                            if (y < csy - 1 && currentBlockLight < _mh.GetBlockLightAt(x, y + 1, z))
                                currentBlockLight = _mh.GetBlockLightAt(x, y + 1, z);
                            if (z < _mh.ChunkScale.Z - 1 && currentBlockLight < _mh.GetBlockLightAt(x, y, z + 1))
                                currentBlockLight = _mh.GetBlockLightAt(x, y, z + 1);
                            if (x > 0 && currentBlockLight < _mh.GetBlockLightAt(x - 1, y, z))
                                currentBlockLight = _mh.GetBlockLightAt(x - 1, y, z);
                            if (y > 0 && currentBlockLight < _mh.GetBlockLightAt(x, y - 1, z))
                                currentBlockLight = _mh.GetBlockLightAt(x, y - 1, z);
                            if (z > 0 && currentBlockLight < _mh.GetBlockLightAt(x, y, z - 1))
                                currentBlockLight = _mh.GetBlockLightAt(x, y, z - 1);

                            // Drop 1 level of light + lightstop for current block
                            currentBlockLight = (byte)(currentBlockLight - 1 - currentBlockInfo.Stop);

                            if (currentBlockLight < 0) currentBlockLight = 0;
                            if (currentBlockLight > 15) currentBlockLight = 15;

                            if (currentSkyLight < 0) currentSkyLight = 0;
                            if (currentSkyLight > 15) currentSkyLight = 15;

                            _mh.SetBlockLightAt(x, y, z, currentBlockLight);
                            _mh.SetSkyLightAt(x, y, z, currentSkyLight);
                        }
                    }
                }
            }
        }

        public override void DoSkyLighting(IMapHandler _mh, long X, long Y)
        {
            Chunk c = _mh.GetChunk(X, Y);
            if (c == null) return;
            this.DoSkylighting(c);
        }   
        private void DoSkylighting(Chunk c)
        {
            int csx = (int)c.Size.X;
            int csz = (int)c.Size.Y;
            for (int _x = 0; _x < csx; _x++)
            {
                for (int _z = 0; _z < csz; _z++)
                {
                    int x = (int)(c.Position.X * csx) + _x;
                    int z = (int)(c.Position.Y * csz) + _z;
                    for (int y = 0; y < c.HeightMap[x, z]; y++)
                    {
                        byte currentSkyLight = c.SkyLight[x,y,z];

                        byte currentBlock = c.Blocks[x, y, z];

                        Block currentBlockInfo = OpenMinecraft.Blocks.Get(currentBlock);

                        // SUNLIGHT
                        currentSkyLight = (byte)(currentSkyLight - currentBlockInfo.Stop - 1);

                        if (currentSkyLight < 0) currentSkyLight = 0;
                        if (currentSkyLight > 15) currentSkyLight = 15;

                        c.SkyLight[x, y, z]=currentSkyLight;
                    }
                }
            }
            c.Save();
        }

    }
}
