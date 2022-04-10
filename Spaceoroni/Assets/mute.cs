using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mute : MonoBehaviour
{
    public void MMute()
    {
        GetComponent<AudioSource>().mute = true;
    }
    public void unMMute()
    {
        GetComponent<AudioSource>().mute = false;
    }
}
