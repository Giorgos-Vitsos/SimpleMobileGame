using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.OnPlaySFX?.Invoke(SoundType.ButtonHover);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameEvents.OnPlaySFX?.Invoke(SoundType.ButtonClick);
    }
}