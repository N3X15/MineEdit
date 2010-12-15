using System;
using System.Collections.Generic;

namespace OpenMinecraft
{
    // This is a simple, stupid system to make a cave using catmull-rom splines.
    // Must be done AFTER genning the map.
    // (new Cave(ref MapHandler, StartingVector));
    public class Cave
    {
        Random rand;
        IMapHandler mMap;
        double delta_t;

        List<Vector3i> Points = new List<Vector3i>();

        public Cave(ref Random rnd, ref IMapHandler mh, Vector3i StartingPoint)
        {
            mMap = mh;
            rand = rnd;
            AddPoint(StartingPoint);
            // We need at least 4 points.
            int numPoints2Make = rand.Next(3, 10);
            for(int i=0;i<numPoints2Make;i++)
            {
                i++;
                AddPoint(new Vector3i(StartingPoint.X+rand.Next(-16, 16), StartingPoint.Y+rand.Next(-16, 16), StartingPoint.Z+rand.Next(-16, 16)));
            }
            Profiler profSphere = new Profiler("MakeSphere");
            Profiler profSpline = new Profiler("GetInterpolatedSplinePoint");
            int rad = rand.Next(1, 3);
            for(int p = 0;p<20;p++)
            {
                double t = (double)p/(double)(Points.Count*32);
                // Between 2/10 radius.
                profSpline.Start();
                Vector3i derp = this.GetInterpolatedSplinePoint(t);
                profSpline.Stop();


                mMap.SetBlockAt(derp.X, derp.Y, derp.Z, 0);

                profSphere.Start();
                MakeSphere(derp, rad);
                profSphere.Stop();
                //Console.WriteLine("MakeSphere r={0} @ t={1}", rad, t);
                //t += 0.05;
            }
            mMap.SaveAll();
            Console.WriteLine(profSpline.ToString());
            Console.WriteLine(profSphere.ToString());
        }
        
        Vector3i GetInterpolatedSplinePoint(double t)
        {
            // Find out in which interval we are on the spline
            int p = (int)(t / delta_t);
            // Compute local control point indices
            int p0 = p - 1;     p0=Utils.Clamp(p0,0,(int)Points.Count-1);
            int p1 = p;         p1=Utils.Clamp(p1,0,(int)Points.Count-1);
            int p2 = p + 1;     p2=Utils.Clamp(p2,0,(int)Points.Count-1);
            int p3 = p + 2;     p3=Utils.Clamp(p3,0,(int)Points.Count-1);
            // Relative (local) time 
	        double lt = (t - delta_t*(double)p) / delta_t;
            return (Vector3i)Utils.PointOnCurve((Vector3d)Points[p0], (Vector3d)Points[p1], (Vector3d)Points[p2], (Vector3d)Points[p3], (float)lt);
        }
        private void AddPoint(Vector3i lolwut)
        {
            Points.Add(lolwut);
            delta_t = 1d / (double)Points.Count;
        }
        private void MakeSphere(Vector3i pos, int rad)
        {
            Profiler profRead = new Profiler("Read"); 
            Profiler profWrite = new Profiler("Write");
            int radsq = rad ^ 2; // So we don't have to do sqrt, which is slow

            for (int x = (int)pos.X - rad; x < pos.X + rad; x++)
            {
                for (int y = (int)pos.Y - rad; y < pos.Y + rad; y++)
                {
                    for (int z = (int)pos.Z - rad; z < pos.Z + rad; z++)
                    {
                        if(y<0||y>=mMap.ChunkScale.Y-1) continue;

                        profRead.Start();
                        byte block = mMap.GetBlockAt(x,y,z);
                        profRead.Stop();
                        //byte blockabove = mMap.GetBlockAt(x,y+1,z);

                        // If water/sand/gravel, or the block above is, abort
                        if (block == 0 || block == 8 || block == 9 || block == 12 || block == 13 || block==KnownBlocks.Error) 
                            continue;
                        //if (blockabove == 0 || blockabove == 8 || blockabove == 9 || blockabove == 12 || blockabove == 13) 
                        //    continue;

                        int distsq = (x - (int)pos.X) ^ 2 + (y - (int)pos.Y) ^ 2 + (z - (int)pos.Z);
                        if (distsq <= radsq)
                        {
                            profWrite.Start();
                            mMap.SetBlockAt(x, y, z, 0);
                            profWrite.Stop();
                        }
                    }
                }
            }

            Console.WriteLine(profRead.ToString());
            Console.WriteLine(profWrite.ToString());
        }
    }
}
