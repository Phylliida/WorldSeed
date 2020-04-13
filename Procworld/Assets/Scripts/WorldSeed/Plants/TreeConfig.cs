using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldSeed.Plants
{
    public struct PlantConfig
    {
        public float lambda;
        public float vigorRootMax;
        public float vigorRootMin;
        public float growthRate; // gP
        public Vector3 tropismEulerAngles; // TODO: do this same as them
        public float weightCollisions; // w1
        public float weightTropism; // w2
        public float D;
        public List<ModulePrototype> modulePrototypes;
        public static ModulePrototype ClosestPrototype(PlantConfig plantConfig, float lambda, float D)
        {
            if (plantConfig.modulePrototypes == null) throw new System.ArgumentNullException("plantConfig.modulePrototypes");
            return plantConfig.modulePrototypes.MinBy(modulePrototype => ModulePrototype.SquaredDist(modulePrototype, lambda, D));
        }
    }
}