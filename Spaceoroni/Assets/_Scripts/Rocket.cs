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

    [SerializeField]
    private GameObject level1Animation;
    [SerializeField]
    private GameObject level2Animation;
    [SerializeField]
    private GameObject level3Animation;
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

    public IEnumerator runEndOfGameAnimation(bool elon)
    {
        if(elon)GetComponentInChildren<EndOfGameAnimation>().elonBlastOff();
        else GetComponentInChildren<EndOfGameAnimation>().jeffBlastOff();
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

    public IEnumerator buildLevel(int level)
    {
        runBuildAnimation(level);
        yield return new WaitForSeconds(1.8f);
        level1Animation.SetActive(false);
        level2Animation.SetActive(false);
        level3Animation.SetActive(false);

        transform.GetChild(level - 1).gameObject.SetActive(true);
        Game.built = true;
    }

    private void runBuildAnimation(int level)
    {
        switch (level)
        {
            case 1:
                level1Animation.SetActive(true);
                break;
            case 2:
                level2Animation.SetActive(true);
                break;
            case 3:
                level3Animation.SetActive(true);
                break;
        }
    }
}
