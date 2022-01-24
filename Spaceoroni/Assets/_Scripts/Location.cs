using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Location : MonoBehaviour
{
    public Material highlightMaterial1;
    private Material startMaterial;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello");
        startMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeMesh(Material highlightMaterial)
    {
        Debug.Log("ChangeMesh()");

        GetComponent<Renderer>().material = highlightMaterial;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        GetComponent<Renderer>().material = highlightMaterial1;

        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        Debug.Log(name + " Game Object Clicked!");
    }

    void OnMouseDown()
    {
        GetComponent<Renderer>().material = highlightMaterial1;

        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        Debug.Log(name + " Game Object Clicked!");
    }
}
