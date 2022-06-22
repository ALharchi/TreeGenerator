using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace TreeGenerator
{
    public class Tree
    {
        bool DoneGrowing = false;

        Vector2d Position = Vector2d.Zero;

        public int TrunkHeight = 60;
        public int MinDistance = 2;
        public int MaxDistance = 15;
        public int BranchLength = 2;

        Branch Root;
        public List<Attractor> Leaves = new List<Attractor>();
        public Dictionary<Vector2d, Branch> Branches;

        public Tree(Vector2d position, List<Point3d> Leaves)
        {
            Position = position;
            //Leaves = new List<Attractor>();
            foreach (Point3d pt in Leaves)
            {
                this.Leaves.Add(new Attractor(new Vector2d(pt.X, pt.Y)));
            }

            GenerateTrunk();
        }

        private void GenerateTrunk()
        {
            Branches = new Dictionary<Vector2d, Branch>();

            Root = new Branch(null, Position, new Vector2d(0, -1));
            Branches.Add(Root.Position, Root);

            Branch current = new Branch(Root, new Vector2d(Position.X, Position.Y - BranchLength), new Vector2d(0, -1));
            Branches.Add(current.Position, current);

            //Keep growing trunk upwards until we reach a leaf       
            while ((Root.Position - current.Position).Length < TrunkHeight)
            {
                Branch trunk = new Branch(current, new Vector2d(current.Position.X, current.Position.Y - BranchLength), new Vector2d(0, -1));
                Branches.Add(trunk.Position, trunk);
                current = trunk;
            }
        }

        public void Grow()
        {
            if (DoneGrowing) return;

            //If no leaves left, we are done
            if (Leaves.Count == 0)
            {
                DoneGrowing = true;
                return;
            }

            //process the leaves
            for (int i = 0; i < Leaves.Count; i++)
            {
                bool leafRemoved = false;

                Leaves[i].ClosestBranch = null;
                Vector2d direction = Vector2d.Zero;

                //Find the nearest branch for this leaf
                foreach (Branch b in Branches.Values)
                {
                    direction = Leaves[i].Position - b.Position;                       //direction to branch from leaf
                    float distance = (float)Math.Round(direction.Length);            //distance to branch from leaf

                    direction.Unitize();

                    if (distance <= MinDistance)            //Min leaf distance reached, we remove it
                    {
                        Leaves.Remove(Leaves[i]);
                        i--;
                        leafRemoved = true;
                        break;
                    }
                    else if (distance <= MaxDistance)       //branch in range, determine if it is the nearest
                    {
                        if (Leaves[i].ClosestBranch == null)
                            Leaves[i].ClosestBranch = b;
                        else if ((Leaves[i].Position - Leaves[i].ClosestBranch.Position).Length > distance)
                            Leaves[i].ClosestBranch = b;
                    }
                }

                //if the leaf was removed, skip
                if (!leafRemoved)
                {
                    //Set the grow parameters on all the closest branches that are in range
                    if (Leaves[i].ClosestBranch != null)
                    {
                        Vector2d dir = Leaves[i].Position - Leaves[i].ClosestBranch.Position;
                        dir.Unitize();
                        Leaves[i].ClosestBranch.GrowDirection += dir;       //add to grow direction of branch
                        Leaves[i].ClosestBranch.GrowCount++;
                    }
                }
            }

            //Generate the new branches
            HashSet<Branch> newBranches = new HashSet<Branch>();
            foreach (Branch b in Branches.Values)
            {
                if (b.GrowCount > 0)    //if at least one leaf is affecting the branch
                {
                    Vector2d avgDirection = b.GrowDirection / b.GrowCount;
                    avgDirection.Unitize();

                    Branch newBranch = new Branch(b, b.Position + avgDirection * BranchLength, avgDirection);

                    newBranches.Add(newBranch);
                    b.Reset();
                }
            }

            //Add the new branches to the tree
            bool BranchAdded = false;
            foreach (Branch b in newBranches)
            {
                //Check if branch already exists.  These cases seem to happen when leaf is in specific areas
                Branch existing;
                if (!Branches.TryGetValue(b.Position, out existing))
                {
                    Branches.Add(b.Position, b);
                    BranchAdded = true;
                }
            }

            //if no branches were added - we are done
            //this handles issues where leaves equal out each other, making branches grow without ever reaching the leaf
            if (!BranchAdded)
                DoneGrowing = true;
        }
    }
}
