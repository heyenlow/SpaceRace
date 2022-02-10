using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        Debug.Log(this.transform.parent.name);

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
        Debug.Log(this.transform.parent.name);
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            Game.recieveLocationClick(Coordinate.stringToCoord(this.transform.parent.name));
            resetMaterial();
        }
    }

    public void makeOpaque()
    {
        var childRenderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (var rend in childRenderers)
        {
            rend.material.shader = Shader.Find("Ultimate 10+ Shaders/Force Field");
        }
    }

    public void resetMaterial()
    {
        var childRenderers = this.gameObject.GetComponentsInChildren<Renderer>();
        foreach (var rend in childRenderers)
        {
            rend.material.shader = Shader.Find("Standard");
        }
    }

}
