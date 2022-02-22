using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeListener : MonoBehaviour
{
    public Slider MusicVolumeSlider;

    public void updateVolume()
    {
        GetComponent<AudioSource>().volume = MusicVolumeSlider.value;
    }
}
