using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfGameAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject elon;
    [SerializeField]
    private GameObject jeff;
    [SerializeField]
    private Animator anim;


    public void elonBlastOff()
    {
        elon.SetActive(true);
        anim.SetTrigger("Elon");
    }

    public void jeffBlastOff()
    {
        jeff.SetActive(true);
        anim.SetTrigger("Jeff");
    }

    public void resetAnim()
    {
        anim.SetTrigger("Reset");
    }
}
