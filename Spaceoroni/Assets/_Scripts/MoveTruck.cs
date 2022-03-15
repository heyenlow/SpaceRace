using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTruck : MonoBehaviour
{
    private Transform[] points;
    [SerializeField]
    private float speed;
    private bool finishedMoving = true;
    private Transform nextPoint;

    private void Start()
    {
        points = GameObject.Find("TruckTrack").GetComponentsInChildren<Transform>();
    }

    public void Move()
    {
        StartCoroutine(moveThroughPoints());
    }
    private IEnumerator moveThroughPoints()
    {
        foreach(var p in points)
        {
            nextPoint = p;
            Debug.Log("Moving to point: " + p.name + " start: " + transform.position + " end: " + nextPoint.position);

            finishedMoving = false;

            StartCoroutine(moveToNextPoint());
            while(!finishedMoving) yield return new WaitForEndOfFrame();
        }
        StartCoroutine(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineCamSwitcher>().MoveToElonText());
    }

    private IEnumerator moveToNextPoint()
    {
        while(!finishedMoving)
        {
            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.MoveTowards(transform.position, nextPoint.position, Time.deltaTime * speed);
            transform.rotation = Quaternion.LookRotation(transform.forward);
            if (VeryCloseObject(transform, nextPoint))
            {
                transform.position = nextPoint.position; 
                finishedMoving = true; 
            }
            yield return null;
        }
    }

    private bool VeryCloseObject(Transform a, Transform b)
    {
        float maxDiff = .1f;
        var xdiff = Mathf.Abs(a.position.x - b.position.x) < maxDiff;
        var ydiff = Mathf.Abs(a.position.x - b.position.x) < maxDiff;
        var zdiff = Mathf.Abs(a.position.x - b.position.x) < maxDiff;

        return xdiff && ydiff && zdiff;
    }
}
