using UnityEngine;
using System.Collections;

public class ChangeCameraLimit : MonoBehaviour {

	public float CameraLimit;

	void OnTriggerEnter2D(Collider2D trigger){
		if(trigger.tag == "Player"){
			GameObject.Find ("Main Camera").GetComponent<SmoothFollowMegaMan> ().CameraLowerLimit = CameraLimit;
			if(Application.loadedLevelName == "NeonTigerStage"){
				if(CameraLimit == -8f){
					GameObject.Find ("_GM/EveningTreeFarBackground").GetComponent<SpriteRenderer>().sortingLayerName = "BackGround";
				}
				else{
					GameObject.Find ("_GM/EveningTreeFarBackground").GetComponent<SpriteRenderer>().sortingLayerName = "Default";
				}
			}
		}
	}
}
