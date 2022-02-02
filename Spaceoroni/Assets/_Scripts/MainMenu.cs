using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HowToPlay(){
        SceneManager.LoadScene("Scene1");
    }
    

    public void Play(){
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().moveCamera();
    }

    public void QuitButton(){
        Debug.Log("Quit");
        Application.Quit();
    }
}
