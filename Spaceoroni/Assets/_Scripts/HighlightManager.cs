using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HighlightManager
{
    public static List<GameObject> highlightedObjects = new List<GameObject>();
    private static Material highlight;
    private static Material possibleHighlight;

    public static void highlightPossibleMoveLocations(Coordinate location)
    {
        GameObject obj = GameObject.Find(Coordinate.coordToString(location));
        obj.GetComponent<Renderer>().material = possibleHighlight;
        highlightedObjects.Add(obj);
    }
    public static void highlightAllPossibleMoveLocations(List<string> locations)
    {
        foreach (string coord in locations)
        {
            GameObject obj = GameObject.Find(coord);
            obj.GetComponent<Renderer>().material = possibleHighlight;
            highlightedObjects.Add(obj);
        }
    }
    public static void unhighlightAllPossibleMoveLocations(List<string> locations)
    {
        foreach (string coord in locations)
        {
            GameObject obj = GameObject.Find(coord);
            obj.GetComponent<Location>().resetMaterial();
            highlightedObjects.Remove(obj);
        }
    }

    public static void highlightAllPossibleBuildLocations(List<GameObject> Levels)
    {
        foreach (GameObject l in Levels)
        {
            l.SetActive(true);
            l.GetComponent<Level>().makeOpaque();
            highlightedObjects.Add(l);
        }
    }

    public static void unhighlightAllPossibleBuildLocations(List<GameObject> Levels)
    {
        foreach (GameObject l in Levels)
        {
            l.SetActive(false);
            l.GetComponent<Level>().resetMaterial();
            highlightedObjects.Remove(l);
        }
    }
    public static void highlightPlayersBuilder(Player p)
    {
        highlightedObjects.Add(p.Builder1GameObject);
        highlightedObjects.Add(p.Builder2GameObject);
    }
    public static Material getHighlightMat()
    {
        return highlight;
    }

    public static bool isHighlightObj(GameObject obj)
    {
        return highlightedObjects.Contains(obj);
    }
}
