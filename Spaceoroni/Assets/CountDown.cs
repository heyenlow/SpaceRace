using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI num;

    bool fontIncrease = false;
    int number = 2;
    void Update()
    {
        if(fontIncrease) num.fontSize += 5;
    }

    private void decrement()
    {
        num.fontSize = 60;
        num.text = number.ToString();
        number--;
    }

    public IEnumerator startCountdown()
    {
        num.text = "3";
        fontIncrease = true;
        yield return new WaitForSeconds(1f);


        while (number > 0){
            decrement();
            yield return new WaitForSeconds(1);
        }
        fontIncrease = false;

        num.fontSize = 150;
        num.text = "Blast Off!";
        yield return new WaitForSeconds(0.4f);
        
        num.text = "";
        num.fontSize = 60;
        number = 2;

        Game.countDownActive = false;
    }
}
