using UnityEngine;
using System.Collections;

public class SmoothFollowMegaMan : MonoBehaviour {
	public Transform target;
	public float CameraLowerLimit;
	public float CameraLeftLimit;
	public float distance = 3.0f;
	public float height = 3.0f;
	public float damping = 5.0f;
	public bool smoothRotation = true;
	public bool followBehind = true;
	public float rotationDamping = 10.0f;
	public bool lockRotation = false;
	
	void Update () {
		//if the Boss is Alive

		if (GameObject.Find ("EnemySpawnPoints/Boss").GetComponent<EnemySpawnPoint> ().IsThisEnemyAlive) {
            if (Application.loadedLevelName == "NeonTigerStage")
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(114.36f, 11.7f, -10f), Time.deltaTime * damping);
            }
            else if (Application.loadedLevelName == "WaterStage")
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(75.8f, -15f, -10f), Time.deltaTime * damping);
            }
		}
		else if(target.position.y > CameraLowerLimit && target.position.x > CameraLeftLimit){
			Vector3 wantedPosition;
			if(followBehind)
				wantedPosition = target.TransformPoint(0, height, -distance);
			else
				wantedPosition = target.TransformPoint(0, height, distance);
		
			transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * damping);
		
			if (smoothRotation) {
				Quaternion wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
				transform.rotation = Quaternion.Slerp (transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
			}
			else transform.LookAt (target, target.up);

			if (lockRotation) 
			{
				transform.localRotation = Quaternion.Euler (0,0,0);
			}
		}

	}
}