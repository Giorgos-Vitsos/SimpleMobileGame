using System.Collections;
using UnityEngine;

public class Fade : MonoBehaviour
{
    [SerializeField]private CanvasGroup canvas;
    [SerializeField]private float fadeSpeed=2f;

    private Coroutine _currentFadeCoroutine=null;

    public enum FadeType
    {
        In,Out
    }

    public void TriggerFade(FadeType type)
    {
        if (_currentFadeCoroutine != null)
        {
            StopCoroutine(_currentFadeCoroutine);
        }
        _currentFadeCoroutine=StartCoroutine(FadePlay(type));
    }
    private IEnumerator FadePlay(FadeType type)
    {
        if (type == FadeType.In)
        {
            while (canvas.alpha < 1f)
            {
                canvas.alpha += fadeSpeed * Time.unscaledDeltaTime;
                yield return null; 
            }
            canvas.alpha = 1f;
        }else if(type == FadeType.Out)
        {
            while (canvas.alpha > 0f)
            {
                canvas.alpha -= fadeSpeed * Time.unscaledDeltaTime;
                yield return null; 
            }
            canvas.alpha = 0f;
        }
    }
}
