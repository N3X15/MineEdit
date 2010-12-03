using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft
{
    // COMPLETELY FUCKING ARBITRARY
    public enum BiomeType: byte
    {
        //     -.5      .5
        //  -1  |   0   |   1
        // COLD    AVG     HOT
        // DRY     AVG     WET

                            // HUMIDITY    TEMPERATURE NOTES
        Desert=0,           // DRY         HOT
        Tundra,             // DRY         COLD
        Grassland,          // DRY         AVG
        Taiga,              // WET         COLD        http://www.blueplanetbiomes.org/taiga.htm
        RainForest,         // WET         HOT/AVG     For AVG, see Olympic Rainforest.
        TemperateForest,    // AVG         AVG
    }

    public enum HumidityClass { DRY, AVG, WET }
    public enum TemperatureClass { COLD, AVG, HOT }
    public class Biome
    {
        public static TemperatureClass GetTempClass(double t)
        {
            if (t < -0.5)
                return TemperatureClass.COLD;
            else if (t < 0.5)
                return TemperatureClass.HOT;
            else
                return TemperatureClass.AVG;
        }
        public static HumidityClass GetHumidClass(double h)
        {
            if (h < -0.5)
                return HumidityClass.DRY;
            else if (h < 0.5)
                return HumidityClass.WET;
            else
                return HumidityClass.AVG;
        }
        public static BiomeType GetBiomeType(double _h, double _t)
        {
            HumidityClass h = GetHumidClass(_h);
            TemperatureClass t = GetTempClass(_t);
            switch (h)
            {
                case HumidityClass.DRY:
                    switch (t)
                    {
                        case TemperatureClass.HOT:
                            return BiomeType.Desert;
                        case TemperatureClass.AVG:
                            return BiomeType.Grassland;
                        default:
                            return BiomeType.Tundra;
                    }
                case HumidityClass.AVG:
                    if (t == TemperatureClass.COLD)
                        return BiomeType.Taiga;
                    return BiomeType.TemperateForest;
                case HumidityClass.WET:
                    if (t == TemperatureClass.COLD)
                        return BiomeType.Taiga;
                    return BiomeType.RainForest;
            }
            return BiomeType.TemperateForest;
        }

        public static bool NeedsSnowAndIce(BiomeType biomeType)
        {
            switch (biomeType)
            {
                case BiomeType.Taiga:
                case BiomeType.Tundra:
                    return true;
                default:
                    return false;
            }
        }

        public static bool NeedsTrees(BiomeType type)
        {
            switch (type)
            {
                case BiomeType.RainForest:
                case BiomeType.Taiga:
                case BiomeType.TemperateForest:
                    return true;
                default:
                    return false;
            }
        }


    }
}
