using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft
{
    ///<summary>
    /// Set up the methods for a larger more complicated tree.
    /// 
    /// This tree type has roots, a trunk, and branches all of varying width, 
    /// and many foliage clusters.
    ///
    /// MUST BE SUBCLASSED.  Specifically, foliage_shape must be set.
    /// Subclass 'prepare' and 'shapefunc' to make different shaped trees.
    /// </summary>
    internal class ProceduralTree:Tree
    {
        private  List<int> foliage_shape;
        private  List<Vector3i> foliage_cords = new List<Vector3i>();
        private  int LIGHTTREE;
        private  double trunkradius;
        private  double branchdensity;
        private  double branchslope;
        private  double trunkheight;
        Random rand = new Random();



        public double BRANCHDENSITY { get; set; }
        public double FOLIAGEDENSITY { get; set; }
        public int TRUNKTHICKNESS { get; set; }
        public bool ROOTBUTTRESSES { get; set; }
        public string SHAPE { get; set; }
        public  string ROOTS { get; set; }

        public ProceduralTree(long _x, long _y, long _z, int h)
            :base(_x,_y,_z,h)
        {
            List<Vector3i> foliage_coords = new List<Vector3i>();
            trunkradius = Math.Sqrt(Height * TRUNKTHICKNESS);
            if(trunkradius < 1)
                trunkradius = 1;
            trunkheight = Height * 0.618;
            branchdensity = BRANCHDENSITY / FOLIAGEDENSITY;
            
            long ystart = Pos[1];
            long yend = Pos[1] + Height;
            int num_of_clusters_per_y = (int)(1.5 + Math.Pow(FOLIAGEDENSITY * 
                                               Height / 19,2));
            if(num_of_clusters_per_y < 1)
                num_of_clusters_per_y = 1;
            for(int y = (int)yend;y>ystart;y--)
            {
                for(int i = 0;i<num_of_clusters_per_y;i++)
                {
                    double shapefac = shapefunc(y-(int)ystart);
                    if(shapefac == -1)
                        continue;
                    double r = (Math.Sqrt(rand.NextDouble()) + .328)*shapefac;
                
                    double theta = rand.NextDouble()*2*Math.PI;
                    int x = (int)((r*Math.Sin(theta)) + Pos[0]);
                    int z = (int)((r*Math.Cos(theta)) + Pos[2]);
                    foliage_coords.Add(new Vector3i(x,y,z));
                }
            }

            foliage_cords = foliage_coords;
        }
        /// <summary>
        /// Create a round section of type matidx in blocklist.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="vCenter">the coordinates of the center block</param>
        /// <param name="radius">the radius of the section.</param>
        /// <param name="diraxis">The list index for the axis to make the section perpendicular to.  0 indicates the x axis, 1 the y, 2 the z.  The section will extend along the other two axies.</param>
        /// <param name="mat">What to make the section out of</param>
        public void crossection(ref IMapHandler map, Vector3i vCenter, double radius, int diraxis, byte mat)
        {
            long[] centArray=vCenter.ToArray();
            int rad = (int)(radius + .618d);
            int secidx1 = (diraxis - 1)%3;
            int secidx2 = (1 + diraxis)%3;
            int[] coord = new int []{0,0,0};
            for(int off1 =-rad; off1<rad+1;off1++)
            {
                for(int off2 =-rad; off2<rad+1;off2++)
                {
                    double thisdist = Math.Sqrt(Math.Pow((double)Math.Abs(off1)+.5d,2d) + Math.Pow((double)Math.Abs(off2) + .5d,2));
                    if(thisdist > radius)
                        continue;
                    int pri = (int)centArray[diraxis];
                    int sec1 = (int)centArray[secidx1] + off1;
                    int sec2 = (int)centArray[secidx2] + off2;
                    coord[diraxis] = pri;
                    coord[secidx1] = sec1;
                    coord[secidx2] = sec2;
                    map.SetBlockAt(coord[0],coord[1],coord[2],mat);
                }
            }
        }
        
        /// <summary>
        /// Take y and return a radius for the location of the foliage cluster.
        /// 
        /// If no foliage cluster is to be created, return -1.
        /// Designed for subclassing.  Only makes clusters close to the trunk.
        /// </summary>
        public double shapefunc(int y)
        {
            if(rand.NextDouble() < 100/((Height)^2) && y < trunkheight)
                return (double)Height * .12d;
            return -1;
        }
        
        /// <summary>
        /// generate a round cluster of foliage at the location center.
        /// 
        /// The shape of the cluster is defined by the list foliage_shape.
        /// This list must be set in a subclass of ProceduralTree.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="center"></param>
        public void foliagecluster(ref IMapHandler map, Vector3i center)
        {
            List<int> level_radius = this.foliage_shape;
            foreach(int i in level_radius)
            {
                crossection(ref map, center,(double)i,1,18);
                center.Y++;
            }
        }

        /// <summary>
        /// Create a tapered cylinder in blocklist.
        /// start and end are the beginning and ending coordinates of form [x,y,z].
        /// startsize and endsize are the beginning and ending radius.
        /// The material of the cylinder is 17, which indicates wood in Minecraft.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startsize"></param>
        /// <param name="endsize"></param>
        public void taperedlimb(ref IMapHandler map, Vector3i vStart, Vector3i vEnd, int startsize, int endsize)
        {
        
            // delta is the coordinate vector for the difference between
            // start and end.
            Vector3i vDelta = vEnd - vStart;

            long[] delta = vDelta.ToArray();
            long[] start = vStart.ToArray();
            long[] end = vEnd.ToArray();

            // primidx is the index (0,1,or 2 for x,y,z) for the coordinate
            // which has the largest overall delta.
            long maxdist = 0;
            int primidx=0;
            for(int i = 0;i<3;i++)
            {
                if(Math.Abs(delta[i])>maxdist)
                {
                    maxdist=delta[i];
                    primidx=i;
                }
            }
            if(maxdist == 0)
                return;

            // secidx1 and secidx2 are the remaining indicies out of [0,1,2].
            int secidx1 = (primidx - 1)%3;
            int secidx2 = (1 + primidx)%3;
            // primsign is the digit 1 or -1 depending on whether the limb is headed
            // along the positive or negative primidx axis.
            long primsign = delta[primidx]/Math.Abs(delta[primidx]);
            // secdelta1 and ...2 are the amount the associated values change
            // for every step along the prime axis.
            long secdelta1 = delta[secidx1];
            double secfac1 = (double)(secdelta1)/delta[primidx];
            long secdelta2 = delta[secidx2];
            double secfac2 = (double)(secdelta2)/delta[primidx];
            // Initialize coord.  These values could be anything, since 
            // they are overwritten.
            long[] coord = new long[]{0,0,0};
            // Loop through each crossection along the primary axis,
            // from start to end.
            long endoffset = delta[primidx] + primsign;
            for(long primoffset=0;primoffset<endoffset;primoffset+=primsign)//primoffset in range(0, endoffset, primsign):
            {
                long primloc = start[primidx] + primoffset;
                int secloc1 = (int)(start[secidx1] + primoffset*secfac1);
                int secloc2 = (int)(start[secidx2] + primoffset*secfac2);
                coord[primidx] = primloc;
                coord[secidx1] = secloc1;
                coord[secidx2] = secloc2;
                long primdist = Math.Abs(delta[primidx]);
                int radius = (int)(endsize + (startsize-endsize) * Math.Abs(delta[primidx] - primoffset) / primdist);
                crossection(ref map,new Vector3i(coord[0],coord[1],coord[2]),radius,primidx,17);
            }
        }

        public override void  MakeFoliage(ref IMapHandler map)
        {
            List<Vector3i> foliage_coords = foliage_cords;
            foreach(Vector3i coord in foliage_coords)
            {
                foliagecluster(ref map,coord);
            }
            foreach(Vector3i cord in foliage_coords)
            {
                map.SetBlockAt(cord.X,cord.Y,cord.Z,17);
                if(LIGHTTREE == 1)
                {
                    map.SetBlockAt(cord.X,cord.Y+1,cord.Z,50);
                    map.SetBlockAt(cord.X,cord.Y+2,cord.Z,17);
                }
                else if (LIGHTTREE == 2 || LIGHTTREE == 4)
                {
                    map.SetBlockAt(cord.X+1,cord.Y,cord.Z,50);
                    map.SetBlockAt(cord.X-1,cord.Y,cord.Z,50);
                    if(LIGHTTREE == 4)
                    {
                        map.SetBlockAt(cord.X,cord.Y,cord.Z+1,50);
                        map.SetBlockAt(cord.X,cord.Y,cord.Z-1,50);
                    }
                }
            }
        }

        public override void  MakeTrunk(ref IMapHandler map)
        {
            int starty = (int)Pos[1];
            int midy = (int)(Pos[1]+(trunkheight*.382));
            int topy = (int)(Pos[1]+(trunkheight + 0.5));
            // In this method, x and z are the position of the trunk.
            long x = Pos[0];
            long z = Pos[2];
            double midrad = trunkradius * .8;
            double endrad = trunkradius * (1 - trunkheight/Height);

            if(endrad < 1.0)
                endrad = 1.0;
            
            if(midrad < endrad)
                midrad = endrad;
            
            List<Vector3i> rootbases = new List<Vector3i>();
            // Make the root buttresses, if indicated
            double startrad;
            if(ROOTBUTTRESSES || SHAPE == "mangrove")
            {
                // The start radius of the trunk should be a little smaller if we
                // are using root buttresses.
                startrad = trunkradius * .8;

                // rootbases is used later in makeroots(...) as
                // starting locations for the roots.
                rootbases.Add(new Vector3i(x,z,(long)startrad));

                double buttress_radius = trunkradius * 0.382;
                // posradius is how far the root buttresses should be offset
                // from the trunk.
                double posradius = trunkradius;
                // In mangroves, the root buttresses are much more extended.
                if(SHAPE == "mangrove")
                    posradius = posradius *2.618;

                int num_of_buttresses = (int)(Math.Sqrt(trunkradius) + 3.5);
                for(int i = 0;i<num_of_buttresses;i++)
                {
                    double rndang = rand.NextDouble()*2*Math.PI;
                    double thisposradius = posradius * (0.9 + rand.NextDouble()*.2);
                    
                    // thisx and thisz are the x and z position for the base of
                    // the root buttress.
                    long thisx = x + (long)(thisposradius * Math.Sin(rndang));
                    long thisz = z + (long)(thisposradius * Math.Cos(rndang));

                    // thisbuttressradius is the radius of the buttress.
                    // Currently, root buttresses do not taper.
                    double thisbuttressradius = buttress_radius * (0.618 + rand.NextDouble());
                    if(thisbuttressradius < 1.0)
                        thisbuttressradius = 1.0;
                    // Make the root buttress.
                    taperedlimb(ref map, new Vector3i(thisx,starty,thisz),new Vector3i(x,midy,z),
                                     (int)thisbuttressradius,(int)thisbuttressradius);
                    // Add this root buttress as a possible location at
                    // which roots can spawn.
                    rootbases.Add(new Vector3i(thisx,thisz,(long)thisbuttressradius));
                }
            }else{
                // If root buttresses are turned off, set the trunk radius
                // to normal size.
                rootbases.Clear();
                startrad=trunkradius;
                rootbases.Add(new Vector3i(x,z,(long)trunkradius));
            }
            // Make the lower and upper sections of the trunk.
            taperedlimb(ref map, new Vector3i(x,starty,z), new Vector3i(x,midy,z),(int)startrad,(int)midrad);
            taperedlimb(ref map,new Vector3i(x,midy,z),new Vector3i(x,topy,z),(int)midrad,(int)endrad);
            // Make the branches
            makebranches(ref map);
            // Make the roots, if indicated.
            if (ROOTS == "yes" || ROOTS == "tostone" || ROOTS == "hanging")
                makeroots(ref map, rootbases);
        }

        /// <summary>
        /// Generate branches.
        /// </summary>
        /// <param name="map"></param>
        public void makebranches(ref IMapHandler map)
        {
            long topy = Pos[1]+(int)((int)trunkheight + 0.5d);
            double endrad = trunkradius * (1 - trunkheight/Height);
            if(endrad < 1.0)
                endrad = 1.0;
            foreach(Vector3i coord in foliage_cords)
            {
                double dist = Math.Sqrt(
                    Math.Pow(coord.X - Pos.X,2d) +
                    Math.Pow(coord.Z - Pos.Z,2d));
                double ydist = coord.Y-Pos.Y;
                double value = (branchdensity * 220 * Height)/Math.Pow((ydist + dist), 3d);
                if(value < rand.NextDouble())
                    continue;
            
                long posy = coord.Y;
                double slope = branchslope + (0.5 - rand.NextDouble())*.16;
                long branchy;
                double basesize;
                if(coord.Y - dist*slope > topy)
                {
                    double threshhold = 1d / (double)(Height);
                    if(rand.NextDouble() < threshhold)
                        continue;
                    branchy = topy;
                    basesize = endrad;
                }
                else
                {
                    branchy = (long)(posy-dist*slope);
                    basesize = (endrad + (trunkradius-endrad) * (topy - branchy) / trunkheight);
                }
                double startsize = (basesize * (1 + rand.NextDouble()) * .618 * 
                             Math.Pow(dist/Height,0.618));
                double rndr = Math.Sqrt(rand.NextDouble())*basesize*0.618;
                double rndang = rand.NextDouble()*2*Math.PI;
                int rndx = (int)(rndr*Math.Sin(rndang) + 0.5);
                int rndz = (int)(rndr*Math.Cos(rndang) + 0.5);
                Vector3i startcoord = new Vector3i(Pos[0]+rndx,
                              (int)branchy,
                              Pos[2]+rndz);
                if(startsize < 1.0)
                    startsize = 1.0;
                double endsize = 1.0;
                taperedlimb(ref map, startcoord, coord, (int)startsize, (int)endsize);
            }
        }
    
        public void makeroots(ref IMapHandler map, List<Vector3i> rootbases)
        {
            foreach(Vector3i coord in foliage_cords)
            {
                // First, set the threshhold for randomly selecting this 
                // coordinate for root creation.
                double dist = Math.Sqrt(   Math.Pow(coord[0]-Pos[0],2) +
                                    Math.Pow(coord[2]-Pos[2],2));
                double ydist = coord[1]-Pos[1];
                double value = (branchdensity * 220 * Height)/Math.Pow((ydist + dist), 3d);
                
                // Randomly skip roots, based on the above threshold
                if(value < rand.NextDouble())
                    continue;

                // initialize the internal variables from a selection of 
                // starting locations.
                int ci = rand.Next(0, rootbases.Count-1);
                Vector3i rootbase = rootbases[ci];
                long rootx = rootbase[0];
                long rootz = rootbase[1];
                double rootbaseradius = rootbase[2];
                // Offset the root origin location by a random amount
                // (radialy) from the starting location.
                double rndr = (Math.Sqrt(rand.NextDouble())*rootbaseradius*.618);
                double rndang = rand.NextDouble()*2*Math.PI;
                int rndx = (int)(rndr*Math.Sin(rndang) + 0.5);
                int rndz = (int)(rndr*Math.Cos(rndang) + 0.5);
                int rndy = (int)(rand.NextDouble()*rootbaseradius*0.5);
                Vector3i startcoord = new Vector3i(rootx+rndx,Pos[1]+rndy,rootz+rndz);
                // offset is the distance from the root base to the root tip.
                Vector3i offset = startcoord-coord;
                // If this is a mangrove tree, make the roots longer.
                if(SHAPE == "mangrove")
                {
                    offset = (offset * 1.618) - 1.5;
                }
                Vector3i endcoord = startcoord+offset;
                double rootstartsize = (rootbaseradius*0.618* Math.Abs(offset[1])/(Height*0.618));
                if(rootstartsize < 1.0)
                    rootstartsize = 1.0;
                double endsize = 1.0;
                // If ROOTS is set to "tostone" or "hanging" we need to check
                // along the distance for collision with existing materials.
                if(ROOTS == "tostone" || ROOTS == "hanging")
                {
                    double offlength = Math.Sqrt(
                        Math.Pow(offset[0],2) +
                        Math.Pow(offset[1],2) +
                        Math.Pow(offset[2],2));

                    if(offlength < 1)
                        continue;
                    double rootmid = endsize;
                    // vec is a unit vector along the direction of the root.
                    Vector3i vec = offset/offlength;
                    byte searchindex;
                    if(ROOTS == "tostone")
                        searchindex = 1;
                    else // Hanging
                        searchindex = 0;
                    // startdist is how many steps to travel before starting to
                    // search for the material.  It is used to ensure that large
                    // roots will go some distance before changing directions
                    // or stopping.
                    double startdist = (int)(rand.NextDouble()*6*Math.Sqrt(rootstartsize) + 2.8);
                    // searchstart is the coordinate where the search should begin
                    Vector3i searchstart = (startcoord + startdist) * vec;

                    // dist stores how far the search went (including searchstart) 
                    // before encountering the expected marterial.
                    dist = startdist + map.DistanceToMaterial(searchstart,vec,searchindex);

                    // If the distance to the materila is less than the length
                    // of the root, change the end point of the root to where
                    // the search found the material.
                    if(dist < offlength)
                        // rootmid is the size of the crossection at endcoord.
                        rootmid +=  (rootstartsize - endsize)*(1-dist/offlength);
                        // endcoord is the midpoint for hanging roots, 
                        // and the endpoint for roots stopped by stone.
                        endcoord = startcoord+(vec*dist);
                        if(ROOTS == "hanging")
                        {
                            // remaining_dist is how far the root had left
                            // to go when it was stopped.
                            double remaining_dist = offlength - dist;
                            // Initialize bottomcord to the stopping point of
                            // the root, and then hang straight down
                            // a distance of remaining_dist.
                            Vector3i bottomcord = endcoord;
                            bottomcord.Y += -(long)remaining_dist;
                            // Make the hanging part of the hanging root.
                            taperedlimb(ref map, endcoord, bottomcord, (int)rootmid, (int)endsize);
                        }
                
                    // make the beginning part of hanging or "tostone" roots
                        taperedlimb(ref map, startcoord, endcoord, (int)rootstartsize, (int)rootmid);
        
                // If you aren't searching for stone or air, just make the root.
                }
                else
                {
                    taperedlimb(ref map, startcoord, endcoord, (int)rootstartsize, (int)endsize);
                }
            }
        }
    }
}
