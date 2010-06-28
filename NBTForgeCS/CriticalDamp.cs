using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineEdit
{
    public static class CriticalDamp
    {
        /** 
         * @file llcriticaldamp.cpp
         * @brief Implementation of the critical damping functionality.
         *
         * $LicenseInfo:firstyear=2002&license=viewergpl$
         * 
         * Copyright (c) 2002-2009, Linden Research, Inc.
         * 
         * Second Life Viewer Source Code
         * The source code in this file ("Source Code") is provided by Linden Lab
         * to you under the terms of the GNU General Public License, version 2.0
         * ("GPL"), unless you have obtained a separate licensing agreement
         * ("Other License"), formally executed by you and Linden Lab.  Terms of
         * the GPL can be found in doc/GPL-license.txt in this distribution, or
         * online at http://secondlifegrid.net/programs/open_source/licensing/gplv2
         * 
         * There are special exceptions to the terms and conditions of the GPL as
         * it is applied to this Source Code. View the full text of the exception
         * in the file doc/FLOSS-exception.txt in this software distribution, or
         * online at
         * http://secondlifegrid.net/programs/open_source/licensing/flossexception
         * 
         * By copying, modifying or distributing this software, you acknowledge
         * that you have read and understood your obligations described above,
         * and agree to abide by those obligations.
         * 
         * ALL LINDEN LAB SOURCE CODE IS PROVIDED "AS IS." LINDEN LAB MAKES NO
         * WARRANTIES, EXPRESS, IMPLIED OR OTHERWISE, REGARDING ITS ACCURACY,
         * COMPLETENESS OR PERFORMANCE.
         * $/LicenseInfo$
         */
        public static Dictionary<float,float> sInterpolants = new Dictionary<float,float>();
        public static float sTimeDelta=0f;
        private static double mTotalTime=0;
        private static int mFrameCount=0;
        private static double mFrameTime=0;
        private static double mStartTotalTime=0;
        private static double mStartTime=0;

        public static void Init()
        {
            mStartTotalTime = Utils.UnixTimestamp();
            mStartTime = mStartTotalTime;
        }
        public static void updateInterpolants()
        {
	        sTimeDelta = getElapsedTimeAndResetF32();

	        float time_constant;

	        foreach(KeyValuePair<float, float> iter in sInterpolants)
	        {
		        time_constant = iter.Key;
		        float new_interpolant = 1f - (float)Math.Pow(2.0, -sTimeDelta / time_constant);
		        new_interpolant = Utils.Clamp(new_interpolant, 0f, 1f);
		        sInterpolants[time_constant] = new_interpolant;
	        }
        }

        private static float getElapsedTimeAndResetF32()
        {
            float t = (float)(mFrameTime - mStartTime);
            mStartTime = Utils.UnixTimestamp();
            return t;

        } 
        public static float getInterpolant(float time_constant, bool use_cache)
        {
	        if (time_constant == 0f)
	        {
		        return 1f;
	        }

	        if (use_cache && sInterpolants.ContainsKey(time_constant))
	        {
		        return sInterpolants[time_constant];
	        }
	
	        float interpolant = 1f - (float)Math.Pow(2.0, -sTimeDelta / time_constant);
	        interpolant = Utils.Clamp(interpolant, 0f, 1f);
	        if (use_cache)
	        {
		        sInterpolants[time_constant] = interpolant;
	        }

	        return interpolant;
        }

        private static  void  updateTimer()
        {
            double total_time = Utils.UnixTimestamp();
            double frameDeltaTime = total_time - mTotalTime;
            mTotalTime=total_time;
            double TotalSeconds = mTotalTime;
            mFrameTime = mTotalTime - mStartTotalTime;
            mFrameCount++;
        }

    }
}
