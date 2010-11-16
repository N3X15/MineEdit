using System;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Vector3d = OpenTK.Vector3d;
using Vector2 = OpenTK.Vector2;
using System.Diagnostics;
namespace OpenMinecraft.Rendering3D
{
	public class Camera
	{
		bool _lastMouseEnabled = false;
		GLControl control;
        Stopwatch movetime;

		public OpenTK.Vector3d Location { get; set; }
		public OpenTK.Vector2 Rotation { get; set; }
		
		public float MoveSpeed { get; set; }
		public float MouseSpeed { get; set; }
		public bool MoveEnabled { get; set; }
		public bool MouseEnabled { get; set; }
		public Point MousePosition { get; set; }
		
		public Camera(GLControl c)
		{
			control=c;
			MoveSpeed = 15;
			MouseSpeed = 5;
			MousePosition = new Point(control.Width/2, control.Height/2);
			control.MouseMove += new MouseEventHandler(control_MouseMove);
			control.KeyDown += new KeyEventHandler(control_KeyDown);
            movetime = Stopwatch.StartNew();
		}

		void control_KeyDown(object sender, KeyEventArgs e)
        {
            long ms = movetime.ElapsedMilliseconds;
            movetime.Reset();
            movetime.Start();
            if (MouseEnabled && control.Focused && (e.KeyCode & Keys.Escape) == Keys.Escape)
            {
                MouseEnabled = false;
            }
			if (MoveEnabled && control.Focused) {
				double x = Location.X;
				double y = Location.Y;
				double z = Location.Z;
				
				bool A = (e.KeyCode & Keys.A) == Keys.A;
				bool D = (e.KeyCode & Keys.D) == Keys.D;
				bool S = (e.KeyCode & Keys.S) == Keys.S;
				bool W = (e.KeyCode & Keys.W) == Keys.W;
				bool LShift = (e.KeyData & Keys.LShiftKey) == Keys.LShiftKey;
				bool LControl = (e.KeyData & Keys.LControlKey) == Keys.LControlKey;
				
				float yaw = (float)Math.PI * Rotation.X / 180;
				float pitch = (float)Math.PI * Rotation.Y / 180;
                double speed = MoveSpeed * (float)ms;
				if (A ^ D && W ^ S)
					speed /= Math.Sqrt(2);
				if (LShift && !LControl) 
					speed *= 5;
				else if (!LShift && LControl)  
					speed /= 3;
				if (W && !S) {
					x -= Math.Sin(yaw) * Math.Cos(pitch) * speed;
					y += Math.Sin(pitch) * speed;
					z += Math.Cos(yaw) * Math.Cos(pitch) * speed;
				} else if (S && !W) {
					x += Math.Sin(yaw) * Math.Cos(pitch) * speed;
					y -= Math.Sin(pitch) * speed;
					z -= Math.Cos(yaw) * Math.Cos(pitch) * speed;
				}
				if (A && !D) {
					x += Math.Cos(yaw) * speed;
					z += Math.Sin(yaw) * speed;
				} else if (D && !A) {
					x -= Math.Cos(yaw) * speed;
					z -= Math.Sin(yaw) * speed;
				}
				Location = new OpenTK.Vector3d(x, y, z);
               // Console.WriteLine("Camera is now at {0}", Location);
			}
		}

		void control_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            long ms = movetime.ElapsedMilliseconds;
            movetime.Reset();
            movetime.Start();
			if(MouseEnabled && _lastMouseEnabled && control.Focused)
			{
				float yaw = Rotation.X;
				float pitch = Rotation.Y;
				yaw += (e.X - MousePosition.X) * MouseSpeed * (float)ms;
                pitch += (e.Y - MousePosition.Y) * MouseSpeed * (float)ms;
				yaw = ((yaw % 360) + 360) % 360;
				pitch = Math.Max(-90, Math.Min(90, pitch));
				Cursor.Position = control.PointToScreen(MousePosition);
				Rotation = new Vector2(yaw, pitch);
                //Console.WriteLine("Camera's rotation is now {0}", Rotation);
			} else if (MouseEnabled && !_lastMouseEnabled && control.Focused) {
				Cursor.Position = control.PointToScreen(MousePosition);
				Cursor.Hide();
				_lastMouseEnabled = true;
			} else {
				Cursor.Show();
				_lastMouseEnabled = false;
			}
			
		}
		
		public void Update()
		{
		}
		
		public void Render()
		{
			GL.Rotate(Rotation.Y, 1.0, 0.0, 0.0);
			GL.Rotate(Rotation.X, 0.0, 1.0, 0.0);
			GL.Translate(Location);
		}
	}
}