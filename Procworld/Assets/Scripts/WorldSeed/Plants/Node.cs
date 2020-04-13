using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WorldSeed.Plants
{
    public class Node
    {
        public Node parent;
        public Node main;
        public Node lateral;
        public Node copyOfMe;
        public Vector3 pos;
        public Module module;
        public float q;
        public float v;

        public Node(Node parent, Vector3 pos)
        {
            this.parent = parent;
            this.pos = pos;
        }

        public static Node MemberwiseClone(Node node)
        {
            return (Node)node.MemberwiseClone();
        }

        public static float TrickleQ(Node node)
        {
            node.q = 0;
            // leaf nodes get q from sun
            if (node.main == null && node.lateral == null)
            {
                node.q = node.module.qFromSun;
            }
            if (node.main != null)
            {
                node.q += TrickleQ(node.main);
            }
            if (node.lateral != null)
            {
                node.q += TrickleQ(node.lateral);
            }
            return node.q;
        }

        // Remember to clamp to vrootmax before doing this
        public static float TrickleV(Node node, float lambda)
        {
            if (node.main != null && node.lateral != null)
            {
                node.main.v = node.v * (lambda * node.main.q) /
                                        (lambda * node.main.q + (1 - lambda) * node.lateral.q);
                node.lateral.v = node.v - node.main.v;
            }
            else if (node.main == null && node.lateral != null)
            {
                node.lateral.v = node.v;
            }
            else if (node.main != null && node.lateral == null)
            {
                node.main.v = node.v;
            }
            if (node.main != null)
                TrickleV(node.main, lambda);
            if (node.lateral != null)
                TrickleV(node.lateral, lambda);

            return node.v;
        }

        public static Vector3 GetGlobalPos(Node node)
        {
            return Module.LocalPosToGlobalPos(node.module.pose, node.pos);
        }

        /// <summary>
        /// Gets a pose with forward pointing away from this node's edge connected to this node's parent
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Pose GetPose(Node node)
        {
            Vector3 forward = Vector3.up;
            if (node.parent != null)
            {
                forward = (node.pos - node.parent.pos).normalized;
            }
            return new Pose(Module.LocalPosToGlobalPos(node.module.pose, node.pos),
                node.module.pose.rotation * Quaternion.LookRotation(forward)); // this order is correct because second is in reference frame of first
        }
    }
}