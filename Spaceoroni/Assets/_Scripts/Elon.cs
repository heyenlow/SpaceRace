using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elon : MonoBehaviour
{
    private Animator anim;
    [SerializeField]
    private GameObject ELONMUSK;

    [SerializeField]
    private GameObject ELONPERCH;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    public void runGetOutOfCarAnimation()
    {
        anim.SetTrigger("OpenDoor");
    }

    public void moveToGameBoard()
    {
        ELONMUSK.transform.position = ELONPERCH.transform.position;
    }

    public void moveToRocket()
    {

    }

    public void runBlastOff()
    {
        anim.SetTrigger("BlastOff");
    }
}
