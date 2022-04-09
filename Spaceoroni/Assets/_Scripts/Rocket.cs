using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private GameObject movingRocket;
    private Vector3 homeLocation;
    public int Speed = 15;
    private Vector3 newLocation;
    public static Rocket blinkingRocket = null;
    [SerializeField]
    private GameObject Flame;
    private void Start()
    {
        homeLocation = this.transform.position;
    }
    void Update()
    {
        if (movingRocket != null)
        {
            movingRocket.transform.position = Vector3.MoveTowards(movingRocket.transform.position, newLocation, Speed * Time.deltaTime);

            if (movingRocket.transform.position == newLocation) movingRocket = null;
        }
    }

    public void blastOffRocket()
    {
        if(!VolumeListener.MUTE) GetComponent<AudioSource>().Play();
        StartCoroutine(blastOffFlame());
        Vector3 heightDiff = new Vector3(0, 100, 0);
        newLocation = this.gameObject.transform.position + heightDiff;
        movingRocket = this.gameObject;
    }

    public IEnumerator blastOffFlame()
    {
        Flame.SetActive(true);
        yield return new WaitForSeconds(5);
        Flame.SetActive(false);
        yield return null;
    }

    public IEnumerator runEndOfGameAnimation()
    {
        GetComponentInChildren<EndOfGameAnimation>().elonBlastOff();
        yield return new WaitForSeconds(5);
        GetComponentInChildren<EndOfGameAnimation>().resetAnim();
    }

    public void resetLocation()
    {
        movingRocket = null;
        this.transform.position = homeLocation;
    }

    public void Blink()
    {
        blinkingRocket = this;
        var levelOfRocket = GetComponentsInChildren<Level>();
        foreach (Level l in levelOfRocket)
        {
            l.Blink();
        }
    }
    public Level[] getRocketsLevels()
    {
        return GetComponentsInChildren<Level>();
    }
}
