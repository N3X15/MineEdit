/*
 * Created by SharpDevelop.
 * User: Rob
 * Date: 8/20/2010
 * Time: 1:28 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenMinecraft.Rendering3D
{
	/// <summary>
	/// Description of Viewport.
	/// </summary>
	public class Viewport:UserControl
	{
        Queue<Chunk> ToBeRendered = new Queue<Chunk>();
		GLControl gl;
		Camera camera;
		public IMapHandler World {get;set;}
		Texture terrain;
		public int RenderRange=3;
		bool Loaded = false;
		public Viewport()
		{
			gl = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 4));
			Controls.Add(gl);
			gl.Dock = DockStyle.Fill;
            gl.MouseDown += new MouseEventHandler(gl_MouseDown);
			Console.WriteLine("Creating viewport.");
			camera = new Camera(gl);
			camera.Location = new OpenTK.Vector3d(0d, -128d, 0d);
			camera.Rotation = new Vector2(0, 70);
			camera.MoveEnabled = true;
			camera.MouseEnabled = false;
			
		}

        void gl_MouseDown(object sender, MouseEventArgs e)
        {
            camera.MouseEnabled = true;

        }

		void Application_Idle(object sender, EventArgs e)
		{
			Render();
		}
		
		void Render()
		{
			if(!Loaded) return;

            // Update is never called;  Mouse/Keyboard events trigger camera updates.

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.PushMatrix();
			camera.Render();
			GL.Light(LightName.Light0, LightParameter.Position, new Vector4(0.75f, 1.0f, 0.6f, 0.0f));
			
			if(World!=null)
			{
				double RenderOriginX = camera.Location.X / World.ChunkScale.X;
				double RenderOriginY = camera.Location.Y / World.ChunkScale.Y;
				GL.PushMatrix();
				for(int x = (int)RenderOriginX-RenderRange;x<RenderOriginX+RenderRange;++x)
				{
					for(int y = (int)RenderOriginY-RenderRange;y<RenderOriginY+RenderRange;++y)
					{
						Chunk chunk = World.GetChunk(x,y);
                        if (chunk == null) continue;
						if(chunk.Renderer == null)
						{
							chunk.Renderer= new ChunkRenderer(chunk,World);
                        }
                        if (!chunk.Renderer.Cached && !ToBeRendered.Contains(chunk))
                        {
                            if (Math.Sqrt((Math.Pow(RenderOriginX + x, 2) + Math.Pow(RenderOriginY + y, 2))) <= RenderRange + 0.5f)
                            {
                                ToBeRendered.Enqueue(chunk);
                                World.SetChunk(chunk);
                            }
                        }
					}
				}
				World.ForEachCachedChunk(delegate(long x, long y, Chunk c){
                    if (c.Renderer == null) 
                    {
                    	//Console.WriteLine("Chunk ({0},{1}) doesn't have a renderer!", x, y);
						c.Renderer= new ChunkRenderer(c,World);
						World.SetChunk(c);
                    }
					if (Math.Sqrt((Math.Pow(RenderOriginX+x, 2) + Math.Pow(RenderOriginY+y, 2))) > RenderRange + 0.5f) 
					{
						World.CullChunk(x,y); // Out of rendering distance, nuke it.
						return;
					}
                    c.Renderer.RenderGround(ToBeRendered.Count>0 && ToBeRendered.Peek() != c);
				});
				GL.DepthMask(false);
				GL.Enable(EnableCap.Blend);
				GL.Translate(0.0, -0.1, 0.0);
                World.ForEachCachedChunk(delegate(long x, long y, Chunk c)
                {
                    if (c.Renderer == null)
                    {
                        return; // We'll pick it up next time.
                    }
					if (Math.Sqrt((Math.Pow(RenderOriginX+x, 2) + Math.Pow(RenderOriginY+y, 2))) > RenderRange + 0.5f) 
					{
						World.CullChunk(x,y); // Out of rendering distance, nuke it.
						return;
					}
                    c.Renderer.RenderWater((ToBeRendered.Count > 0 && ToBeRendered.Peek() != c));
				});
                if(ToBeRendered.Count>0)
                    ToBeRendered.Dequeue();
				GL.DepthMask(true);
				GL.Disable(EnableCap.Blend);
				GL.PopMatrix();
			}
			
			GL.PopMatrix();
			try
			{
				gl.SwapBuffers();
			} catch(Exception)
			{
			}
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			GL.ClearColor(Color.SkyBlue);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.Texture2D);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.Enable(EnableCap.AlphaTest);
			GL.AlphaFunc(AlphaFunction.Greater, 0.4f);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);
			GL.Light(LightName.Light0, LightParameter.Ambient, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
			
			terrain = Texture.Load("terrain.png");
			Application.Idle += new EventHandler(Application_Idle);
            this.Paint += new PaintEventHandler(Viewport_Paint);
			Loaded=true;
		}

        void Viewport_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }
		
		protected override void OnResize(EventArgs e)
		{
			if(!Loaded) return;
			base.OnResize(e);
			
			GL.Viewport(ClientRectangle);
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI/4, (float)Width/Height, 0.05f, 1024f);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);
			GL.MatrixMode(MatrixMode.Modelview);
		}

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Viewport
            // 
            this.Name = "Viewport";
            this.ResumeLayout(false);

        }
	}
}
