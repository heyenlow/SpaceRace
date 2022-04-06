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
    private CinemachineVirtualCamera BoardCameraHigh;
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
    private Elon ElonAnimator;
    [SerializeField]
    private CinemachineVirtualCamera JeffTextVCam;
    [SerializeField]
    private Jeff JeffAnimator;
    protected TextMeshProUGUI turnText;

    [SerializeField]
    private AudioSource panNoise;
    
    //private MoveTruck Truck;

    private List<CinemachineVirtualCamera> IntroSceneCams;

    private bool introRunning = false;

    void Start()
    {
        IntroSceneCams = new List<CinemachineVirtualCamera>();
        //Truck = GameObject.Find("CyberTruck").GetComponentInChildren<MoveTruck>();
        var cams = GameObject.FindGameObjectsWithTag("IntroVCam");
        foreach (var c in cams) IntroSceneCams.Add(c.GetComponent<CinemachineVirtualCamera>());
        turnText = GameObject.FindGameObjectWithTag("TurnText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && introRunning)
        {
            turnText.text = "";
            JeffAnimator.runAnimation();
            ElonAnimator.runGetOutOfCarAnimation();
            ResetAllPriorities();
            introRunning = false;
            SpaceCamera.Priority = 2;
        }
    }

    private void ResetAllPriorities()
    {
        foreach (var c in IntroSceneCams) { c.Priority = 1; }
        //reset all cameras priorities to 1;
            BoardCameraHigh.Priority = 1;
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
    public void MoveToGameBoardHigh()
    {
        ResetAllPriorities();
        BoardCameraHigh.Priority = 2;
    }
    public void MoveToMainMenu()
    {
        panNoise.Play();

        if (introRunning) introRunning = false;
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
        panNoise.Play();
        introRunning = true;
        turnText.text = "Press Space to Skip Intro";
        StartCoroutine(moveThroughSolarSystem());
    }

    private IEnumerator moveThroughSolarSystem()
    {
        foreach (var c in IntroSceneCams)
        {
            if (introRunning) ResetAllPriorities();
            if (introRunning) c.Priority = 2;
            if (introRunning) yield return new WaitForSeconds(2.5f);
        }
        if (introRunning) StartCoroutine(MoveToElonText());
    }

    private IEnumerator MoveToFollowTruck()
    {
        if (introRunning) ResetAllPriorities();
        if (introRunning) TruckFollowVCam.Priority = 2;
        if (introRunning) yield return new WaitForSeconds(3f);
        //if (introRunning) Truck.Move();
    }

    private IEnumerator MoveToTruckDoor()
    {
        if (introRunning) ResetAllPriorities();
        if (introRunning) TruckDoorVCam.Priority = 2;
        if (introRunning) yield return new WaitForSeconds(6);
        if (introRunning) yield return StartCoroutine(MoveToElonText());
    }

    public IEnumerator MoveToElonText()
    {
        if (introRunning) ResetAllPriorities();
        if (introRunning) ElonTextVCam.Priority = 2;
        if (introRunning) yield return new WaitForSeconds(1.3f);
        if (introRunning) ElonAnimator.runGetOutOfCarAnimation();
        if (introRunning) yield return new WaitForSeconds(3);
        if (introRunning) yield return StartCoroutine(MoveToJeffText());
    }
    private IEnumerator MoveToJeffText()
    {
        if (introRunning) ResetAllPriorities();
        if (introRunning) JeffTextVCam.Priority = 2;
        if (introRunning) yield return new WaitForSeconds(1.3f);
        if (introRunning) JeffAnimator.runAnimation();
        if (introRunning) yield return new WaitForSeconds(3);
        if (introRunning) turnText.text = "";
        if (introRunning) MoveToMainMenu();
    }
}
