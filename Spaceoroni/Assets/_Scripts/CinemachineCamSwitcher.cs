using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

    private List<CinemachineVirtualCamera> IntroSceneCams;


    private IntroSceneManager IntroSceneManager;

    void Start()
    {
        IntroSceneManager = GameObject.Find("IntroSceneManager").GetComponent<IntroSceneManager>();
        IntroSceneCams = new List<CinemachineVirtualCamera>();
        var cams = GameObject.FindGameObjectsWithTag("IntroVCam");
        foreach (var c in cams) IntroSceneCams.Add(c.GetComponent<CinemachineVirtualCamera>());
    }

    private void ResetAllPriorities()
    {
        foreach (var c in IntroSceneCams) { c.Priority = 1; }
        TruckFollowVCam.Priority = 1;
        TruckDoorVCam.Priority = 1;
        SolarSystemCam.Priority = 1;
        CenterEarthCamera.Priority = 1;
        EndOfGameCamera.Priority = 1;
        BoardCamera.Priority = 1;
        SpaceCamera.Priority = 1;
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

    public IEnumerator MoveToFollowTruck()
    {
        ResetAllPriorities();
        TruckFollowVCam.Priority = 2;
        IntroSceneManager.MoveTruck();
        yield return new WaitForSeconds(4);
        yield return StartCoroutine(MoveToTruckDoor());
    }

    public IEnumerator MoveToTruckDoor()
    {
        ResetAllPriorities();
        TruckDoorVCam.Priority = 2;
        yield return new WaitForSeconds(4);
        MoveToStart();
    }

    private IEnumerator moveThroughSolarSystem()
    {
        foreach (var c in IntroSceneCams)
        {
            ResetAllPriorities();
            c.Priority = 2;
            yield return new WaitForSeconds(3);
        }
        yield return StartCoroutine(MoveToFollowTruck());
    }

    public void startIntroScene()
    {
        StartCoroutine(moveThroughSolarSystem());
    }
}
