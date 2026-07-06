using System;
using TMPro;
using UnityEngine;

public class MileStonSign : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI worldText;

    public void UpdateDistance(float distance)
    {
        int distInMeters=Mathf.FloorToInt(distance/2);
        if (worldText != null)
        {
            worldText.text=distInMeters.ToString()+"m";
        }

    }
}
