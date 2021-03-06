﻿using UnityEngine;
using System.Collections;

public class LargeBusterShot : MonoBehaviour {

	public float ShotSpeed;
	public float ShotDamage;
	
	private Transform myTransform;
	private float InitialPos;
	public float ShotDestructionDistance = 20f;

	bool Right = false;
	//reference our other script to figure out which direction the player is facing
	MegamanMovement Player;
    WeaponsHandling Buster;

	Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		//grabs the initial position of the shot
		InitialPos = transform.position.x;
		myTransform = transform;
		Player = GameObject.Find ("Character").GetComponent<MegamanMovement>();
        Buster = GameObject.Find("Character").GetComponent<WeaponsHandling>();

		if (Player.FacingRight && !Player.onWall) {
			Right = true;		
		}else if(!Player.FacingRight && Player.onWall && !Player.OnGround){
			Right = true;
		}else if(Player.FacingRight && Player.OnGround && Player.onWall){
			Right = true;
		}else{
			//if firing left we need to swap the sprite direction
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}

	// Update is called once per frame
	void Update () {
		float Movement = ShotSpeed * Time.deltaTime;
		anim.SetBool ("Creation", false);
	
		if (Right) {
			FireRight (Movement);		
		} 
		else {
			FireLeft (Movement);	
		}
	}
	
	void FireRight(float Move){
		myTransform.Translate (Vector3.right * Move);
		
		//if our buster shot has gone too far right
		if (InitialPos + ShotDestructionDistance < myTransform.position.x) {
			--Buster.CurrentShotsInPlay;
			Destroy (gameObject);
		}
	}
	
	void FireLeft(float Move){
		myTransform.Translate (Vector3.left * Move);
		
		//if our buster shot has gone too far left
		if (InitialPos - ShotDestructionDistance > myTransform.position.x) {
            --Buster.CurrentShotsInPlay;
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collide)
	{
		if (collide.gameObject.tag == "Enemy") 
		{
            --Buster.CurrentShotsInPlay;
			Destroy (gameObject);
		}
		if (collide.gameObject.tag == "RockManProjectile") {
			ShotDamage -= 2;
			Destroy (collide.gameObject);	
		}
	}

	void OnTriggerEnter2D(Collider2D trigger)
	{
        if (trigger.gameObject.tag == "Enemy" || trigger.gameObject.tag == "AcidBall") 
		{
            --Buster.CurrentShotsInPlay;
			Destroy (gameObject);
		}
	}
}
