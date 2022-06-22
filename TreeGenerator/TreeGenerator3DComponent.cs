using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TreeGenerator
{
    public class TreeGenerator3DComponent : GH_Component
    {
        public TreeGenerator3DComponent() : base("Tree Generator 3D", "Tree Generator 3D", "Generate a natural looking tree in 3D based on the Space Colonization Algorithm.", "Vector", "Tree Generator") { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Start", "S", "Growth starting point.", GH_ParamAccess.item);
            pManager.AddPointParameter("Attractors", "A", "Attractors where the branches will grow.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Iterations", "I", "Number of growth iterations.", GH_ParamAccess.item, 50);
            pManager.AddIntegerParameter("Trunk Height", "TH", "", GH_ParamAccess.item, 20);
            pManager.AddIntegerParameter("Minimum Distance", "mD", "", GH_ParamAccess.item, 2);
            pManager.AddIntegerParameter("Maximum Distance", "MD", "", GH_ParamAccess.item, 15);
            pManager.AddIntegerParameter("Branch Lenght", "BL", "", GH_ParamAccess.item, 2);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Branches", "B", "Tree branches.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d startingPoint = new Point3d();
            List<Point3d> attractors = new List<Point3d>();
            int iterationsCount = 0;
            int trunkHeight = 0;
            int minDistance = 0;
            int maxDistance = 0;
            int branchLenght = 0;

            DA.GetData(0, ref startingPoint);
            DA.GetDataList(1, attractors);
            DA.GetData(2, ref iterationsCount);
            DA.GetData(3, ref trunkHeight);
            DA.GetData(4, ref minDistance);
            DA.GetData(5, ref maxDistance);
            DA.GetData(6, ref branchLenght);


            Tree tree = new Tree(new Vector2d(), attractors);
            tree.TrunkHeight = trunkHeight;
            tree.MinDistance = minDistance;
            tree.MaxDistance = maxDistance;
            tree.BranchLength = branchLenght;

            for (int i = 0; i < iterationsCount; i++)
                tree.Grow();


            List<Line> treeAsLines = new List<Line>();
            foreach (Branch br in tree.Branches.Values)
            {
                if (br.Parent == null)
                    continue;

                Line ln = new Line(br.Position.X, br.Position.Y, 0, br.Parent.Position.X, br.Parent.Position.Y, 0);
                treeAsLines.Add(ln);
            }

            DA.SetDataList(0, treeAsLines);

        }

        protected override System.Drawing.Bitmap Icon { get { return null; } }
        public override Guid ComponentGuid { get { return new Guid("F0761A5B-6F92-4574-AAB9-7989029223A4"); } }
    }
}