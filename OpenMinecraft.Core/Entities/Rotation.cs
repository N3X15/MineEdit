using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNbt.Tags;

namespace OpenMinecraft
{
    public class Rotation
    {
        /*
         * TAG_List("Rotation"): Two TAG_Floats representing rotation in degrees.
         *
         *  * TAG_Float[0]: The entity's rotation clockwise around the Y axis (called yaw). Due west is 0. Can have large values because it accumulates all of the entity's lateral rotation throughout the game.
         *  * TAG_Float[1]: The entity's declination from the horizon (called pitch). Horizontal is 0. Positive values look downward. Does not exceed positive or negative 90 degrees. 
         */
        private float mYaw;
        public float Yaw
        {
            get
            {
                return mYaw;
            }
            set
            {
                mYaw = value; // Degrees? Radians?
            }
        }
        private float mPitch;
        public float Pitch
        {
            get
            {
                return mPitch;
            }
            set
            {
                mPitch = Utils.Clamp(value, -90f, 90f);
            }
        }

        public Rotation(float yaw, float pitch)
        {
            mYaw = yaw;
            mPitch = pitch;
        }

        public NbtTag ToNBT()
        {
            return new NbtList("Rotation", new NbtFloat[] 
            {
                new NbtFloat(Yaw),
                new NbtFloat(Pitch)
            });
        }

        public static Rotation FromNbt(NbtList nbtTag)
        {
            Rotation rot = new Rotation(0f, 0f);
            rot.Yaw = nbtTag.Get<NbtFloat>(0).Value;
            rot.Pitch = nbtTag.Get<NbtFloat>(1).Value;
            return rot;
        }
    }
}
