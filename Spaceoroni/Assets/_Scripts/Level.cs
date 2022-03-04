using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Vector3 homeLocation;
    private void Start()
    {
        homeLocation = this.transform.position;
    }
    public void reset()
    {
        this.transform.position = homeLocation;
        removeHighlight(); 
        this.gameObject.SetActive(false); 
    }
    public enum HighlightType
    {
        move,
        build
    }
    public static HighlightType highlightType;

    private void OnMouseOver()
    {
        //GetComponent<Renderer>().material.color = highlightMaterial;
        if (HighlightManager.isHighlightObj(this.gameObject) || Rocket.blinkingRocket == GetComponentInParent<Rocket>())
        {
            removeHighlight();
        }
        //gameController.canHighlightBuilderPlacement(Coordinate.stringToCoord(this.name));
    }

    private void OnMouseExit()
    {
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            highlightLevel();
        }
        if(Rocket.blinkingRocket == this.GetComponentInParent<Rocket>())
        {
            Blink();
        }
    }

    void OnMouseDown()
    {
        if (HighlightManager.isHighlightObj(this.gameObject) && (Rocket.blinkingRocket == null || GetComponentInParent<Rocket>() == Rocket.blinkingRocket))
        {
            Game.recieveLocationClick(Coordinate.stringToCoord(this.transform.parent.parent.name));
            Rocket.blinkingRocket = null;
            GetComponentInParent<Location>().removeHighlight();
            foreach (Level l in GetComponentInParent<Rocket>().getRocketsLevels()) l.removeHighlight();
            removeHighlight();
        }
    }

    public void highlightLevel()
    {
        var childRenderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (var rend in childRenderers)
        {
            if(highlightType == HighlightType.build) rend.material.shader = Shader.Find("Ultimate 10+ Shaders/Force Field");
            else if (highlightType == HighlightType.move) rend.material.shader = Shader.Find("Ultimate 10+ Shaders/Plexus Line");
        }
    }

    public void Blink()
    {
        Rocket.blinkingRocket = GetComponentInParent<Rocket>();
        var childRenderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (var rend in childRenderers)
        {
            rend.material.shader = Shader.Find("Shader Graphs/Blink");
        }
    }

    public void removeHighlight()
    {
        var childRenderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (var rend in childRenderers)
        {
            rend.material.shader = Shader.Find("Universal Render Pipeline/Lit");
        }
    }

}
