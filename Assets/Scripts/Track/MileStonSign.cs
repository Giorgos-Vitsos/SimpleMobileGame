using TMPro;
using UnityEngine;

public class MileStonSign : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI worldText;

    //updates the text with the distance
    public void UpdateDistance(float distance)
    {
        int distInMeters = Mathf.FloorToInt(distance / 2);
        if (worldText != null)
        {
            worldText.text = distInMeters.ToString() + "m";
        }

    }
}
