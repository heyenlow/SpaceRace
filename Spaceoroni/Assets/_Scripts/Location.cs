using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour
{
    Color highlightMaterial;
    Game gameController;
    Material startMaterial;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        startMaterial = this.GetComponent<Renderer>().material;
        highlightMaterial = new Color(98, 136, 147);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        //GetComponent<Renderer>().material.color = highlightMaterial;
        gameController.highlightPossibleMoveLocations(Coordinate.stringToCoord(this.name));
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material = startMaterial;
    }

    void OnMouseDown()
    {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        Debug.Log(name + " Game Object Clicked!");
        gameController.recieveLocationClick(Coordinate.stringToCoord(this.name));
    }
}
