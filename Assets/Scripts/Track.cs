using UnityEngine;
using UnityEngine.Pool;

public class Track : MonoBehaviour
{
   public void OnDespawn()
    {
        gameObject.SetActive(false);
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }
}
