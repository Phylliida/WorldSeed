using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using WorldSeed.Plants;


public class ExamplePlants : MonoBehaviour
{
    public Forest forest;
    public Transform renderTransform;

    // Start is called before the first frame update
    void Start()
    {
        Node root1 = new WorldSeed.Plants.Node(null, Vector3.zero);
        Node stem1a = new WorldSeed.Plants.Node(root1, new Vector3(0, 1, 0));
        root1.main = stem1a;
        Node stem1b = new Node(stem1a, new Vector3(0, 2, 0));
        Node leaf1 = new Node(stem1a, new Vector3(-1, 2, 0));
        stem1a.main = stem1b;
        stem1a.lateral = leaf1;
        List<Node> nodes = new List<Node>()
        {
            root1,
            stem1a,
            stem1b,
            leaf1
        };

        ModulePrototype modulePrototype = new ModulePrototype()
        {
            D=0.0f,
            lambda=0.0f,
            root=root1,
            nodes=nodes
        };

        PlantConfig plantConfig = new PlantConfig()
        {
            //pMax=20,
            vigorRootMin = 0.01f,
            vigorRootMax = 42,
            growthRate = 0.19f,
            lambda = 0.62f,
            //lambdaMature=0.62f,
            D = 0.25f,
            //DMature=0.25f,
            //Fage = 0.0f,
            tropismEulerAngles = new Vector3(0.52f, 0.52f, 0.52f),
            weightCollisions = 0.63f,
            weightTropism = 1 - 0.63f,
            //g1=-0.38,
            //phi=0.57,
            //beta=0.47,
            modulePrototypes=new List<ModulePrototype>() { modulePrototype}
        };

        Plant testPlant = new WorldSeed.Plants.Plant(plantConfig, modulePrototype, new Pose(Vector3.zero, Quaternion.identity));
        forest = new Forest(new List<Plant>() { testPlant });
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnityEngine.Debug.Log("Updating forest");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            forest.UpdateForest(4, 1.0f, 0.1f);
            timer.Stop();
            UnityEngine.Debug.Log("Updated forest, took:" + timer.ElapsedMilliseconds + " millis");

            forest.plants.ForEach(plant =>
            {
                plant.modules.ForEach(module =>
                {
                    //UnityEngine.Debug.Log("Module at pos " + module.pose.position + " with psy age " + module.physiologicalAge + " light received " + module.qFromSun + " is shed " + module.isShed + " and parent " + module.parentModule);
                    module.nodes.ForEach(node =>
                    {
                        //UnityEngine.Debug.Log(Node.GetGlobalPos(node) + " "  + node.v + " " + node.q + " " + node.parent + " " + node.lateral + " " + node.main);
                    });
                });
            });
        }
    }

    Material mat = null;
    void OnRenderObject()
    {
        if (mat == null)
        {
            UnityEngine.Debug.Log("Making shader");
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
        }
        ForestRenderer.DrawForest(mat, renderTransform.localToWorldMatrix, forest);
    }
}
