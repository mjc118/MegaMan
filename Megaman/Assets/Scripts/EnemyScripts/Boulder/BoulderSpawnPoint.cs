using UnityEngine;
using System.Collections;

public class BoulderSpawnPoint : MonoBehaviour {

    bool Spawning = false;
    public bool AmIAlive = false;
    public GameObject BoulderPrefab;
    GameObject Boulder;
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!AmIAlive && !Spawning)
        {
            Spawning = true;
            AmIAlive = true;
            Invoke("Spawn", 1.5f);
        }
	}

    void Spawn()
    {
        Boulder = Instantiate(BoulderPrefab, transform.position, Quaternion.identity) as GameObject;
        Boulder.transform.parent = this.transform;
        Spawning = false;
    }
}
