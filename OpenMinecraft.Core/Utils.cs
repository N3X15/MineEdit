using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using OpenMinecraft.TileEntities;
using OpenMinecraft.Entities;
using System.Security.Cryptography;

namespace OpenMinecraft
{
    public static class Utils
    {
        const int MinWaterToBeConsideredUnderground = 10;
        #region Clamp
        /// <summary>
        /// Ensure value is between min and max.
        /// </summary>
        /// <param name="val">value</param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Clamped value</returns>
        public static short Clamp(short val, short min, short max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static double Clamp(double val, double min, double max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static float Clamp(float val, float min, float max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static int Clamp(int val, int min, int max)
        {
            return Math.Min(Math.Max(val, min), max);
        }

        public static long Clamp(long val, long min, long max)
        {
            return Math.Min(Math.Max(val, min), max);
        }
        #endregion

        #region OpenJDK Stuff
        /*
         * Copyright (c) 1994, 2006, Oracle and/or its affiliates. All rights reserved.
         * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
         *
         * This code is free software; you can redistribute it and/or modify it
         * under the terms of the GNU General Public License version 2 only, as
         * published by the Free Software Foundation.  Oracle designates this
         * particular file as subject to the "Classpath" exception as provided
         * by Oracle in the LICENSE file that accompanied this code.
         *
         * This code is distributed in the hope that it will be useful, but WITHOUT
         * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
         * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
         * version 2 for more details (a copy is included in the LICENSE file that
         * accompanied this code).
         *
         * You should have received a copy of the GNU General Public License version
         * 2 along with this work; if not, write to the Free Software Foundation,
         * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
         *
         * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
         * or visit www.oracle.com if you need additional information or have any
         * questions.
         */

        /**
            * All possible chars for representing a number as a String
            */
        private static char[] digits = {
            '0' , '1' , '2' , '3' , '4' , '5' ,
            '6' , '7' , '8' , '9' , 'a' , 'b' ,
            'c' , 'd' , 'e' , 'f' , 'g' , 'h' ,
            'i' , 'j' , 'k' , 'l' , 'm' , 'n' ,
            'o' , 'p' , 'q' , 'r' , 's' , 't' ,
            'u' , 'v' , 'w' , 'x' , 'y' , 'z'
        };

        /*
        * Returns a string representation of the first argument in the
        * radix specified by the second argument.
        *
        * <p>If the radix is smaller than {@code Character.MIN_RADIX}
        * or larger than {@code Character.MAX_RADIX}, then the radix
        * {@code 10} is used instead.
        *
        * <p>If the first argument is negative, the first element of the
        * result is the ASCII minus character {@code '-'}
        * (<code>'&#92;u002D'</code>). If the first argument is not
        * negative, no sign character appears in the result.
        *
        * <p>The remaining characters of the result represent the magnitude
        * of the first argument. If the magnitude is zero, it is
        * represented by a single zero character {@code '0'}
        * (<code>'&#92;u0030'</code>); otherwise, the first character of
        * the representation of the magnitude will not be the zero
        * character.  The following ASCII characters are used as digits:
        *
        * <blockquote>
        *   {@code 0123456789abcdefghijklmnopqrstuvwxyz}
        * </blockquote>
        *
        * These are <code>'&#92;u0030'</code> through
        * <code>'&#92;u0039'</code> and <code>'&#92;u0061'</code> through
        * <code>'&#92;u007A'</code>. If {@code radix} is
        * <var>N</var>, then the first <var>N</var> of these characters
        * are used as radix-<var>N</var> digits in the order shown. Thus,
        * the digits for hexadecimal (radix 16) are
        * {@code 0123456789abcdef}. If uppercase letters are
        * desired, the {@link java.lang.String#toUpperCase()} method may
        * be called on the result:
        *
        * <blockquote>
        *  {@code Integer.toString(n, 16).toUpperCase()}
        * </blockquote>
        *
        * @param   i       an integer to be converted to a string.
        * @param   radix   the radix to use in the string representation.
        * @return  a string representation of the argument in the specified radix.
        * @see     java.lang.Character#MAX_RADIX
        * @see     java.lang.Character#MIN_RADIX
        */

        public static string IntToBase(int i, int radix) {

            if (radix < 2 || radix > 36)
                radix = 10;

            /* Use the faster version */
            if (radix == 10) {
                return i.ToString();
            }

            char[] buf = new char[33];
            bool negative = (i < 0);
            int charPos = 32;

            if (!negative) {
                i = -i;
            }

            while (i <= -radix) {
                buf[charPos--] = digits[-(i % radix)];
                i = i / radix;
            }
            buf[charPos] = digits[-i];

            if (negative) {
                buf[--charPos] = '-';
            }

            return new String(buf, charPos, (33 - charPos));
        }




        #endregion
        /// <summary>
        /// For FileSystemWatcher stuff
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #region Linear Interpolation
        /// <summary>
        /// Used for blending colors or line-drawing.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static float Lerp(float a, float b, float u)
        {
            return a + ((b - a) * u);
        }
        public static double Lerp(double a, double b, double u)
        {
            return a + ((b - a) * u);
        }
        public static int Lerp(int a, int b, int u)
        {
            return (int)((float)a + (((float)b - (float)a) * ((float)u/100f)));
        }
        #endregion

        public static double UnixTimestamp()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        #region Trees
        // All of this shit is a fixed version of all the tree stuff in NBTForge.
        // Thank you NBTForge for giving me eyes and a legal version of the block reading algoryithm. :V
        // (Also trees.)
        public static void GrowTree(ref byte[, ,] b, Random r, int x, int y, int z)
        {
            int h = r.Next(6, 8);
            GrowTreeFoliage(ref b, r, x, y, z, h);
            GrowTreeTrunk(ref b, r, x, y, z, h);
        }
        static void GrowTreeTrunk(ref byte[,,] b, Random r, int x, int y, int z, int height)
        {
            if (
                z + height > b.GetLength(2) - 1 ||
                x + 2 > b.GetLength(0) - 1 ||
                y + 2 > b.GetLength(1) - 1)
                return;
            if (
                z - height < 0 ||
                x - 2 < 0 ||
                y - 2 < 0)
                return;
            for(int i=0;i<height;i++)
	            b[x,y,z+i]=17;
        }
        // From NBTForge.
        static void GrowTreeFoliage(ref byte[, ,] b, Random r, int x, int y, int _z, int height)
        {
            _z = _z + height - 1;
            if (
                _z + 2 > b.GetLength(2) - 1 ||
                x + 2 > b.GetLength(0) - 1 ||
                y + 2 > b.GetLength(1) - 1)
                return;
            if (
                _z - 2 < 0 ||
                x - 2 < 0 ||
                y - 2 < 0)
                return;
            /*
             Note, foliage will disintegrate if there is no foliage below, or
	        if there is no "log" block within range 2 (square) at the same level or
	        one level below
             */
	        int astart = _z - 2;
	        int aend = _z + 2;
	        int rad;
            for (int z = astart; z < aend; z++)
            {
                if (z > astart + 1)
                    rad = 1;
                else
                    rad = 2;
                for (int xoff = -rad; xoff < rad + 1; xoff++)
                {
                    for (int yoff = -rad; yoff < rad + 1; yoff++)
                    {
                        for (int zoff = -rad; zoff < rad + 1; zoff++)
                        {
                            if (//Math.Abs(xoff) == Math.Abs(zoff)
                               Math.Abs(xoff) <= rad ||
                               Math.Abs(yoff) <= rad
                            )
                            {
                                b[x + xoff, y + yoff, z] = 18;
                            }
                        }
                    }
                }
            }
        }
#endregion

        public static bool CheckForTreeSpace(ref byte[, ,] b, int x, int y, int z)
        {
            return false;
        }
        
        #region Dungeons
        public static int DungeonSizeX = 2; // Air blocks
        public static int DungeonSizeY = 3; // Air blocks
        public static int DungeonSizeZ = 3;
        public static bool CheckForDungeonSpace(byte[,,] b, int x, int y, int z)
        {
            Vector3i pos=new Vector3i(x,y,z);
            Vector3i size = new Vector3i((DungeonSizeX+1)*2+1,(DungeonSizeY+1)*2+1,DungeonSizeZ+1);
            if (!ObjectIsInChunk(b, pos, size))
            {
                //Console.WriteLine("Object is not in chunk.");
                return false;
            }
            if (!ObjectIsCompletelyUnderground(b, pos, size))
            {
                //Console.WriteLine("Object is not underground.");
                return false;
            }
            return true;
        }
        public static void MakeDungeonWalls(ref byte[, ,] b, Random r, Vector3i position, Vector3i size)
        {
            for (int x = (int)(position.X - (size.X / 2)); x < (position.X + (size.X / 2)); x++)
            {
                for (int y = (int)(position.Y - (size.Y / 2)); y < (position.Y + (size.Y / 2)); y++)
                {
                    for (int z = (int)(position.Z - (size.Z / 2)); z < (position.Z + (size.Z / 2)); z++)
                    {
                        if (
                            x == position.X - size.X || x == (position.X + size.X) - 1 ||
                            y == position.Y - size.Y || y == (position.Y + size.Y) - 1 ||
                            z == position.Z - size.Z || z == (position.Z + size.Z) - 1)
                        {
                            b[x, y, z] = (r.Next(0, 1) == 1) ? (byte)4 : (byte)48;
                        }
                    }
                }
            }
        }
        public static void FillRect(ref byte[, ,] b, byte blk, Vector3i position, Vector3i size)
        {
            for (int x = (int)(position.X - (size.X/2)); x < (position.X + (size.X/2)); x++)
            {
                for (int y = (int)(position.Y - (size.Y/2)); y < (position.Y + (size.Y/2)); y++)
                {
                    for (int z = (int)(position.Z - (size.Z/2)); z < (position.Z + (size.Z/2)); z++)
                    {
                        try
                        {
                            b[x, y, z] = blk;
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        /// <summary>
        /// Gen a dungeon
        /// </summary>
        /// <param name="CX">Chunk X pos</param>
        /// <param name="CY">Chunk Y pos</param>
        /// <param name="CH">Chunk Horizontal Scale</param>
        /// <param name="b"></param>
        /// <param name="mh"></param>
        /// <param name="r"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static bool MakeDungeon(int CX, int CY, ref byte[, ,] b, ref IMapHandler mh, Random r)
        {

            int CH = (int)mh.ChunkScale.X;
            int CV = (int)mh.ChunkScale.Z;
            int x = r.Next(0+DungeonSizeX-1, CH-DungeonSizeX+1);
            int y = r.Next(0+DungeonSizeY-1, CH-DungeonSizeY+1);
            int z = r.Next(0+DungeonSizeZ-1, CV-DungeonSizeZ+1);
            
            Vector3i position = new Vector3i(x,y,z);
            //Console.WriteLine("Creating dungeon in {0}...", position);
            if (!CheckForDungeonSpace(b, x, y, z)) return false;
            Vector3i size = new Vector3i((DungeonSizeX*2)+1,(DungeonSizeY*2)+1,(DungeonSizeZ*2)+1);
            FillRect(ref b, 0, position, size);
            MakeDungeonWalls(ref b, r, position, size);
            mh.SetTileEntity(new MobSpawner(x+(int)(CX*CH), y+(int)(CY*CH), z-DungeonSizeZ+1, Entity.GetRandomMonsterID(r), 20));
            b[x, y, z - DungeonSizeZ + 1]=52;
            return true;
        }
#endregion

        private static bool ObjectIsInChunk(byte[, ,] b, Vector3i position, Vector3i size)
        {
            int ChunkX = b.GetLength(0);
            int ChunkY = b.GetLength(1);
            int ChunkZ = b.GetLength(2);
            if ((position.X - (size.X/2)) < 0 ||
                (position.Y - (size.Y/2)) < 0 ||
                (position.Z - (size.Z/2)) < 0)
            {
                //Console.WriteLine("Object with size {0} @ {1} is not within the chunk of size {2},{3},{4}", position, size, ChunkX, ChunkY, ChunkZ);
                return false;
            }

            if ((position.X + (size.X/2)) > ChunkX - 1 ||
                (position.Y + (size.Y/2)) > ChunkY - 1 ||
                (position.Z + (size.Z/2)) > ChunkZ - 1)
            {
                //Console.WriteLine("Object with size {0} @ {1} is not within the chunk of size {2},{3},{4}", position, size, ChunkX, ChunkY, ChunkZ);
                return false;
            }
                
            //Console.WriteLine("Object with size {0} @ {1} is within the chunk of size {2},{3},{4}",position,size,ChunkX,ChunkY,ChunkZ);
            return true;
        }

        private static bool ObjectIsIntersectingWithGround(byte[, ,] b, Vector3i position, Vector3i size)
        {
            int ChunkX = b.GetLength(0);
            int ChunkY = b.GetLength(1);
            int ChunkZ = b.GetLength(2);

            for (int x = (int)(position.X - (size.X / 2)); x < (position.X + (size.X / 2)); x++)
            {
                for (int y = (int)(position.Y - (size.Y/2)); y < (position.Y + (size.Y/2)); y++)
                {
                    for (int z = (int)(position.Z - (size.Z/2)); z < (position.Z + (size.Z/2)); z++)
                    {
                        if (b[x, y, z] == 8 || b[x, y, z] == 9)
                        {
                            continue;
                        }
                        if (b[x, y, z] != 0)
                            return true;
                    }
                }
            }
            return false;
        }

        private static bool ObjectIsCompletelyUnderground(byte[, ,] b, Vector3i position, Vector3i size)
        {
            int ChunkX = b.GetLength(0);
            int ChunkY = b.GetLength(1);
            int ChunkZ = b.GetLength(2);
            for (int x = (int)(position.X - (size.X/2)); x < (position.X + (size.X/2)); x++)
            {
                for (int y = (int)(position.Y - (size.Y/2)); y < (position.Y + (size.Y/2)); y++)
                {
                    int wd = 0;
                    bool IsUndergroundSoFar=false;
                    for (int z = ChunkZ-1; z > (position.Z + (size.Z/2)); z--)
                    {
                        if (b[x, y, z] == 8 || b[x, y, z] == 9)
                        {
                            wd++;
                            continue;
                        }
                        if (b[x, y, z] != 0)
                        {
                            IsUndergroundSoFar = true;
                        }
                    }
                    if (!IsUndergroundSoFar && wd<MinWaterToBeConsideredUnderground)
                        return false;
                }
            }
            return true;
        }
        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }

        public static void GrowCactus(ref byte[, ,] b, Random rand, int x, int y, int z)
        {
            for (int i = 0; i < rand.Next(1,3); i++)
                b[x, y, z + i] = 0x51;
        }
        public static double CosineInterpolate(
            double y1, double y2,
            double mu)
        {
            double mu2;

            mu2 = (1 - Math.Cos(mu * Math.PI)) / 2;
            return (y1 * (1 - mu2) + y2 * mu2);
        }


        public static void CopyFlat(string from, string to)
        {
            foreach (string _f in Directory.GetFiles(from,"*",SearchOption.AllDirectories))
            {
                string f = Path.GetFileName(_f);
                File.Copy(_f, Path.Combine(to, f),true);
            }
        }

        public static void CopyRecursive( string sourceFolder, string destFolder )
        {
            if (!Directory.Exists( destFolder ))
                Directory.CreateDirectory( destFolder );
            string[] files = Directory.GetFiles( sourceFolder );
            foreach (string file in files)
            {
                string name = Path.GetFileName( file );
                string dest = Path.Combine( destFolder, name );
                File.Copy( file, dest );
            }
            string[] folders = Directory.GetDirectories( sourceFolder );
            foreach (string folder in folders)
            {
                string name = Path.GetFileName( folder );
                string dest = Path.Combine( destFolder, name );
                CopyRecursive( folder, dest );
            }
        }
    }
    /// <summary>
	/// Radix is a convertor class for converting numbers to different radices
	/// e.g. display the number 1000 in base 16
	/// </summary>
	public class Radix
	{
		/// <summary>
		/// Lookup tables for strings
		/// </summary>
		static string digit = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Error Messages
		/// </summary>
		static string ErrRadixTooLarge1 = "RadixError: radix larger than 36.";
		static string ErrRadixTooLarge2 = "RadixError: radix larger than 1000000.";
		static string ErrRadixTooSmall = "RadixError: radix smaller than 2.";
		static string ErrRadixFormat = "RadixError: number not in radix format.";
		static string ErrRadixDecode = "RadixError: generic decode error.";
		static string ErrRadixNoSymbolFormat = "RadixError: number not in symbolic format.";

		public static string Spaces(string val, int nr)
		{
			return Spaces(val,nr, ' ');
		}

		public static string Spaces(string val, int nr, char sep)
		{
			string rv = "";
			int j = 0;
			for (int i=val.Length-1; i>=0; i--)
			{
				j++;
				rv = val[i] + rv;
				if (j % nr == 0) rv = sep + rv;
			}
			if (rv[0] == sep) rv = rv.Substring(1);
			return rv;
		}
		/// <summary>
		/// CheckArg checks the arguments for the encoder and decoder calls
		/// </summary>
		private static void CheckArg(long radix, bool sym)
		{
			if ((radix > 36) && (sym == false))
			{
				throw new Exception(ErrRadixTooLarge1);
			}
			if (radix > 1000000)
			{
				throw new Exception(ErrRadixTooLarge2);
			}
			if (radix < 2)
			{
				throw new Exception(ErrRadixTooSmall);
			}
		}

		//////////////////////////////////////////////////////////////
		/// LONG CODE
		//////////////////////////////////////////////////////////////

		public static string Encode(long x, long radix)
		{
				return Encode(x, radix, false);
		}
		/// <summary>
		/// Encoder for a long to a string with the base [radix]. if sym is true
		/// the number will be converted to a generic symbolic notation.
		/// </summary>
		public static string Encode(long x, long radix, bool sym)
		{
			// check parameters
			CheckArg(radix, sym);

			// work in positive domain
			long t = Math.Abs(x);

			// return value
			string rv = "";

			if (sym)
			{
				if (t == 0)
				{
					rv = ",0";
				}
				while (t > 0)
				{
					// split of one digit
					long r = t % radix;
					// convert it and add it to the return string
					rv = "," + r.ToString() + rv;
					t = (t-r)/radix;
				}
				rv = rv.Substring(1);			// strip one ','
				// add sign
				if (x < 0)
				{
					rv = "-," + rv;
				}
				if (x == 0)
				{
					rv = "0";
				}
				rv = "[(" + radix + ")," + rv + "]";
			}
			else
			{
				if (t==0)
				{
					rv = "0";
				}
				while (t > 0)
				{
					// split of one digit
					long r = t % radix;
					// convert it and add it to the return string
					rv = digit[(int)r] + rv;
					t = (t-r)/radix;
				}
				if (x < 0)
				{
					// add sign
					rv = "-" + rv;
				}
				if (x == 0)
				{
					rv = "0";
				}
			}
			return rv;
		}

		public static void Decode(string val, long radix, out long rv)
		{
			Decode(val, radix, out rv, false);
		}
		/// <summary>
		/// Decoder for a string to a long with the base [radix]. if sym is true
		/// the number will be converted from a generic symbolic notation.
		/// </summary>
		public static void Decode(string val, long radix, out long rv, bool sym)
		{
			CheckArg(radix,sym);
			rv = 0;
			try
			{
				if (sym)
				{
					string ws = val.Trim();
					if (ws[0] != '[')
					{
						throw new Exception(ErrRadixNoSymbolFormat);
					}
					// strip [(
					ws = ws.Substring(2);
					// get radix
					int pos = ws.IndexOf(')');
					long tr = Int64.Parse(ws.Substring(0,pos));
					// strip it
					ws = ws.Substring(pos + 2);
					ws = ws.Remove(ws.Length-1, 1);		// strip ]

					char sign = ws[0];
					int si = 1;
					if ((sign == '-') || (sign == '+'))
					{
						if (sign =='-') si = -1;
						ws = ws.Substring(2);				// skip sign and ,
					}

					string [] t = ws.Split(',');
					for (int i=0; i< t.Length; i++)
					{
						rv *= radix;
						long l = long.Parse(t[i]);
						if (l >= radix) throw new Exception(ErrRadixFormat);
						rv += l;
					}
					// add sign
					rv *= si;
				}
				else
				{
					string ws = val.Trim();
					char sign = ws[0];
					int si = 1;
					if ((sign == '-') || (sign =='+'))
					{
						if (sign =='-') si = -1;
						ws = ws.Substring(1);
					}

					for (int i=0; i< ws.Length; i++)
					{
						rv *= radix;
						char c = ws[i];
						long l = digit.IndexOf(c);
						if (l >= radix) throw new Exception(ErrRadixFormat);
						rv += l;
					}
					// add sign
					rv *= si;
				}
			}
			catch
			{
				throw new Exception(ErrRadixDecode);
			}

			return;
		}

		//////////////////////////////////////////////////////////////
		/// FLOATING POINT CODE
		//////////////////////////////////////////////////////////////

		public static string Encode(double x, long radix)
		{
				return Encode(x, radix, false);
		}

		public static string Encode(double x, long radix, bool sym)
		{
			// Check parameters
			CheckArg(radix, sym);

			double t = Math.Abs(x);

			// first part before decimal point
			long t1 = (long) t;

			// t2 holds part after decimal point
			double t2 = t - t1;

			// return value;
			string rv = "";

			if (sym)
			{
				if (x == 0.0)
				{
					rv = ",0";
				}
				// process part before decimal point
				while (t1 > 0)
				{
					long r = t1 % radix;
					rv = "," + r.ToString() + rv;
					t1 = (t1-r)/radix;
				}
				rv = rv.Substring(1);	// strip one ','

				// after the decimal point
				if (t2 > 0.0)
				{
					rv += ",.,";
				}
				int maxdigit = 50; // to prevent endless loop
				while (t2 > 0)
				{
					long r = (long) (t2 * radix);
					rv += r.ToString() + ",";
					t2 = (t2 * radix) - r;

					// forced break after maxdigits
					maxdigit--;
					if (maxdigit == 0) break;
				}
				rv = rv.Substring(0,rv.Length -1);	// strip one ','
				if (x < 0)
				{
					rv = "-," + rv;
				}
				rv = "[(" + radix + ")," + rv + "]";
			}
			else
			{
				if (x == 0.0)
				{
					rv = "0";
				}
				// process part before decimal point
				while (t1 > 0)
				{
					long r = t1 % radix;
					rv = digit[(int)r] + rv;
					t1 = (t1-r)/radix;
				}

				// after the decimal point
				if (t2 > 0.0)
				{
					rv += ".";
				}
				int maxdigit = 50; // to prevent endless loop
				while (t2 > 0)
				{
					long r = (long) (t2 * radix);
					rv += digit[(int)r];
					t2 = (t2 * radix) - r;

					// forced break after 10 digits
					maxdigit--;
					if (maxdigit == 0) break;
				}
				if (x < 0)
				{
					rv = "-" + rv;
				}
			}
			return rv;
		}

		public static void Decode(string val, long radix, out double rv)
		{
			Decode(val, radix, out rv, false);
		}
		public static void Decode(string val, long radix, out double rv, bool sym)
		{
			CheckArg(radix,sym);
			rv = 0;

			try
			{
				double tradix = 1;
				if (sym)
				{
					string ws = val.Trim();
					// strip [(
					ws = ws.Substring(2);
					// get radix
					int pos = ws.IndexOf(')');
					long tr = Int64.Parse(ws.Substring(0,pos));
					// strip it
					ws = ws.Substring(pos + 2);
					ws = ws.Remove(ws.Length-1, 1);		// strip ]

					char sign = ws[0];
					int si = 1;
					if ((sign == '-') || (sign =='+'))
					{
						if (sign =='-') si = -1;
						ws = ws.Substring(2);					// skip sign and ,
					}

					string [] t = ws.Split(',');
					bool before = true;
					for (int i=0; i< t.Length; i++)
					{
						if (t[i] == ".")
						{
							before = false;
							continue;
						}
						// next 'digit'
						long l = long.Parse(t[i]);
						if (l >= radix) throw new Exception(ErrRadixFormat);

						if (before)
						{
							// process before dec. point
							rv *= radix;
							rv += l;
						}
						else
						{
							// process after decimal point
							tradix *= radix;
							rv += l / tradix;
						}
					}

					// add sign
					rv *= si;
				}
				else
				{
					string ws = val.Trim();
					char sign = ws[0];
					int si = 1;
					if ((sign == '-') || (sign =='+'))
					{
						if (sign =='-') si = -1;
						ws = ws.Substring(1);
					}

					bool before = true;
					for (int i=0; i< ws.Length; i++)
					{
						if (ws[i] == '.')
						{
							before = false;
							continue;
						}
						// next 'digit'
						long l = digit.IndexOf(ws[i]);
						if (l >= radix) throw new Exception(ErrRadixFormat);

						if (before)
						{
							// process before dec. point
							rv *= radix;
							rv += l;
						}
						else
						{
							// process after decimal point
							tradix *= radix;
							rv += digit.IndexOf(ws[i])/ tradix;
						}
					}
					// add sign
					rv *= si;
				}
			}
			catch
			{
				throw new Exception(ErrRadixDecode);
			}
			return;
		}
	}
}
