using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

namespace WorldSeed.Plants
{
    public class Forest
    {
        int debugLevel;
        public List<Plant> plants;
        public Forest(List<Plant> plants, int debugLevel)
        {
            this.plants = plants;
            this.debugLevel = debugLevel;
        }

        public void UpdateForest(int optimizationSteps, float optimizationEps, float dt)
        {
            var allPlants = ParallelEnumerable.AsParallel(plants);
            var allModules = allPlants.SelectMany(plant => plant.modules); // this merges all the modules into a single list
            UpdateBoundingSpheres(allModules);
            UpdateLight(allModules);
            UpdateVigor(allPlants);
            OptimizeRotation(allModules, optimizationSteps, optimizationEps);
            UpdatePhysiologicalAge(allModules, dt);
            GrowNewBranches(allModules);
            // Do this at the end since it removes some modules (so allModules won't be right)
            ShedBranches(allPlants, allModules);
        }

        public static void UpdateBoundingSpheres(ParallelQuery<Module> allModules)
        {
            allModules.ForAll(module =>
            {
                Module.UpdateFurthestPoints(module);
                module.boundingSphere = Module.GetBoundingSphere(module, module.furthestA, module.furthestB, module.pose);
            });
        }

        public static void UpdateLight(ParallelQuery<Module> allModules)
        {
            var allSpheres = allModules.Select(module => module.boundingSphere);
            allModules.ForAll(module =>
            {
                if (!module.isShed) module.qFromSun = PlantUtil.CalculateLight(module.boundingSphere, allSpheres);
                else module.qFromSun = 0;
            });
        }

        public static void UpdateVigor(ParallelQuery<Plant> allPlants)
        {
            allPlants.ForAll(plant =>
            {
                // trickle light from leaves down to root
                Node.TrickleQ(plant.root.root);
                // clamp vigior at vigor root max
                plant.root.root.v = Min(plant.root.root.q, plant.plantConfig.vigorRootMax);
                // trickle vigor from root out to leaves, according to extended Borchert-Honda model
                Node.TrickleV(plant.root.root, plant.plantConfig.lambda);

                Log("Vigor for plant is " + plant.root.root.v);
            });
        }

        public static void UpdatePhysiologicalAge(ParallelQuery<Module> allModules, float dt)
        {
            allModules.ForAll(module =>
            {
                module.physiologicalAge += dt * PlantUtil.ChangeInPhysiologicalAge(module.root.v,
                    module.plant.plantConfig.vigorRootMin,
                    module.plant.plantConfig.vigorRootMax,
                    module.plant.plantConfig.growthRate);
            });
        }

        public static void OptimizeRotation(ParallelQuery<Module> allModules, int optimizationSteps, float eps)
        {
            var allSpheres = allModules.Select(module => module.boundingSphere);
            for (int step = 0; step < optimizationSteps; step++)
            {
                // Find a better orientation
                allModules.ForAll(module =>
                {
                    float lowestFDistr = Module.ComputeFDistribution(module, module.pose, allSpheres);
                    Pose betterPose = module.pose;
                    for (int k = 0; k < 4; k++)
                    {
                        // This does euler angles (eps, 0, 0), (-eps, 0, 0), (0, 0, eps), (0, 0, -eps)
                        float rotAngle = eps;
                        if (k % 2 == 1)
                        {
                            rotAngle = -eps;
                        }
                        Quaternion rot = Quaternion.Euler(rotAngle, 0, 0);
                        if (k / 2 == 1)
                        {
                            rot = Quaternion.Euler(0, 0, rotAngle);
                        }
                        Pose nextPose = new Pose(module.pose.position, module.pose.rotation * rot); // according to https://answers.unity.com/questions/1353333/how-to-add-2-quaternions.html, multiplying in this order means that rot will be relative to the reference frame of module.orientation.rotation, so x axis is right, y axis is up, z axis is forward. We should double check this.
                        float fDistribution = Module.ComputeFDistribution(module, nextPose, allSpheres);
                        if (fDistribution < lowestFDistr)
                        {
                            betterPose = nextPose;
                            lowestFDistr = fDistribution;
                        }
                    }
                    module.nextPose = betterPose;
                });

                // Update orientation
                allModules.ForAll(module =>
                {
                    module.pose = module.nextPose;
                });
            }
        }

        public static void GrowNewBranches(ParallelQuery<Module> allModules)
        {
            // SelectMany will join all of the new modules returned by each module (if any) into a single list
            allModules.SelectMany(module =>
            {
                return module.nodes
                    // Grow module off of it if not grown and vigor is greater than vigorRootMins
                    .Where(node => node.main == null && node.lateral == null && node.v > module.plant.plantConfig.vigorRootMin)
                    .Select(node =>
                    {
                        UnityEngine.Debug.Log("Growing new branch at pos " + Node.GetGlobalPos(node));
                        float lambda = module.plant.plantConfig.lambda;
                        float dPrime = module.root.v * module.plant.plantConfig.D / module.plant.plantConfig.vigorRootMax;

                        ModulePrototype closestPrototype = PlantConfig.ClosestPrototype(module.plant.plantConfig, lambda, dPrime);
                        Module newModule = ModulePrototype.CreateInstance(module.plant, closestPrototype, Node.GetPose(node));
                        node.main = newModule.root; // connect main to root so the trickle of q and v can work properly
                        newModule.root.parent = node.main;
                        return newModule;
                    });
            })
            // Do last piece not in parallel, since we are appending to lists that is not thread safe
            .ToList()
            .ForEach(newModule =>
            {
                newModule.plant.modules.Add(newModule);
            });
        }

        public static void ShedBranches(ParallelQuery<Plant> allPlants, ParallelQuery<Module> allModules)
        {
            // Set as pruned
            allModules.ForAll(module =>
            {
                // Branches are shed if vigor is less than vigorRootMin
                if (module.root.v < module.plant.plantConfig.vigorRootMin)
                {
                    //module.isShed = true;
                }
            });

            // Remove all sheded modules from plants
            allPlants.ForAll(plant =>
            {
                plant.modules = ParallelEnumerable.AsParallel(plant.modules).Where(module => !module.isShed).ToList();
            });
        }
    }
}