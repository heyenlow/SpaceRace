using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
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

    private void OnMouseOver()
    {
        //GetComponent<Renderer>().material.color = highlightMaterial;
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            resetMaterial();
        }
        //gameController.canHighlightBuilderPlacement(Coordinate.stringToCoord(this.name));
    }

    private void OnMouseExit()
    {
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            makeOpaque();
        }
    }

    void OnMouseDown()
    {
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            Game.recieveLocationClick(Coordinate.stringToCoord(this.transform.parent.name));
            resetMaterial();
        }
    }

    public void makeOpaque()
    {
        this.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Transparent");
    }

    public void resetMaterial()
    {
        this.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
        GetComponent<Renderer>().material = startMaterial;
    }

}
