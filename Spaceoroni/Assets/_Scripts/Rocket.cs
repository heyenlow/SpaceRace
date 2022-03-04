using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private GameObject movingRocket;
    private Vector3 homeLocation;
    public int Speed = 15;
    private Vector3 newLocation;
    public static Rocket blinkingRocket = null;
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

    public void Blink()
    {
        blinkingRocket = this;
        var levelOfRocket = GetComponentsInChildren<Level>();
        foreach (Level l in levelOfRocket)
        {
            l.Blink();
        }
    }
    public Level[] getRocketsLevels()
    {
        return GetComponentsInChildren<Level>();
    }
}
