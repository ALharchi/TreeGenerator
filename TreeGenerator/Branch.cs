using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace TreeGenerator
{
    public class Branch
    {
        public Branch Parent { get; private set; }
        public Vector2d GrowDirection { get; set; }
        public Vector2d OriginalGrowDirection { get; set; }
        public int GrowCount { get; set; }
        public Vector2d Position { get; private set; }

        public Branch(Branch parent, Vector2d position, Vector2d growDirection)
        {
            Parent = parent;
            Position = position;
            GrowDirection = growDirection;
            OriginalGrowDirection = growDirection;
        }

        public void Reset()
        {
            GrowCount = 0;
            GrowDirection = OriginalGrowDirection;
        }
    }
}
