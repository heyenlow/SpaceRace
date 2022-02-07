using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Game g;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator startGame()
    {
        Player player1 = new Player();
        Player player2 = new Player();
        yield return StartCoroutine(g.PlayGameToEnd());
        yield return null;
    }
}
