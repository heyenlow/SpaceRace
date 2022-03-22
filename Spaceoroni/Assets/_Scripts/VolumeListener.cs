using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeListener : MonoBehaviour
{
    [SerializeField]
    private Slider MusicVolumeSliderMain;
    [SerializeField]
    private Slider MusicVolumeSliderInGame;

    [SerializeField]
    private GameObject MuteIG;
    [SerializeField]
    private GameObject UnMuteIG;
    [SerializeField]
    private GameObject UnMuteMM;
    [SerializeField]
    private GameObject MuteMM;


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


    public void Mute()
    {
        GetComponent<AudioSource>().mute = true;

        MuteIG.SetActive(false);
        MuteMM.SetActive(false);
        UnMuteIG.SetActive(true);
        UnMuteMM.SetActive(true);
    }

    public void UnMute()
    {
        GetComponent<AudioSource>().mute = false;

        UnMuteIG.SetActive(false);
        UnMuteMM.SetActive(false);
        MuteIG.SetActive(true);
        MuteMM.SetActive(true);
    }
}
