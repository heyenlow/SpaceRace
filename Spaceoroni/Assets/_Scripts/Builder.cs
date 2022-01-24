using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    Coordinate coord = new Coordinate();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void move(Coordinate c)
    {
        coord.x = c.x;
        coord.y = c.y;
    }

    public string getLocation()
    {
        return Coordinate.coordToString(coord);
    }
}
