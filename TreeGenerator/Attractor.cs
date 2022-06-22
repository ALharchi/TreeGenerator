using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace TreeGenerator
{
    public class Attractor
    {
        public Vector2d Position { get; set; }
        public Branch ClosestBranch { get; set; }

        public Attractor(Vector2d position)
        {
            Position = position;
        }
    }
}
