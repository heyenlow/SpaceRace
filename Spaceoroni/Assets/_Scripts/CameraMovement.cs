using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    ////HAS BEEN MOVED TO CHINEMACHINECAMSWITCHER.CS
//    [SerializeField]
//    Transform GameBoardLocation;
//    [SerializeField]
//    Transform StartScreenLocation;
//    [SerializeField]
//    Transform StartScreenLocationCenter;
//    [SerializeField]
//    Transform PostGameScreenPerspective;

//    Vector3 newLocation;
//    Quaternion newRotation;
//    [SerializeField]
//    private int MovementSpeed = 35;
//    [SerializeField]
//    private int RotationSpeed = 10;
//    GameObject movingCamera;

//    void Update()
//    {
//        if (movingCamera != null)
//        {
//            movingCamera.transform.position = Vector3.MoveTowards(movingCamera.transform.position, newLocation, MovementSpeed * Time.deltaTime);  
//            movingCamera.transform.rotation = Quaternion.RotateTowards(movingCamera.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
            
//            //if (movingCamera.transform.position == newLocation && movingCamera.transform.rotation == newRotation) movingCamera = null;
//        }
//    }

//    public void moveCameraToGameBoard()
//    {
//        newLocation = GameBoardLocation.position;
//        newRotation = GameBoardLocation.rotation;
//        movingCamera = this.gameObject;
//    }

//    public void moveCameraToCenterSpace()
//    {
//        newLocation = StartScreenLocationCenter.position;
//        newRotation = StartScreenLocationCenter.rotation;
//        movingCamera = this.gameObject;
//    }
//    public void moveCameraToStart()
//    {
//        newLocation = StartScreenLocation.position;
//        newRotation = StartScreenLocation.rotation;
//        movingCamera = this.gameObject;
//    }
//    public void moveCameraToPostGame()
//    {
//        newLocation = PostGameScreenPerspective.position;
//        newRotation = PostGameScreenPerspective.rotation;
//        movingCamera = this.gameObject;
//    }
}
