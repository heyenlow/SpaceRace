using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundFX : MonoBehaviour
{
    public void buttonClick()
    {
        this.GetComponent<AudioSource>().Play();
    }
}
