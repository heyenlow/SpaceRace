using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField]
    private MoveObject Truck;

    public void MoveTruck() { StartCoroutine(Truck.moveThroughPoints()); }
}
