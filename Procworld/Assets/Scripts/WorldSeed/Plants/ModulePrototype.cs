using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldSeed.Plants
{
    public struct ModulePrototype
    {
        public float lambda;
        public float D;
        public List<Node> nodes;
        public Node root;

        public static float SquaredDist(ModulePrototype prototype, float lambda, float D)
        {
            float dLambda = lambda - prototype.lambda;
            float dD = D - prototype.lambda;
            return dLambda * dLambda + dD * dD;
        }

        public static Module CreateInstance(Plant plant, ModulePrototype module, Pose pose)
        {
            // Make a copy of all the nodes
            module.nodes.ForEach(node => { node.copyOfMe = Node.MemberwiseClone(node); });
            // Assign references of the copies to the copied elements
            List<Node> copiedNodes = Enumerable.ToList(
                Enumerable.Select(module.nodes, (node =>
                {
                    node.copyOfMe.parent = GetCopyOfMeIfNotNull(node.parent);
                    node.copyOfMe.main = GetCopyOfMeIfNotNull(node.main);
                    node.copyOfMe.lateral = GetCopyOfMeIfNotNull(node.lateral);
                    return node.copyOfMe;
                })));

            Module result = new Module(plant, module.root.copyOfMe, copiedNodes, pose);
            result.nodes.ForEach(node =>
            {
                node.module = result;
            });
            return result;
        }

        static Node GetCopyOfMeIfNotNull(Node node)
        {
            return node != null ? node.copyOfMe : null;
        }
    }
}