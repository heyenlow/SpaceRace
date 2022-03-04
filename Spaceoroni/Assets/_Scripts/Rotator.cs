using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    public enum Axis
    {
        x,
        y,
        z,
        earth
    };
    
    public Axis RotateAxis;
    public float Speed = 0.015f;
    public GameObject CenterObject;

    void Update()
    {
        if (CenterObject == null)
        {
            rotateObject();
        }
        else
        {
            rotateAroundCenterObject();
        }
    }

    private void rotateObject()
    {
        switch (RotateAxis)
        {
            case Axis.x:
                this.transform.Rotate( Speed, 0f, 0f);
                break;
            case Axis.y:
                this.transform.Rotate(0f, Speed, 0f);
                break;
            case Axis.z:
                this.transform.Rotate(0f, 0f, Speed);
                break;
            case Axis.earth:
                this.transform.Rotate(Speed, 0f, -1 * Speed);
                break;

        }

    }
    private void rotateAroundCenterObject()
    {

        switch (RotateAxis)
        {
            case Axis.x:
                this.transform.RotateAround(CenterObject.transform.position, new Vector3(Speed, 0f, 0f), Speed);
                break;
            case Axis.y:
                this.transform.RotateAround(CenterObject.transform.position, new Vector3(0f, Speed, 0f), Speed);
                break;
            case Axis.z:
                this.transform.RotateAround(CenterObject.transform.position, new Vector3(0f, 0f, Speed), Speed);
                break;
            case Axis.earth:
                this.transform.Rotate(Speed, 0f, -1 * Speed);
                break;

        }
    }
}
