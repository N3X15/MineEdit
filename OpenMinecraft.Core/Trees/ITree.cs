using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMinecraft
{
    /// <summary>
    /// Set up the interface for tree objects.  Designed for subclassing.
    /// </summary>
    public class Tree
    {
        public Vector3i Pos;
        public int Height;

        public Tree(long x, long y, long z, int h)
        {
            Pos.X=x;
            Pos.Y=y;
            Pos.Z=z;
            Height=h;
        }

        //Generate the trunk and enter it in blocklist.
        public virtual void MakeTrunk(ref IMapHandler map)
        {
        }

        /// <summary>
        /// Generate the foliage and enter it in blocklist.
        ///
        /// Note, foliage will disintegrate if there is no foliage below, or
        /// if there is no "log" block within range 2 (square) at the same level or 
        /// one level below
        /// </summary>
        /// <param name="map"></param>
        public virtual void MakeFoliage(ref IMapHandler map)
        {
        }

        /// <summary>
        /// Copy the essential values of the other tree object into self.
        /// </summary>
        /// <param name="other"></param>
        public virtual void Copy(Tree other)
        {
            Pos=other.Pos;
            Height=other.Height;
        }
    }
    /*
    class BambooTree(StickTree):
        '''Set up the foliage for a bamboo tree.
    
        Make foliage sparse and adjacent to the trunk.
        '''
        def makefoliage(self,blocklist):
            start = self.pos[1]
            end = self.pos[1] + self.height + 1
            for y in range(start,end):
                for i in [0,1]:
                    xoff = choice([-1,1])
                    zoff = choice([-1,1])
                    x = self.pos[0] + xoff
                    z = self.pos[2] + zoff
                    assign_value(x,y,z,18,blocklist)


    class PalmTree(StickTree):
        '''Set up the foliage for a palm tree.
    
        Make foliage stick out in four directions from the top of the trunk.
        '''
        def makefoliage(self,blocklist):
            y = self.pos[1] + self.height
            for xoff in range(-2,3):
                for zoff in range(-2,3):
                    if abs(xoff) == abs(zoff):
                        x = self.pos[0] + xoff
                        z = self.pos[2] + zoff
                        assign_value(x,y,z,18,blocklist)




    class RoundTree(ProceduralTree):
        '''This kind of tree is designed to resemble a deciduous tree.
        '''
        def prepare(self):
            ProceduralTree.prepare(self)
            self.branchslope = 0.382
            self.foliage_shape = [2,3,3,2.5,1.6]
            self.trunkradius = self.trunkradius * 0.8
            self.trunkheight = TRUNKHEIGHT * self.height
        
        def shapefunc(self,y):
            twigs = ProceduralTree.shapefunc(self,y)
            if twigs is not None:
                return twigs
            if y < self.height * (.282 + .1*sqrt(random())) :
                return None
            radius = self.height / 2.
            adj = self.height/2. - y
            if adj == 0 :
                dist = radius
            elif abs(adj) >= radius:
                dist = 0
            else:
                dist = sqrt( ((radius)**2) - ((adj)**2) )
            dist = dist * .618
            return dist
            

    class ConeTree(ProceduralTree):
        '''this kind of tree is designed to resemble a conifer tree.
        '''
        def prepare(self):
            ProceduralTree.prepare(self)
            self.branchslope = 0.15
            self.foliage_shape = [3,2.6,2,1]
            self.trunkradius = self.trunkradius * 0.618
            self.trunkheight = self.height
        
        def shapefunc(self,y):
            twigs = ProceduralTree.shapefunc(self,y)
            if twigs is not None:
                return twigs
            if y < self.height * (.25 + .05*sqrt(random())) :
                return None
            radius = (self.height - y )*0.382
            if radius < 0:
                radius = 0
            return radius


    class RainforestTree(ProceduralTree):
        '''This kind of tree is designed to resemble a rainforest tree.
        '''
        def prepare(self):
            self.foliage_shape = [3.4,2.6]
            ProceduralTree.prepare(self)
            self.branchslope = 1.0
            self.trunkradius = self.trunkradius * 0.382
            self.trunkheight = self.height * .9
    
        def shapefunc(self,y):
            if y < self.height * 0.8:
                if HEIGHT < self.height:
                    twigs = ProceduralTree.shapefunc(self,y)
                    if (twigs is not None) and random() < 0.05:
                        return twigs
                return None
            else:
                width = self.height * .382
                topdist = (self.height - y)/(self.height*0.2)
                dist = width * (0.618 + topdist) * (0.618 + random()) * 0.382
                return dist


    class MangroveTree(RoundTree):
        '''This kind of tree is designed to resemble a mangrove tree.
        '''
        def prepare(self):
            RoundTree.prepare(self)
            self.branchslope = 1.0
            self.trunkradius = self.trunkradius * 0.618
    
        def shapefunc(self,y):
            val = RoundTree.shapefunc(self,y)
            if val is None:
                return val
            val = val * 1.618
            return val
    */


}
