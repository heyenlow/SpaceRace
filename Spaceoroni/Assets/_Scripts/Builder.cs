using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public Coordinate coord = new Coordinate();
    GameObject movingBuilder;
    Vector3 newLocation;
    Quaternion newRotation;
    public float Speed = 5;
    public float RotationSpeed = 3;
    Vector3 homeLocation;
    Quaternion homeRotation;


    // Start is called before the first frame update
    void Start()
    {
        homeLocation = this.transform.position;
        homeRotation = this.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (movingBuilder != null)
        {
            movingBuilder.transform.position = Vector3.MoveTowards(movingBuilder.transform.position, newLocation, Speed * Time.deltaTime);
            movingBuilder.transform.rotation = Quaternion.RotateTowards(movingBuilder.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);

            if (movingBuilder.transform.position == newLocation && movingBuilder.transform.rotation == newRotation) movingBuilder = null;
        }
    }

    public void move(Coordinate c, Game g)
    {
        moveBuilderToNewSquare(this.gameObject, findSquare(c), g); //moves the object to the new square
        coord.x = c.x;
        coord.y = c.y;
    }

    public void moveBuilderToNewSquare(GameObject GamePiece, GameObject Square, Game g)
    {
        Coordinate coordinateOfSquare = Coordinate.stringToCoord(Square.name);
        //this next line will need to be adjusted for the height of each level object
        Vector3 heightDiff = new Vector3(0, (g.heightAtCoordinate(coordinateOfSquare)), 0);
        newLocation = Square.transform.position + heightDiff;
        movingBuilder = GamePiece;
    }

    public GameObject findSquare(Coordinate c)
    {
        return GameObject.Find(Coordinate.coordToString(c));
    }

    public Coordinate getLocation()
    {
        return coord;
    }
    public void returnHome()
    {
        newLocation = homeLocation;
        newRotation = homeRotation;
        movingBuilder = this.gameObject;
        coord = new Coordinate();
    }

    private void OnMouseOver()
    {
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            GetComponent<Renderer>().material.shader = Shader.Find("Ultimate 10+ Shaders/Outline");
        }
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.shader = Shader.Find("Standard");
    }

    void OnMouseDown()
    {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        if(HighlightManager.isHighlightObj(this.gameObject)) Game.recieveLocationClick(coord);
    }
}