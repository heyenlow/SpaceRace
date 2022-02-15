using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void setGameModeTutorial() { GameSettings.gameType = GameSettings.GameType.Tutorial; }
    public void setGameModeSinglePlayer() 
    {
        //gameObject.GetComponent<CameraMovement>().moveCamera();
        GameSettings.gameType = GameSettings.GameType.Singleplayer; 
    }
    public void setGameModeMultiplayer() { GameSettings.gameType = GameSettings.GameType.Multiplayer; }
    public void GameModeClear() { GameSettings.gameType = GameSettings.GameType.Singleplayer; }


    //move the camera
    //start the game
    public void Play()
    {
        
    }
    public void QuitButton(){
        Debug.Log("Quit");
        Application.Quit();
    }
}
