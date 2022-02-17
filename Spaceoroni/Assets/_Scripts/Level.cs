using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum HighlightType
    {
        move,
        build
    }
    public static HighlightType highlightType;

    private void OnMouseOver()
    {
        //GetComponent<Renderer>().material.color = highlightMaterial;
        if (HighlightManager.isHighlightObj(this.gameObject))
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
    }

    void OnMouseDown()
    {
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            Game.recieveLocationClick(Coordinate.stringToCoord(this.transform.parent.parent.name));
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

    public void removeHighlight()
    {
        var childRenderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (var rend in childRenderers)
        {
            rend.material.shader = Shader.Find("Standard");
        }
    }

}
