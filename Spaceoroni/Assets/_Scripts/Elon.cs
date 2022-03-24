using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elon : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    public void runAnimation()
    {
        anim.SetBool("OpenDoor", true);
    }
}
