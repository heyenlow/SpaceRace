using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField]
    public GameObject[] points;
    [SerializeField]
    public float[] speedToNextPoint;

    private bool finishedMoving = true;

    // Transforms to act as start and end markers for the journey.
    private Transform startMarker;
    private Transform endMarker;

    // Movement speed in units per second.
    private float speed;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;
    private float portionOfJourneyDone;

    private void Start()
    {
        if (speedToNextPoint.Length != points.Length) { Debug.LogError(this.name + " MoveObject: All Points Must Have A SpeedToNextPoint"); }
    }

    // Move to the target end position.
    void Update()
    {
        if (!finishedMoving)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.MoveTowards(startMarker.position, endMarker.position, fractionOfJourney);
            if (transform.position == endMarker.position) {Debug.Log("DoneMoveing"); finishedMoving = true;}
        }
    }

    public IEnumerator moveThroughPoints()
    {
        int i = 0;
        foreach(var p in points)
        {
            portionOfJourneyDone = 0;
            Debug.Log("Moving to point: " + p.name);
            startMarker = this.transform;
            endMarker = p.transform;

            speed = speedToNextPoint[i++];

            // Keep a note of the time the movement started.
            startTime = Time.time;

            // Calculate the journey length.
            journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

            finishedMoving = false;
            while(!finishedMoving) yield return new WaitForEndOfFrame();
        }
    }
}
