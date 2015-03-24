using UnityEngine;
using System.Collections;

public class BossSpawnScript : MonoBehaviour {

    public float BossRoomEntryPosition;

    void OnTriggerExit2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Player")
        {
            if (trigger.gameObject.transform.position.x > BossRoomEntryPosition)
            {
                this.gameObject.GetComponentInParent<EnemySpawnPoint>().SpawnBoss = true;
                this.GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }
	
}
