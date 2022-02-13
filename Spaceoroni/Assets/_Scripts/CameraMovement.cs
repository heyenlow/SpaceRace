using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField]
    Transform GameBoardLocation;
    [SerializeField]
    Transform StartScreenLocation;
   
    /*old
    Vector3 EntryScreenCameraPostition = new Vector3((float)6.71000004, (float)5.44999981, (float)-8.80999947);
    Quaternion EntryScreenCameraAngle = new Quaternion((float)0, (float)0, (float)0, (float)1);
    Vector3 gbca = new Vector3((float)40.4000015, (float)284.269989, (float)0.00000224223459);
    Vector3 GameBoardCameraPostion = new Vector3((float)7.5999999, (float)5.44000006, (float)0.589999974);
    Quaternion GameBoardCameraAngle = new Quaternion((float)0.272598833, (float)-0.576049924, (float)0.211945131, (float)0.740901887);
    */

    Vector3 newLocation;
    Quaternion newRotation;
    [SerializeField]
    private int MovementSpeed = 35;
    [SerializeField]
    private int RotationSpeed = 10;
    GameObject movingCamera;

    void Start()
    {
        //this.transform.position = EntryScreenCameraPostition;
        //this.transform.rotation = EntryScreenCameraAngle;

        //gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        //GameBoardLocation = GameObject.Find("GameBoardCameraLocation").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingCamera != null)
        {
            movingCamera.transform.position = Vector3.MoveTowards(movingCamera.transform.position, newLocation, MovementSpeed * Time.deltaTime);  
            movingCamera.transform.rotation = Quaternion.RotateTowards(movingCamera.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
            
            if (movingCamera.transform.position == newLocation) movingCamera = null;
        }
    }

    public void moveCameraFromStartScreenToGameBoard()
    {
        newLocation = GameBoardLocation.position;
        newRotation = GameBoardLocation.rotation;
        movingCamera = this.gameObject;
    }
}
