using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using LibNbt.Tags;

namespace OpenMinecraft.TileEntities
{
	/*
	TAG_Compound: 8 entries
	{
		TAG_String("id"): Sign
		TAG_String("Text1"): 
		TAG_String("Text2"): STONE
		TAG_String("Text3"): 
		TAG_String("Text4"): 
		TAG_Int("z"): -69
		TAG_Int("y"): 77
		TAG_Int("x"): 9
	}
	 */
	public class Sign :TileEntity
	{
		private static Image icon;
		public string[] Text = new string[4];

        // Needed for autoload
        public Sign() { }

		public Sign(int CX, int CY, int CS, LibNbt.Tags.NbtCompound c)
			: base(CX, CY, CS, c)
		{
			for(int i = 0;i<4;i++)
			{
				string tagID = string.Format("Text{0}", i + 1);
				Text[i] = c.Get<NbtString>(tagID).Value;
			}
		}
		public Sign(LibNbt.Tags.NbtCompound c)
			: base(c)
		{
			for (int i = 0; i < 4; i++)
			{
				string tagID = string.Format("Text{0}", i + 1);
				Text[i] = c.Get<NbtString>(tagID).Value;
			}
		}

		public override NbtCompound ToNBT()
		{
			NbtCompound c = new NbtCompound();
			Base2NBT(ref c);
			for (int i = 0; i < 4; i++)
			{
				string tagID = string.Format("Text{0}", i + 1);
				c.Add(new NbtString(tagID, Text[i]));
			}
			return c;
		}

		public override Image Image 
		{ 
			get 
			{
				if(icon==null)
					icon = Blocks.Get(323).Image;
				return icon; 
			} 
		}

		public override string ToString()
		{

			return "Sign ("+Text.ToString()+")";
		}

        public override string GetID()
        {
            return "Sign";
        }
	}
}
