using UnityEngine;
using System.Collections;

public class SpinShotProjectile : MonoBehaviour {

	public float ShotSpeed;

	private Vector2 ParentPos;
	private Vector2 InitialPos;
	public float ShotDestructionDistance = 10f;
	float DistanceGone;
	
	string DirectionChose;

	Vector3 DirectionTranslation;

	// Use this for initialization
	void Start () {
		//grabs the initial position of the shot
		InitialPos.x = transform.position.x;
		InitialPos.y = transform.position.y;
		ParentPos.x = transform.parent.transform.position.x;
		ParentPos.y = transform.parent.transform.position.y;

		float Movement = ShotSpeed * Time.deltaTime;

		if (InitialPos.x > ParentPos.x && InitialPos.y == ParentPos.y) {
			DirectionTranslation = (Vector3.right * Movement);	
		}
		else if (InitialPos.x < ParentPos.x && InitialPos.y == ParentPos.y) {
			DirectionTranslation = (Vector3.left * Movement);		
		}
		else if (InitialPos.x > ParentPos.x && InitialPos.y > ParentPos.y) {
			DirectionTranslation = ((Vector3.right * Movement) + (Vector3.up * Movement));	
		}
		else if (InitialPos.x < ParentPos.x && InitialPos.y > ParentPos.y) {
			DirectionTranslation = ((Vector3.left * Movement) + (Vector3.up * Movement));		
		}
		else if (InitialPos.x > ParentPos.x && InitialPos.y < ParentPos.y) {
			DirectionTranslation = ((Vector3.right * Movement) + (Vector3.down * Movement));		
		}
		else{
			DirectionTranslation = ((Vector3.left * Movement) + (Vector3.down * Movement));	
		}
	}
	
	// Update is called once per frame
	void Update () {
		//perform our movement
		transform.Translate (DirectionTranslation);

		DistanceGone = Mathf.Sqrt (Mathf.Pow ((transform.position.x - InitialPos.x), 2) + Mathf.Pow ((transform.position.y - InitialPos.y), 2));
		if (DistanceGone > ShotDestructionDistance) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D trigger)
	{
		if (trigger.gameObject.tag == "Player") {
			trigger.gameObject.GetComponent<MegamanMovement>().Health -= 1;
			Destroy (gameObject);
		} 
		else if (trigger.gameObject.tag == "Terrain") {
			Destroy (gameObject);	
		}
	}
}
