using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI num;
    [SerializeField]
    AudioSource countdownAudio;

    int fontStartSize = 40;
    bool fontIncrease = false;
    int number = 2;

    void Update()
    {
        if(fontIncrease) num.fontSize += 5;
    }

    private void decrement()
    {
        num.fontSize = fontStartSize;
        num.text = number.ToString();
        number--;
    }

    public IEnumerator startCountdown()
    {
        countdownAudio.Play();
        num.text = "3";
        fontIncrease = true;
        yield return new WaitForSeconds(1.3f);


        while (number > 0){
            decrement();
            yield return new WaitForSeconds(1.3f);
        }
        fontIncrease = false;

        num.fontSize = 150;
        num.text = "Blast Off!";
        yield return new WaitForSeconds(0.4f);
        
        num.text = "";
        num.fontSize = fontStartSize;
        number = 2;

        Game.countDownActive = false;
    }
}
