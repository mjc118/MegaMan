using UnityEngine;
using System.Collections;

public class NeonTigerProjectileAI : MonoBehaviour {

	public float ShotSpeed;
	
	private Vector2 ParentPos;
	private Vector2 InitialPos;
	public float ShotDestructionDistance = 10f;
	float DistanceGone;

	Vector3 DirectionTranslation;
	//from 1-5, used to identify which shot I am to
	//deduce which direction i should go
	float ShotNumber = 0;
	bool ParentFacingRight;

	// Use this for initialization
	void Start () {
		InitialPos.x = transform.position.x;
		InitialPos.y = transform.position.y;
		ParentPos.x = transform.parent.transform.position.x;
		ParentPos.y = transform.parent.transform.position.y;
		ParentFacingRight = transform.parent.GetComponent<NeonTigerAI> ().FacingRight;

		//loop until our ShotNumberIsAssigned
		while (ShotNumber == 0);

		float Movement = ShotSpeed * Time.deltaTime;

		//we are firing from the ground 1-5, 6-10 means we are firing from the wall
		if (ShotNumber < 6) {
			if(ParentFacingRight){//firing towards the right on the Ground
				if(ShotNumber == 1){
					DirectionTranslation = (Vector3.right * Movement) + ((Vector3.up * Movement)/3);
				}
				else if(ShotNumber == 2){
					DirectionTranslation = (Vector3.right * Movement) + ((Vector3.up * Movement) - ((Vector3.up * Movement)/3));
				}
				else if(ShotNumber == 3){
					DirectionTranslation = (Vector3.right * Movement);
				}
				else if(ShotNumber == 4){
					DirectionTranslation = (Vector3.right * Movement) + ((Vector3.down * Movement)/9);
				}
				else{// == 5
					DirectionTranslation = (Vector3.right * Movement) + ((Vector3.up * Movement)/6);
				}
			}else{//firing towards the left on the Ground
				if(ShotNumber == 1){
					DirectionTranslation = (Vector3.left * Movement) + ((Vector3.up * Movement)/3);
				}
				else if(ShotNumber == 2){
					DirectionTranslation = (Vector3.left * Movement) + ((Vector3.up * Movement) - ((Vector3.up * Movement)/3));
				}
				else if(ShotNumber == 3){
					DirectionTranslation = (Vector3.left * Movement);
				}
				else if(ShotNumber == 4){
					DirectionTranslation = (Vector3.left * Movement) + ((Vector3.down * Movement)/9);
				}
				else{// == 5
					DirectionTranslation = (Vector3.left * Movement) + ((Vector3.up * Movement)/6);
				}
			}
		}else{//Firing from Wall
			if(ParentFacingRight){//Parent is on the Left Wall
				if(ShotNumber == 6){
					DirectionTranslation = ((Vector3.down * Movement)/4) + (Vector3.right * Movement);
				}else if(ShotNumber == 7){
					DirectionTranslation = (Vector3.down * Movement) + (Vector3.right * Movement);
				}else if(ShotNumber == 8){
					DirectionTranslation = (Vector3.right * Movement);
				}else if(ShotNumber == 9){
					DirectionTranslation = ((Vector3.down * Movement)/3) + (Vector3.right * Movement);
				}else{
					DirectionTranslation = ((Vector3.down * Movement)/2) + (Vector3.right * Movement);
				}
			}else{
				if(ShotNumber == 6){
					DirectionTranslation = ((Vector3.down * Movement)/4) + ((Vector3.left * Movement));
				}else if(ShotNumber == 7){
					DirectionTranslation = (Vector3.down * Movement) + (Vector3.left * Movement);
				}else if(ShotNumber == 8){
					DirectionTranslation = (Vector3.left * Movement);
				}else if(ShotNumber == 9){
					DirectionTranslation = ((Vector3.down * Movement)/3) + (Vector3.left * Movement);
				}else{
					DirectionTranslation = ((Vector3.down * Movement)/2) + (Vector3.left * Movement);
				}
			}
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
			if(!trigger.gameObject.GetComponent<MegamanMovement>().CurrentlyInvulnerable){
				trigger.gameObject.GetComponent<MegamanMovement>().CurrentlyInvulnerable = true;
				trigger.gameObject.GetComponent<MegamanMovement>().Health -= 1;
				trigger.gameObject.GetComponent<MegamanMovement>().StartCoroutine("InvulnerabilityFrames", transform.parent.gameObject.GetComponent<NeonTigerAI>().FacingRight);
			}
			Destroy (gameObject);
		} 
	}

	public void SetShotNumber(float WhichNumber){
		ShotNumber = WhichNumber;
	}
}
