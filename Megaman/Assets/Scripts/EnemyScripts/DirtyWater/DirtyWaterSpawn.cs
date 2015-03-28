using UnityEngine;
using System.Collections;

public class DirtyWaterSpawn : MonoBehaviour {

    bool Spawning = false;
    public bool AmIAlive = false;
    public GameObject DirtyWaterPrefab;
    GameObject DirtyWater;
	
	// Update is called once per frame
	void FixedUpdate () {
        if(!AmIAlive && !Spawning){
            Spawning = true;
            AmIAlive = true;
            Invoke("Spawn", 1f);
        }
	}

    void Spawn()
    {
        DirtyWater = Instantiate(DirtyWaterPrefab, transform.position, Quaternion.identity) as GameObject;
        DirtyWater.transform.parent = this.transform;
        Spawning = false;
    }
}
