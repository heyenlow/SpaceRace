using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class CinemachineCamSwitcher : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera SpaceCamera;
    [SerializeField]
    private CinemachineVirtualCamera BoardCamera;
    [SerializeField]
    private CinemachineVirtualCamera EndOfGameCamera;
    [SerializeField]
    private CinemachineVirtualCamera CenterEarthCamera;
    [SerializeField]
    private CinemachineVirtualCamera SolarSystemCam;


    [SerializeField]
    private CinemachineVirtualCamera TruckFollowVCam;
    [SerializeField]
    private CinemachineVirtualCamera TruckDoorVCam;
    [SerializeField]
    private CinemachineVirtualCamera ElonTextVCam;
    [SerializeField]
    private CinemachineVirtualCamera JeffTextVCam;

    protected TextMeshProUGUI turnText;


    private MoveTruck Truck;

    private List<CinemachineVirtualCamera> IntroSceneCams;

    bool introRunning = false;


    void Start()
    {
        IntroSceneCams = new List<CinemachineVirtualCamera>();
        Truck = GameObject.Find("CyberTruck").GetComponentInChildren<MoveTruck>();
        var cams = GameObject.FindGameObjectsWithTag("IntroVCam");
        foreach (var c in cams) IntroSceneCams.Add(c.GetComponent<CinemachineVirtualCamera>());
        turnText = GameObject.FindGameObjectWithTag("TurnText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && introRunning)
        {
            turnText.text = "";
            SpaceCamera.Priority = 3;
        }
    }

    private void ResetAllPriorities()
    {
        foreach (var c in IntroSceneCams) { c.Priority = 1; }
        //reset all cameras priorities to 1;
            TruckFollowVCam.Priority = 1;
            TruckDoorVCam.Priority = 1;
            SolarSystemCam.Priority = 1;
            CenterEarthCamera.Priority = 1;
            EndOfGameCamera.Priority = 1;
            BoardCamera.Priority = 1;
            if(!introRunning) SpaceCamera.Priority = 1;
            ElonTextVCam.Priority = 1;
            JeffTextVCam.Priority = 1;
    }

    public void MoveToGameBoard()
    {
        ResetAllPriorities();
        BoardCamera.Priority = 2;
    }
    public void MoveToStart()
    {
        ResetAllPriorities();
        SpaceCamera.Priority = 2;
    }
    public void MoveToEnd()
    {
        ResetAllPriorities();
        EndOfGameCamera.Priority = 2;
    }
    public void MoveToCenterEarth()
    {
        ResetAllPriorities();
        CenterEarthCamera.Priority = 2;
    }

    public void MoveToSolarSystem()
    {
        ResetAllPriorities();
        SolarSystemCam.Priority = 2;
    }
    public void startIntroScene()
    {
        introRunning = true;
        turnText.text = "Press Space to Skip Intro";
        StartCoroutine(moveThroughSolarSystem());
    }

    private IEnumerator moveThroughSolarSystem()
    {
        foreach (var c in IntroSceneCams)
        {
            ResetAllPriorities();
            c.Priority = 2;
            yield return new WaitForSeconds(2.5f);
        }
        StartCoroutine(MoveToFollowTruck());
    }

    private IEnumerator MoveToFollowTruck()
    {
        ResetAllPriorities();
        TruckFollowVCam.Priority = 2;
        yield return new WaitForSeconds(3f);
        Truck.Move();
    }

    private IEnumerator MoveToTruckDoor()
    {
        ResetAllPriorities();
        TruckDoorVCam.Priority = 2;
        yield return new WaitForSeconds(6);
        yield return StartCoroutine(MoveToElonText());
    }

    public IEnumerator MoveToElonText()
    {
        ResetAllPriorities();
        ElonTextVCam.Priority = 2;
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(MoveToJeffText());
    }
    private IEnumerator MoveToJeffText()
    {
        ResetAllPriorities();
        JeffTextVCam.Priority = 2;
        yield return new WaitForSeconds(3);
        introRunning = false;
        turnText.text = "";
        MoveToStart();
    }
}
