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

    public void MoveToGameBoard()
    {
        BoardCamera.Priority = 2;
        SpaceCamera.Priority = 1;
        EndOfGameCamera.Priority = 1;
        CenterEarthCamera.Priority = 1;
    }
    public void MoveToStart()
    {
        SpaceCamera.Priority = 2;
        BoardCamera.Priority = 1;
        EndOfGameCamera.Priority = 1;
        CenterEarthCamera.Priority = 1;
    }
    public void MoveToEnd()
    {
        EndOfGameCamera.Priority = 2;
        BoardCamera.Priority = 1;
        SpaceCamera.Priority = 1;
        CenterEarthCamera.Priority = 1;
    }
    public void MoveToCenterEarth()
    {
        CenterEarthCamera.Priority = 2;
        EndOfGameCamera.Priority = 1;
        BoardCamera.Priority = 1;
        SpaceCamera.Priority = 1;
    }
}
