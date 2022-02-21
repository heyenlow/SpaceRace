using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour
{
    private bool isMove = false;
    public void blastOffRocket()
    {
        var rocket = this.GetComponentInChildren<Rocket>();
        rocket.blastOffRocket();
    }

    private void OnMouseOver()
    {
        if(HighlightManager.isHighlightObj(this.gameObject)) 
        {
            mouseOverHighlight();
        }
    }

    private void OnMouseExit()
    {
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            if (isMove)
            {
                highlightLocation();
            }
            else
            {
                removeHighlight();
            }
        }
    }

    void OnMouseDown()
    {
        if (HighlightManager.isHighlightObj(this.gameObject)) 
        {
            Game.recieveLocationClick(Coordinate.stringToCoord(this.name));
            removeHighlight();
        }
    }

    public void removeHighlight()
    {
        isMove = false;
        this.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
    }
    public void highlightLocation()
    {
        isMove = true;
        this.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Ultimate 10+ Shaders/Force Field");
    }
    public void mouseOverHighlight()
    {
        this.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Ultimate 10+ Shaders/Plexus Line");
    }

}
