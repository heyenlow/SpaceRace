using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour
{
    private Game gameController;
    private Color startColor;
    private Color highlightColor;
    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        originalColor = this.GetComponent<Renderer>().material.color;
        highlightColor = new Color(0, 145, 255);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        //GetComponent<Renderer>().material.color = highlightMaterial;
        if(gameController.isHighlightObj(this.gameObject)) 
        {
            if (GetComponent<Renderer>().material.color != highlightColor) { startColor = GetComponent<Renderer>().material.color; }
            GetComponent<Renderer>().material.color = highlightColor;
        }
    }

    private void OnMouseExit()
    {
        if (gameController.isHighlightObj(this.gameObject))
        {
            GetComponent<Renderer>().material.color = startColor;
        }
    }

    void OnMouseDown()
    {
        gameController.recieveLocationClick(Coordinate.stringToCoord(this.name));
        resetMaterial();
    }

    public void resetMaterial()
    {
        this.GetComponent<Renderer>().material.color = originalColor;
    }

}
