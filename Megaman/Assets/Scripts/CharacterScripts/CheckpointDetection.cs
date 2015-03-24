using UnityEngine;
using System.Collections;

public class CheckpointDetection : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D trigger){
		if (trigger.gameObject.tag == "Player") {
			if(trigger.GetComponent<MegamanMovement>().SpawnPoint.position.x < transform.position.x){
				trigger.GetComponent<MegamanMovement>().SpawnPoint.position = transform.position;
			}
		}
	}
}
