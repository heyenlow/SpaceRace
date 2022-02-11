using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 EntryScreenCameraPostition = new Vector3((float)6.71000004, (float)5.44999981, (float)-8.80999947);
    Quaternion EntryScreenCameraAngle = new Quaternion((float)0, (float)0, (float)0, (float)1);
    Vector3 gbca = new Vector3((float)40.4000015, (float)284.269989, (float)0.00000224223459);
    Transform GameBoardLocation;
    Vector3 GameBoardCameraPostion = new Vector3((float)7.5999999, (float)5.44000006, (float)0.589999974);
    Quaternion GameBoardCameraAngle = new Quaternion((float)0.272598833, (float)-0.576049924, (float)0.211945131, (float)0.740901887);

    Vector3 newLocation;
    public int Speed = 10;
    Game gameController;
    GameObject movingCamera;

    void Start()
    {
        //this.transform.position = EntryScreenCameraPostition;
        //this.transform.rotation = EntryScreenCameraAngle;

        //TEST
        //GameObject.Find("Main Camera").transform.position = new Vector3((float)2.24000001, (float)4.86000013, (float)-2.77999997);
        //GameObject.Find("Main Camera").transform.rotation = new Quaternion((float)0.42261827, 0, 0, (float)0.906307876);

        //Testing gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        //Testing GameBoardLocation = GameObject.Find("GameBoardCameraLocation").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (movingCamera != null)
        {
            movingCamera.transform.position = Vector3.MoveTowards(movingCamera.transform.position, newLocation, Speed * Time.deltaTime);
            Debug.Log(movingCamera.transform.rotation + " -> " + GameBoardCameraAngle);
            movingCamera.transform.rotation = Quaternion.RotateTowards(movingCamera.transform.rotation, Quaternion.Euler(gbca.x, gbca.y,gbca.z) , Speed * Time.deltaTime);
            if (movingCamera.transform.position == newLocation) movingCamera = null;
        }
    }

    public void moveCamera()
    {
        newLocation = GameBoardCameraPostion;
        movingCamera = this.gameObject;
    }
}
