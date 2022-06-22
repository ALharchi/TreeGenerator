using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace TreeGenerator
{
    public class TreeGeneratorInfo : GH_AssemblyInfo
    {
        public override string Name => "TreeGenerator";
        public override Bitmap Icon => null;
        public override string Description => "A natural-looking tree generator. Based on the Space Colonization Algorithm.";
        public override Guid Id => new Guid("8431647B-DC41-47FC-ACBD-CD3472F65BE8");
        public override string AuthorName => "Ayoub Lharchi";
        public override string AuthorContact => "alha@kglakademi.dk";
    }
}