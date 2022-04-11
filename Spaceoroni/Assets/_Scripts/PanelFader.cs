using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFader : MonoBehaviour
{
    public float Duration = 10.0f;

    public void FadePanel(GameObject panel)
    {
        var canvasGroup = panel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;

        StartCoroutine(DoFade(canvasGroup, 1, 0));
    }

    public IEnumerator DoFade(CanvasGroup canvG, float start, float end)
    {
        float counter = 0f;

        while (counter < Duration)
        {
            counter += (Time.deltaTime);
            canvG.alpha = Mathf.Lerp(start, end, counter / Duration);

            yield return null;
        }
    }
}
