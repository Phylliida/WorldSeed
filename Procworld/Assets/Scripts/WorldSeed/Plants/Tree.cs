using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldSeed.Plants
{
    public class Plant
    {
        public PlantConfig plantConfig;
        public Module root;
        public List<Module> modules;
        public Plant(PlantConfig plantConfig, ModulePrototype startModule, Pose rootPose)
        {
            this.plantConfig = plantConfig;
            this.root = ModulePrototype.CreateInstance(this, startModule, rootPose);
            this.modules = new List<Module>();
            this.modules.Add(this.root);
        }
    }
}