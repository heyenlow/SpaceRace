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
    private CinemachineVirtualCamera BoardCameraOther;
    [SerializeField]
    private CinemachineVirtualCamera BoardCameraHighOther;
    [SerializeField]
    private CinemachineVirtualCamera EndOfGameCamera;
    [SerializeField]
    private CinemachineVirtualCamera CenterEarthCamera;
    [SerializeField]
    private CinemachineVirtualCamera SolarSystemCam;


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

    public bool textFocused = false;
    public void setTextFocused(bool inFocus)
    {
        textFocused = inFocus;
    }

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

    enum GameCameraLocations
    {
        main,
        mainHigh,
        other,
        otherHigh,
        none
    };

    GameCameraLocations CameraGameBoardPosition = GameCameraLocations.none;

    private void Update()
    {
        if (Input.GetKeyDown("space") && introRunning )
        {
            panNoise.Play();
            turnText.text = "";
            JeffAnimator.runAnimation();
            ElonAnimator.runGetOutOfCarAnimation();
            ResetAllPriorities();
            introRunning = false;
            SpaceCamera.Priority = 2;
        }

        if (Input.GetKeyDown("space") && CameraGameBoardPosition != GameCameraLocations.none && !textFocused)
        {
            switch (CameraGameBoardPosition)
            {
                case GameCameraLocations.main:
                    MoveToGameBoardOther();
                    break;
                case GameCameraLocations.mainHigh:
                    MoveToGameBoardHighOther();
                    break;
                case GameCameraLocations.other:
                    MoveToGameBoard();
                    break;
                case GameCameraLocations.otherHigh:
                    MoveToGameBoardHigh();
                    break;
            }
        }
    }

    private void ResetAllPriorities()
    {
        foreach (var c in IntroSceneCams) { c.Priority = 1; }
        //reset all cameras priorities to 1;
            BoardCameraHigh.Priority = 1;
            SolarSystemCam.Priority = 1;
            BoardCameraOther.Priority = 1;
            CenterEarthCamera.Priority = 1;
            EndOfGameCamera.Priority = 1;
            BoardCamera.Priority = 1;
            if(!introRunning) SpaceCamera.Priority = 1;
            BoardCameraHighOther.Priority = 1;
            ElonTextVCam.Priority = 1;
            JeffTextVCam.Priority = 1;
    }

    public void moveToHigh()
    {
        switch (CameraGameBoardPosition)
        {
            case GameCameraLocations.main:
                MoveToGameBoardHigh();
                break;
            case GameCameraLocations.other:
                MoveToGameBoardHighOther();
                break;
        }
    }

    public void moveToLow()
    {
        switch (CameraGameBoardPosition)
        {
            case GameCameraLocations.mainHigh:
                MoveToGameBoard();
                break;
            case GameCameraLocations.otherHigh:
                MoveToGameBoardOther();
                break;
        }
    }

    public void MoveToGameBoard()
    {
        CameraGameBoardPosition = GameCameraLocations.main;
        ResetAllPriorities();
        BoardCamera.Priority = 2;
    }
    public void MoveToGameBoardOther()
    {
        CameraGameBoardPosition = GameCameraLocations.other;

        ResetAllPriorities();
        BoardCameraOther.Priority = 2;
    }
    public void MoveToGameBoardHigh()
    {
        CameraGameBoardPosition = GameCameraLocations.mainHigh;

        ResetAllPriorities();
        BoardCameraHigh.Priority = 2;
    }
    public void MoveToGameBoardHighOther()
    {
        CameraGameBoardPosition = GameCameraLocations.otherHigh;

        ResetAllPriorities();
        BoardCameraHighOther.Priority = 2;
    }
    public void MoveToMainMenu()
    {
        panNoise.Play();
        CameraGameBoardPosition = GameCameraLocations.none;

        if (introRunning) introRunning = false;
        ResetAllPriorities();
        SpaceCamera.Priority = 2;
    }
    public void MoveToEnd()
    {
        CameraGameBoardPosition = GameCameraLocations.none;

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
