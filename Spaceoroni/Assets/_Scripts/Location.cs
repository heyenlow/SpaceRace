using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour
{
    private bool isMove = false;
    public static Location LocationBlinking = null;
    private bool deadLocation = false;

    public void runEndOfGameAnimation()
    {
        var rocket = this.GetComponentInChildren<Rocket>();
        rocket.runEndOfGameAnimation();
    }

    public void blastOffRocket()
    {
        var rocket = this.GetComponentInChildren<Rocket>();
        StartCoroutine(blastOffAnimatin());
        rocket.blastOffRocket();
        deadLocation = true;
        this.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("FX/Flare");
    }

    public void setLocationAlive() { deadLocation = false; }

    public IEnumerator blastOffAnimatin()
    {
        this.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        this.transform.GetChild(1).gameObject.SetActive(false);
        yield return null;
    }

    private void OnMouseOver()
    {
        if(HighlightManager.isHighlightObj(this.gameObject) && !deadLocation) 
        {
            mouseOverHighlight();
        }
    }

    private void OnMouseExit()
    {
        if (!deadLocation)
        {
            if (HighlightManager.isHighlightObj(this.gameObject))
            {
                if (this == LocationBlinking)
                {
                    Blink();
                }
                else if (isMove)
                {
                    highlightLocation();
                }
                else
                {
                    removeHighlight();
                }
            }
            else
            {
                removeHighlight();
            }
        }
    }

    void OnMouseDown()
    {
        if (HighlightManager.isHighlightObj(this.gameObject) && (LocationBlinking == null || this == LocationBlinking) && !deadLocation) 
        {
            Game.recieveLocationClick(Coordinate.stringToCoord(this.name));
            LocationBlinking = null;
            removeHighlight();
        }
    }

    public void removeHighlight()
    {
        isMove = false;
        this.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
    }
    public void highlightLocation()
    {
        isMove = true;
        this.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Shader Graphs/HighlightSquare");// "Ultimate 10+ Shaders/Force Field");
    }
    public void mouseOverHighlight()
    {
        this.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Ultimate 10+ Shaders/Plexus Line");// 
    }

    public void Blink()
    {
        LocationBlinking = this;
        this.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Shader Graphs/Blink");
        var rocket = GetComponentInChildren<Rocket>();
        rocket.Blink();
    }

}
