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
            //need to treat full rockets differently
            if (l.name == "Rocket")
            {
                highlightRocket(l);
            }
            else
            {
                l.SetActive(true);
                l.GetComponent<Level>().makeOpaque();
                highlightedObjects.Add(l);
            }
        }
    }

    public static void unhighlightAllPossibleBuildLocations(List<GameObject> Levels)
    {
        foreach (GameObject l in Levels)
        {
            //need to treat full rockets differently
            if (l.name == "Rocket")
            {
                unhighlightRocket(l);
            }
            else
            {
                l.SetActive(false);
                l.GetComponent<Level>().resetMaterial();
                highlightedObjects.Remove(l);
            }
        }
    }
    private static void highlightRocket(GameObject Rocket)
    {
        GameObject level1 = Rocket.transform.GetChild(0).gameObject;
        GameObject level2 = Rocket.transform.GetChild(1).gameObject;
        GameObject level3 = Rocket.transform.GetChild(2).gameObject;

        level1.GetComponent<Level>().makeOpaque();
        level2.GetComponent<Level>().makeOpaque();
        level3.GetComponent<Level>().makeOpaque();

        highlightedObjects.Add(level1);
        highlightedObjects.Add(level2);
        highlightedObjects.Add(level3);
    }
    private static void unhighlightRocket(GameObject Rocket)
    {
        GameObject level1 = Rocket.transform.GetChild(0).gameObject;
        GameObject level2 = Rocket.transform.GetChild(1).gameObject;
        GameObject level3 = Rocket.transform.GetChild(2).gameObject;
        
        level1.GetComponent<Level>().resetMaterial();
        level2.GetComponent<Level>().resetMaterial();
        level3.GetComponent<Level>().resetMaterial();

        highlightedObjects.Remove(level1);
        highlightedObjects.Remove(level2);
        highlightedObjects.Remove(level3);
    }
    public static void highlightPlayersBuilder(Player p)
    {
        highlightedObjects.Add(p.getBuilders().Item1.gameObject);
        highlightedObjects.Add(p.getBuilders().Item2.gameObject);
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
