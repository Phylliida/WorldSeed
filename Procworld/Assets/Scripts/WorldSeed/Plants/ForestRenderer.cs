using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldSeed.Plants
{
    public class ForestRenderer
    {
        public static void DrawForest(Material mat, Matrix4x4 localToWorldMat, Forest forest)
        {
            GL.PushMatrix();
            GL.MultMatrix(localToWorldMat);
            mat.SetPass(0);
            forest.plants.ForEach(plant => DrawPlant(mat, plant));
            GL.PopMatrix();
        }

        public static void DrawPlant(Material mat, Plant plant)
        {
            GL.Begin(GL.LINES);
            GL.Color(Color.green);
            DrawConnectionsOut(plant.root.root);
            GL.End();
        }

        public static void DrawConnectionsOut(Node node)
        {
            Vector3 myPos = Node.GetGlobalPos(node);
            if (node.main != null)
            {
                GL.Vertex(myPos);
                GL.Vertex(Node.GetGlobalPos(node.main));
                DrawConnectionsOut(node.main);
            }
            if (node.lateral != null)
            {
                GL.Vertex(myPos);
                GL.Vertex(Node.GetGlobalPos(node.lateral));
                DrawConnectionsOut(node.lateral);
            }
        }
    }

}
