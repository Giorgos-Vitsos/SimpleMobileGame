using UnityEngine;


public class Track : MonoBehaviour
{

    [SerializeField] public Transform[] spawnPoints;
   public void OnDespawn()
    {
        gameObject.SetActive(false);
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }
}
