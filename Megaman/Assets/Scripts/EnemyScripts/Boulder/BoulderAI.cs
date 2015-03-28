using UnityEngine;
using System.Collections;

public class BoulderAI : MonoBehaviour {

    bool DestroyingSelf = false;

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (!DestroyingSelf && trigger.tag == "Spikes")
        {
            DestroyingSelf = true;
            Invoke("DestroySelf", 0.5f);
        }
    }

    void DestroySelf()
    {
        this.GetComponentInParent<BoulderSpawnPoint>().AmIAlive = false;
        Destroy(gameObject);
    }
}
