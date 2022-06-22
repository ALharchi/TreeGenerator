using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace TreeGenerator
{
    public class TreeGenerator2DComponent : GH_Component
    {
        public TreeGenerator2DComponent() : base("Tree Generator 2D", "Tree Generator 2D", "Generate a natural looking tree in 2D based on the Space Colonization Algorithm.", "Vector", "Tree Generator") { }
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("1A0C889E-FAD5-424B-A270-148E9E5ECD1F");

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

            if (!Point3d.ArePointsCoplanar(attractors, Rhino.RhinoMath.DefaultDistanceToleranceMillimeters))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Attractors should co-planar.");
                return;
            }    

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

    }
}