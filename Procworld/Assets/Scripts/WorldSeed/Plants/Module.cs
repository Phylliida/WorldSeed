using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

namespace WorldSeed.Plants
{

    public class Module
    {
        public Plant plant;
        public Node root;
        public List<Node> nodes;
        public List<Module> children;
        public Module parentModule;
        public Pose pose;
        public Pose nextPose;
        public float qFromSun;
        public BoundingSphere boundingSphere;
        public float physiologicalAge = 0.0f;
        public bool isShed = false;
        public Vector3 furthestA;
        public Vector3 furthestB;
        public Module(Plant plant, Node root, List<Node> nodes, Pose pose)
        {
            this.plant = plant;
            this.root = root;
            this.nodes = nodes;
            this.pose = pose;
        }

        public static void UpdateFurthestPoints(Module module)
        {
            float minDist = float.MaxValue;
            module.furthestA = Vector3.zero;
            module.furthestB = Vector3.zero;
            foreach (Node a in module.nodes)
            {
                foreach (Node b in module.nodes)
                {
                    float dist = Vector3.Distance(a.pos, b.pos);
                    if (dist < minDist)
                    {
                        module.furthestA = a.pos;
                        module.furthestB = b.pos;
                        minDist = dist;
                    }
                }
            }
        }

        public static Vector3 LocalPosToGlobalPos(Pose pose, Vector3 localPos)
        {
            return pose.position + (pose.rotation * localPos);
        }

        public static BoundingSphere GetBoundingSphere(Module module, Vector3 furthestA, Vector3 furthestB, Pose pose)
        {
            return new BoundingSphere(module, LocalPosToGlobalPos(pose, VectorUtil.Midpoint(furthestA, furthestB)), Vector3.Distance(furthestA, furthestB) / 2.0f);
        }

        public static float ComputeFDistribution(Module module, Pose pose, ParallelQuery<BoundingSphere> allSpheres)
        {
            BoundingSphere updatedBoundingSphere = Module.GetBoundingSphere(module, module.furthestA, module.furthestA, pose);
            float fCollision = IntersectionUtil.GetVolumeOfIntersection(updatedBoundingSphere, allSpheres);
            Vector3 rot = pose.rotation.eulerAngles * Deg2Rad;
            Vector3 tropismAngle = module.plant.plantConfig.tropismEulerAngles * Deg2Rad;
            float fTropism = Vector3.Magnitude(
                new Vector3(Cos(tropismAngle.x), Cos(tropismAngle.y), Cos(tropismAngle.z)) -
                new Vector3(Cos(rot.x), Cos(rot.y), Cos(rot.z)));
            return module.plant.plantConfig.weightCollisions * fCollision + module.plant.plantConfig.weightTropism * fTropism;
        }
    }
}
  