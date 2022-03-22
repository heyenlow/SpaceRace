using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeListener : MonoBehaviour
{
    public Slider MusicVolumeSliderMain;
    public Slider MusicVolumeSliderInGame;

    public void UpdateVolume(string whichSlider)
    {
        if (whichSlider == "MainMenu")
        {
            MusicVolumeSliderInGame.value = MusicVolumeSliderMain.value;
            GetComponent<AudioSource>().volume = MusicVolumeSliderMain.value;
        }
        else if (whichSlider == "InGame")
        {
            MusicVolumeSliderMain.value = MusicVolumeSliderInGame.value;
            GetComponent<AudioSource>().volume = MusicVolumeSliderInGame.value;
        }
        else
        {
            Debug.LogError("The String passed to update volume is not found: " + whichSlider);
        }
    }
}
