using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private GameObject movingRocket;
    private Vector3 homeLocation;
    public int Speed = 2;
    private Vector3 newLocation;
    private void Start()
    {
        homeLocation = this.transform.position;
    }
    void Update()
    {
        if (movingRocket != null)
        {
            movingRocket.transform.position = Vector3.MoveTowards(movingRocket.transform.position, newLocation, Speed * Time.deltaTime);

            if (movingRocket.transform.position == newLocation) movingRocket = null;
        }
    }

    public void blastOffRocket()
    {
        Vector3 heightDiff = new Vector3(0, 100, 0);
        newLocation = this.gameObject.transform.position + heightDiff;

        movingRocket = this.gameObject;
    }

    public void resetLocation()
    {
        movingRocket = null;
        this.transform.position = homeLocation;
    }
}
