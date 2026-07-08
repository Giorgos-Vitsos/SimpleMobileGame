using System.Collections.Generic;
using UnityEngine;

//shuffles the contents of a list (for better randomization)
public static class ShuffleUtility
{
    public static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}