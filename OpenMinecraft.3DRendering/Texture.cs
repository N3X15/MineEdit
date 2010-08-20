/*
 * Created by SharpDevelop.
 * User: Rob
 * Date: 8/20/2010
 * Time: 12:36 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenMinecraft._3DRendering
{
	public class Texture
	{
		int id;
		
		public int ID { get { return id; } }
		
		Texture() {  }
		
		public static Texture Load(string path)
		{
            Console.WriteLine(path);
			Bitmap bitmap0 = new Bitmap(path);
			Bitmap bitmap1 = Resize(bitmap0, 16, 1);
			Bitmap bitmap2 = Resize(bitmap0, 16, 2);
			Bitmap bitmap3 = Resize(bitmap0, 16, 3);
			Bitmap bitmap4 = Resize(bitmap0, 16, 4);
			BitmapData data0 = bitmap0.LockBits(new Rectangle(0, 0, bitmap0.Width, bitmap0.Height),
			                                   ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			BitmapData data1 = bitmap1.LockBits(new Rectangle(0, 0, bitmap1.Width, bitmap1.Height),
			                                   ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			BitmapData data2 = bitmap2.LockBits(new Rectangle(0, 0, bitmap2.Width, bitmap2.Height),
			                                   ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			BitmapData data3 = bitmap3.LockBits(new Rectangle(0, 0, bitmap3.Width, bitmap3.Height),
			                                   ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			BitmapData data4 = bitmap4.LockBits(new Rectangle(0, 0, bitmap4.Width, bitmap4.Height),
			                                   ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			int texture;
			GL.GenTextures(1, out texture);
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 4);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap0.Width, bitmap0.Height,
			              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data0.Scan0);
			GL.TexImage2D(TextureTarget.Texture2D, 1, PixelInternalFormat.Rgba, bitmap1.Width, bitmap1.Height,
			              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data1.Scan0);
			GL.TexImage2D(TextureTarget.Texture2D, 2, PixelInternalFormat.Rgba, bitmap2.Width, bitmap2.Height,
			              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data2.Scan0);
			GL.TexImage2D(TextureTarget.Texture2D, 3, PixelInternalFormat.Rgba, bitmap3.Width, bitmap3.Height,
			              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data3.Scan0);
			GL.TexImage2D(TextureTarget.Texture2D, 4, PixelInternalFormat.Rgba, bitmap4.Width, bitmap4.Height,
			              0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data4.Scan0);
			bitmap0.UnlockBits(data0);
			bitmap1.UnlockBits(data1);
			bitmap2.UnlockBits(data2);
			bitmap3.UnlockBits(data3);
			bitmap4.UnlockBits(data4);
			bitmap0.Dispose();
			bitmap1.Dispose();
			bitmap2.Dispose();
			bitmap3.Dispose();
			bitmap4.Dispose();
			return new Texture(){ id = texture };
		}
		
		public static Bitmap Resize(Bitmap b, int blockSize, int level) {
			level += 1;
			Bitmap bitmap = new Bitmap(b.Width/level, b.Height/level);
			Graphics g = Graphics.FromImage(bitmap);
			for (int x = 0; x < b.Width; x += b.Width/blockSize)
				for (int y = 0; y < b.Height; y += b.Height/blockSize)
					g.DrawImage(b, new Rectangle(x, y, blockSize, blockSize),
					            new Rectangle(x/level, y/level, blockSize/level, blockSize/level), GraphicsUnit.Pixel);
			return bitmap;
		}
	}
}
