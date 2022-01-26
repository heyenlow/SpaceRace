using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    Coordinate coord = new Coordinate();
    Game gameController;
    Material startMaterial;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        startMaterial = this.GetComponent<Renderer>().material;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void move(Coordinate c)
    {
        gameController.moveToNewSquare(this.gameObject, gameController.findSquare(c)); //moves the object to the new square
        coord.x = c.x;
        coord.y = c.y;
    }

    public string getLocation()
    {
        return Coordinate.coordToString(coord);
    }

    private void OnMouseOver()
    {
        if (gameController.GameRunning && gameController.isTurn(coord)) GetComponent<Renderer>().material = gameController.getHighlightMat();
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material = startMaterial;
    }


    void OnMouseDown()
    {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        if(gameController.GameRunning && gameController.isTurn(coord)) gameController.recieveLocationClick(coord);
    }
}