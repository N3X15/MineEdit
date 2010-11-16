/*
 * Created by SharpDevelop.
 * User: Rob
 * Date: 8/20/2010
 * Time: 12:05 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using OpenTK.Graphics.OpenGL;
using OpenMinecraft;
namespace OpenMinecraft.Rendering3D
{
	/// <summary>
	/// Description of ChunkRenderer.
	/// </summary>
	public class ChunkRenderer:IChunkRenderer
	{
		public Chunk Chunk {get;set;}
		public IMapHandler World {get;set;}
		public ChunkRenderer(Chunk c, IMapHandler _mh)
		{
			Chunk=c;
			World=_mh;
            Cached = false;
		}

        public bool Cached { get; set; }
        int displayList = -1;

        public void RenderGround(bool FromCache)
        {
            // If chunk is null or cache is empty and we're being requested to draw from cache
            if (Chunk == null || (!Cached && FromCache)) return;
            int X = (int)this.Chunk.Position.X;
            int Z = (int)this.Chunk.Position.Y;
            int Width = (int)this.Chunk.Size.X;
            int Height = (int)this.Chunk.Size.Y;
            int Depth = (int)this.Chunk.Size.Z;
            bool rightLoaded = (World.GetChunk(X + 1, Z) != null);
            bool leftLoaded = (World.GetChunk(X - 1, Z) != null);
            bool frontLoaded = (World.GetChunk(X, Z + 1) != null);
            bool backLoaded = (World.GetChunk(X, Z - 1) != null);
            GL.PushMatrix();
            GL.Translate(X * Width, 0.0, Z * Height);
            if (Cached)
                GL.CallList(displayList);
            else
            {
                Console.WriteLine("[Rendering chunk " + Chunk.Position.ToString() + " Ground]");
                displayList = GL.GenLists(2);
                GL.NewList(displayList, ListMode.CompileAndExecute);
                GL.Begin(BeginMode.Quads);
                for (int x = 0; x < Width; ++x)
                {
                    for (int y = 0; y < Height; ++y)
                    {
                        for (int z = 0; z < Depth; ++z)
                        {
                            Block type = Chunk.GetBlockType(x, y, z);
                            if (type != null && type.DrawMode != DrawMode.Water)
                            {
                                Block top = Chunk.GetBlockType(x, y, z + 1);
                                Block bottom = Chunk.GetBlockType(x, y, z - 1);
                                Block right = Chunk.GetBlockType(x + 1, y, z);
                                Block left = Chunk.GetBlockType(x - 1, y, z);
                                Block front = Chunk.GetBlockType(x, y + 1, z);
                                Block back = Chunk.GetBlockType(x, y - 1, z);
                                if (type.SideVisible(top))
                                {
                                    GL.Normal3(0.0, 1.0, 0.0);
                                    GL.TexCoord2(type.Top.X1, type.Top.Y1); GL.Vertex3(x + 0.0, y + 1.0, z + 0.0);
                                    GL.TexCoord2(type.Top.X2, type.Top.Y1); GL.Vertex3(x + 0.0, y + 1.0, z + 1.0);
                                    GL.TexCoord2(type.Top.X2, type.Top.Y2); GL.Vertex3(x + 1.0, y + 1.0, z + 1.0);
                                    GL.TexCoord2(type.Top.X1, type.Top.Y2); GL.Vertex3(x + 1.0, y + 1.0, z + 0.0);
                                }
                                if (y != 0 && type.SideVisible(bottom))
                                {
                                    GL.Normal3(0.0, 1.0, 0.0);
                                    GL.TexCoord2(type.Bottom.X1, type.Bottom.Y1); GL.Vertex3(x + 0.0, y + 0.0, z + 0.0);
                                    GL.TexCoord2(type.Bottom.X2, type.Bottom.Y1); GL.Vertex3(x + 1.0, y + 0.0, z + 0.0);
                                    GL.TexCoord2(type.Bottom.X2, type.Bottom.Y2); GL.Vertex3(x + 1.0, y + 0.0, z + 1.0);
                                    GL.TexCoord2(type.Bottom.X1, type.Bottom.Y2); GL.Vertex3(x + 0.0, y + 0.0, z + 1.0);
                                }
                                if ((x != Width - 1 || rightLoaded) && type.SideVisible(right))
                                {
                                    GL.Normal3(1.0, 0.0, 0.0);
                                    GL.TexCoord2(type.Sides.X1, type.Sides.Y1); GL.Vertex3(x + 1.0, y + 1.0, z + 0.0);
                                    GL.TexCoord2(type.Sides.X2, type.Sides.Y1); GL.Vertex3(x + 1.0, y + 1.0, z + 1.0);
                                    GL.TexCoord2(type.Sides.X2, type.Sides.Y2); GL.Vertex3(x + 1.0, y + 0.0, z + 1.0);
                                    GL.TexCoord2(type.Sides.X1, type.Sides.Y2); GL.Vertex3(x + 1.0, y + 0.0, z + 0.0);
                                }
                                if ((x != 0 || leftLoaded) && type.SideVisible(left))
                                {
                                    GL.Normal3(1.0, 0.0, 0.0);
                                    GL.TexCoord2(type.Sides.X1, type.Sides.Y1); GL.Vertex3(x + 0.0, y + 1.0, z + 1.0);
                                    GL.TexCoord2(type.Sides.X2, type.Sides.Y1); GL.Vertex3(x + 0.0, y + 1.0, z + 0.0);
                                    GL.TexCoord2(type.Sides.X2, type.Sides.Y2); GL.Vertex3(x + 0.0, y + 0.0, z + 0.0);
                                    GL.TexCoord2(type.Sides.X1, type.Sides.Y2); GL.Vertex3(x + 0.0, y + 0.0, z + 1.0);
                                }
                                if ((z != Height - 1 || frontLoaded) && type.SideVisible(front))
                                {
                                    GL.Normal3(0.0, 0.0, 1.0);
                                    GL.TexCoord2(type.Sides.X1, type.Sides.Y1); GL.Vertex3(x + 1.0, y + 1.0, z + 1.0);
                                    GL.TexCoord2(type.Sides.X2, type.Sides.Y1); GL.Vertex3(x + 0.0, y + 1.0, z + 1.0);
                                    GL.TexCoord2(type.Sides.X2, type.Sides.Y2); GL.Vertex3(x + 0.0, y + 0.0, z + 1.0);
                                    GL.TexCoord2(type.Sides.X1, type.Sides.Y2); GL.Vertex3(x + 1.0, y + 0.0, z + 1.0);
                                }
                                if ((z != 0 || backLoaded) && type.SideVisible(back))
                                {
                                    GL.Normal3(0.0, 0.0, 1.0);
                                    GL.TexCoord2(type.Sides.X1, type.Sides.Y1); GL.Vertex3(x + 0.0, y + 1.0, z + 0.0);
                                    GL.TexCoord2(type.Sides.X2, type.Sides.Y1); GL.Vertex3(x + 1.0, y + 1.0, z + 0.0);
                                    GL.TexCoord2(type.Sides.X2, type.Sides.Y2); GL.Vertex3(x + 1.0, y + 0.0, z + 0.0);
                                    GL.TexCoord2(type.Sides.X1, type.Sides.Y2); GL.Vertex3(x + 0.0, y + 0.0, z + 0.0);
                                }
                            }
                        }
                    }
                }
                GL.End();
                GL.EndList();
            }
            GL.PopMatrix();
        }

        public void RenderWater(bool FromCache)
        {
            // If chunk is null or cache is empty and we're being requested to draw from cache
            if (Chunk == null || (!Cached && FromCache)) return;
            if (Chunk == null) return;
			int X = (int)this.Chunk.Position.X;
			int Z = (int)this.Chunk.Position.Y;
			int Width = (int)this.Chunk.Size.X;
			int Height = (int)this.Chunk.Size.Y;
			int Depth = (int)this.Chunk.Size.Z;
			bool rightLoaded = (World.GetChunk(X+1, Z) != null);
			bool leftLoaded = (World.GetChunk(X-1, Z) != null);
			bool frontLoaded = (World.GetChunk(X, Z+1) != null);
			bool backLoaded = (World.GetChunk(X, Z-1) != null);
			GL.PushMatrix();
			GL.Translate(X * Width, 0.0, Z * Height);
			if (Cached) 
                GL.CallList(displayList+1);
			else
            {
                Console.WriteLine("[Rendering chunk " + Chunk.Position.ToString() + " Water]");
				GL.NewList(displayList+1, ListMode.CompileAndExecute);
				GL.Begin(BeginMode.Quads);
				for (int x = 0; x < Chunk.Size.X; ++x) 
				{
					for (int y = 0; y < Chunk.Size.Y; ++y) 
					{
						for (int z = 0; z < Chunk.Size.Z; ++z) 
						{
							Block type = Chunk.GetBlockType(x, y, z);
							if (type != null && type.DrawMode == DrawMode.Water) 
							{
								Block top 		= Chunk.GetBlockType(x, y+1, z);
								Block bottom	= Chunk.GetBlockType(x, y-1, z);
								Block right		= Chunk.GetBlockType(x+1, y, z);
								Block left		= Chunk.GetBlockType(x-1, y, z);
								Block front		= Chunk.GetBlockType(x, y, z+1);
								Block back		= Chunk.GetBlockType(x, y, z-1);
								if (type.SideVisible(top)) 
								{
									GL.Normal3(0.0, 1.0, 0.0);
									GL.TexCoord2(type.Top.X1, type.Top.Y1); GL.Vertex3(x+0.0, y+1.0, z+0.0);
									GL.TexCoord2(type.Top.X2, type.Top.Y1); GL.Vertex3(x+0.0, y+1.0, z+1.0);
									GL.TexCoord2(type.Top.X2, type.Top.Y2); GL.Vertex3(x+1.0, y+1.0, z+1.0);
									GL.TexCoord2(type.Top.X1, type.Top.Y2); GL.Vertex3(x+1.0, y+1.0, z+0.0);
								}
								if (type.SideVisible(bottom)) 
								{
									GL.Normal3(0.0, 1.0, 0.0);
									GL.TexCoord2(type.Bottom.X1, type.Bottom.Y1); GL.Vertex3(x+0.0, y+0.0, z+0.0);
									GL.TexCoord2(type.Bottom.X2, type.Bottom.Y1); GL.Vertex3(x+1.0, y+0.0, z+0.0);
									GL.TexCoord2(type.Bottom.X2, type.Bottom.Y2); GL.Vertex3(x+1.0, y+0.0, z+1.0);
									GL.TexCoord2(type.Bottom.X1, type.Bottom.Y2); GL.Vertex3(x+0.0, y+0.0, z+1.0);
								}
								if ((x != Width-1 || rightLoaded) && type.SideVisible(right)) 
								{
									GL.Normal3(1.0, 0.0, 0.0);
									GL.TexCoord2(type.Sides.X1, type.Sides.Y1); GL.Vertex3(x+1.0, y+1.0, z+0.0);
									GL.TexCoord2(type.Sides.X2, type.Sides.Y1); GL.Vertex3(x+1.0, y+1.0, z+1.0);
									GL.TexCoord2(type.Sides.X2, type.Sides.Y2); GL.Vertex3(x+1.0, y+0.0, z+1.0);
									GL.TexCoord2(type.Sides.X1, type.Sides.Y2); GL.Vertex3(x+1.0, y+0.0, z+0.0);
								}
								if ((x != 0 || leftLoaded) && type.SideVisible(left)) 
								{
									GL.Normal3(1.0, 0.0, 0.0);
									GL.TexCoord2(type.Sides.X1, type.Sides.Y1); GL.Vertex3(x+0.0, y+1.0, z+1.0);
									GL.TexCoord2(type.Sides.X2, type.Sides.Y1); GL.Vertex3(x+0.0, y+1.0, z+0.0);
									GL.TexCoord2(type.Sides.X2, type.Sides.Y2); GL.Vertex3(x+0.0, y+0.0, z+0.0);
									GL.TexCoord2(type.Sides.X1, type.Sides.Y2); GL.Vertex3(x+0.0, y+0.0, z+1.0);
								}
								if ((z != Height-1 || frontLoaded) && type.SideVisible(front)) 
								{
									GL.Normal3(0.0, 0.0, 1.0);
									GL.TexCoord2(type.Sides.X1, type.Sides.Y1); GL.Vertex3(x+1.0, y+1.0, z+1.0);
									GL.TexCoord2(type.Sides.X2, type.Sides.Y1); GL.Vertex3(x+0.0, y+1.0, z+1.0);
									GL.TexCoord2(type.Sides.X2, type.Sides.Y2); GL.Vertex3(x+0.0, y+0.0, z+1.0);
									GL.TexCoord2(type.Sides.X1, type.Sides.Y2); GL.Vertex3(x+1.0, y+0.0, z+1.0);
								}
								if ((z != 0 || backLoaded) && type.SideVisible(back)) 
								{
									GL.Normal3(0.0, 0.0, 1.0);
									GL.TexCoord2(type.Sides.X1, type.Sides.Y1); GL.Vertex3(x+0.0, y+1.0, z+0.0);
									GL.TexCoord2(type.Sides.X2, type.Sides.Y1); GL.Vertex3(x+1.0, y+1.0, z+0.0);
									GL.TexCoord2(type.Sides.X2, type.Sides.Y2); GL.Vertex3(x+1.0, y+0.0, z+0.0);
									GL.TexCoord2(type.Sides.X1, type.Sides.Y2); GL.Vertex3(x+0.0, y+0.0, z+0.0);
								}
							}
						}
					}
				}
				GL.End();
				GL.EndList();
				Cached = true;
			}
			GL.PopMatrix();
		}

        public void SetDirty() { Cached = false; }
	}
}
