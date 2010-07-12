using System;
using System.Collections.Generic;

using System.Text;

namespace MineEdit
{
    static class Utils
    {
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

        public static float Lerp(float a, float b, float u)
        {
            return a + ((b - a) * u);
        }
        public static int Lerp(int a, int b, int u)
        {
            return a + ((b - a) * u);
        }

        public static double UnixTimestamp()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }
    }
}
