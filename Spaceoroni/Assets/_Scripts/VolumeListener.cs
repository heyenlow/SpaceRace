using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeListener : MonoBehaviour
{
    [SerializeField]
    private AudioSource BackGroundMusic;

    [SerializeField]
    private Slider MusicVolumeSliderMainMenu;
    [SerializeField]
    private Slider MusicVolumeSliderGameMenu;

    [SerializeField]
    private GameObject MuteGameMenu;
    [SerializeField]
    private GameObject UnmuteGameMenu;
    [SerializeField]
    private GameObject MuteMainMenu;
    [SerializeField]
    private GameObject UnmuteMainMenu;

    public static bool MUTE = false;


    public void updateVolume(bool inGame)
    {
        if (inGame)
        {
            BackGroundMusic.volume = MusicVolumeSliderGameMenu.value;
            MusicVolumeSliderMainMenu.value = MusicVolumeSliderGameMenu.value;
        }
        else
        {
            BackGroundMusic.volume = MusicVolumeSliderMainMenu.value;
            MusicVolumeSliderGameMenu.value = MusicVolumeSliderMainMenu.value;

        }
    }

    public void Mute()
    {
        MUTE = true;

        var soundObjects = GameObject.FindGameObjectsWithTag("Audio");
        foreach (var sound in soundObjects)
        {
            sound.GetComponent<mute>().MMute();
        }
        //set the mute button hidden
        MuteGameMenu.SetActive(false);
        MuteMainMenu.SetActive(false);

        //set the unmute button visible
        UnmuteGameMenu.SetActive(true);
        UnmuteMainMenu.SetActive(true);
    }

    public void Unmute(bool inGame)
    {
        var soundObjects = GameObject.FindGameObjectsWithTag("Audio");
        foreach (var sound in soundObjects)
        {
            sound.GetComponent<mute>().unMMute();
        }
        MUTE = false;
        //set the unmute button hidden
        UnmuteGameMenu.SetActive(false);
        UnmuteMainMenu.SetActive(false);

        //set the mute button visible
        MuteGameMenu.SetActive(true);
        MuteMainMenu.SetActive(true);
    }
}
