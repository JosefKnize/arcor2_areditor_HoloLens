using Base;
using Hololens;
using IO.Swagger.Model;
using QRTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExperimentManager : Base.Singleton<ExperimentManager>
{
    private Vector3 lastPosition;
    private float totalDistance;
    private DateTime startTime;

    private GameObject refDobotM1;
    private GameObject refDobotMagician;
    private GameObject refConveyorBelt;

    public GameObject TrackedCamera;
    public GameObject SceneOrigin;

    public GameObject RobotPrefab;

    public bool Running { get; set; } = false;
    public bool DisplayModels { get; private set; } = true;

    private bool ghostRobotsCreated = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Running)
        {
            totalDistance += Vector3.Distance(TrackedCamera.transform.position, lastPosition);
            lastPosition = TrackedCamera.transform.position;
        }
    }

    public void StartExperiment()
    {
        Running = true;
        lastPosition = TrackedCamera.transform.position;
        startTime = DateTime.Now;

        if (!ghostRobotsCreated)
        {
            // Add invisible robots in scene Vector3(Dop�edu/Dozadu, Nahoru/Dolu, Doleva/Doprava)
            refDobotM1 = CreateGhostRobot("DobotM1", new Vector3(-0.56f, 0, 0.56f), new Vector3(0, 135f, 0));
            refDobotMagician = CreateGhostRobot("DobotMagician", new Vector3(-0.28f, 0.141f, -0.44f), new Vector3(0, 0, 0));
            refConveyorBelt = CreateGhostConveyorBelt("ConveyorBelt", new Vector3(-0.295f, 0, 0.145f), new Vector3(0, -90, 0));
            //refDobotM1 = CreateGhostRobot("DobotM1", new Vector3(-0.5553f, 0, 0.5556f), new Vector3(0, 135f, 0));             
            //refDobotMagician = CreateGhostRobot("DobotMagician", new Vector3(-0.3285f, 0.141f, -0.4535f), new Vector3(0, 0, 0)); 
            //refConveyorBelt = CreateGhostConveyorBelt("ConveyorBelt", new Vector3(-0.3475f, 0, 0.1150f), new Vector3(0, -90, 0));
            //refDobotM1 = CreateGhostRobot("DobotM1", new Vector3(-0.571f, 0, 0.569f), new Vector3(0, 134.81f, 0));
            //refDobotMagician = CreateGhostRobot("DobotMagician", new Vector3(-0.279f, 0.141f, -0.443f), new Vector3(0, 0, 0));
            //refConveyorBelt = CreateGhostConveyorBelt("ConveyorBelt", new Vector3(-0.273f, 0, 0.140f), new Vector3(0, -90, 0));
            ghostRobotsCreated = true;
        }
    }

    GameObject CreateGhostConveyorBelt(string type, Vector3 scenePosition, Vector3 rotation)
    {
        var gameObject = new GameObject($"GhostRobot_ConveyorBelt");
        gameObject.transform.parent = SceneOrigin.transform;
        gameObject.transform.localEulerAngles = rotation;
        gameObject.transform.localPosition = scenePosition;

        if (DisplayModels)
        {
            MeshImporterH.Instance.OnMeshImported += OnModelLoaded;
            var actionObject = ActionsManagerH.Instance.ActionObjectsMetadata.Values.First(x => x.Type == type);
            MeshImporterH.Instance.LoadModel(actionObject.ObjectModel.Mesh, actionObject.Type);
        }

        return gameObject;
    }

    private void OnModelLoaded(object sender, ImportedMeshEventArgsH args)
    {
        args.RootGameObject.gameObject.transform.parent = refConveyorBelt.transform;

        args.RootGameObject.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        args.RootGameObject.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        MeshImporterH.Instance.OnMeshImported -= OnModelLoaded;
    }

    GameObject CreateGhostRobot(string type, Vector3 scenePosition, Vector3 rotation)
    {
        var gameObject = new GameObject($"GhostRobot_{type}");
        gameObject.transform.parent = SceneOrigin.transform;
        gameObject.transform.localEulerAngles = rotation;
        gameObject.transform.localPosition = scenePosition;

        if (DisplayModels)
        {
            if (ActionsManagerH.Instance.RobotsMeta.TryGetValue(type, out RobotMeta robotMeta))
            {
                RobotModelH robotModel = UrdfManagerH.Instance.GetRobotModelInstance(robotMeta.Type, robotMeta.UrdfPackageFilename);
                robotModel.RobotModelGameObject.gameObject.transform.parent = gameObject.transform;
                robotModel.RobotModelGameObject.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                robotModel.RobotModelGameObject.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                robotModel.RobotModelGameObject.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                robotModel.SetActiveAllVisuals(true);
            }
            else
            {
                throw new Exception("Experiment manager couldn't create ghost robot");
            }
        }

        return gameObject;
    }

    public void StopExperiment()
    {
        Running = false;
        var time = DateTime.Now - startTime;

        // Measure distance in robots

        var conveyor_belt = GameObject.Find("conveyor_belt");
        var dobot_magician = GameObject.Find("dobot_magician");
        var dobot_m1 = GameObject.Find("dobot_m1");

        float DobotM1_distance = Vector3.Distance(refDobotM1.transform.position, dobot_m1.transform.position);
        float DobotM1_angleDifference = Mathf.Abs(dobot_m1.transform.localEulerAngles.y - refDobotM1.transform.localEulerAngles.y);
        DobotM1_angleDifference = (DobotM1_angleDifference > 180f) ? 360f - DobotM1_angleDifference : DobotM1_angleDifference;

        float DobotMagician_distance = Vector3.Distance(refDobotMagician.transform.position, dobot_magician.transform.position);
        float DobotMagician_angleDifference = Mathf.Abs(dobot_magician.transform.localEulerAngles.y - refDobotMagician.transform.localEulerAngles.y);
        DobotMagician_angleDifference = (DobotMagician_angleDifference > 180f) ? 360f - DobotMagician_angleDifference : DobotMagician_angleDifference;

        float ConveyorBelt_distance = Vector3.Distance(refConveyorBelt.transform.position, conveyor_belt.transform.position);
        float ConveyorBelt_angleDifference = Mathf.Abs(conveyor_belt.transform.localEulerAngles.y - refConveyorBelt.transform.localEulerAngles.y);
        ConveyorBelt_angleDifference = (ConveyorBelt_angleDifference > 180f) ? 360f - ConveyorBelt_angleDifference : ConveyorBelt_angleDifference;

        float averageDistance = (DobotM1_distance + DobotMagician_distance + ConveyorBelt_distance) / 3;
        float averageAngleDifference = (DobotM1_angleDifference + DobotMagician_angleDifference + ConveyorBelt_angleDifference) / 3;

        string filePath = Path.Combine(Application.persistentDataPath, $"Experiment_{DateTime.Now.ToString("dd.MM_HH.mm")}.txt");

        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        File.WriteAllText(filePath, $"Time: {time}\nDistance: {totalDistance}\nRobot position error: {averageDistance}\nRobot rotation error: {averageAngleDifference}\nDobotM1: {dobot_m1.transform.localPosition}{dobot_m1.transform.localEulerAngles}\nConveyorBelt: {conveyor_belt.transform.localPosition}{conveyor_belt.transform.localEulerAngles}\nDobotMagician: {dobot_magician.transform.localPosition}{dobot_magician.transform.localEulerAngles}");
    }
}
