using UnityEngine;
using System.Collections;

public class RockManAI : MonoBehaviour {

	public float Health;
	bool Dieing = false;
	bool FacingRight;
	Animator RockManAnim;

	BoxCollider2D HitBox;
	//handle initial spawning anim
	bool Grounded = false;
	public Transform EnemyGroundCheck;
	float groundRadius = 0.2f;
	public LayerMask WhatIsGround;

	//prefab for our projectile
	public GameObject RockProjectile;
	GameObject Projectile;//used to keep track of child projectile that will spawn

	bool Spawning = true;

	//need to track where player is in regards to spawn point
	Transform PlayerPos;
	float AttackRadius;
	bool CurrentlyAttacking;
	public float AttackCooldownDuration;

	public AudioSource BusterShotSoundSource;
	public AudioClip [] BusterShotSound;

	// Use this for initialization
	void Start () {
		HitBox = GetComponent<BoxCollider2D> ();
		RockManAnim = GetComponent<Animator> ();
		PlayerPos = GameObject.Find ("Character").transform;
		transform.parent.gameObject.GetComponent<EnemySpawnPoint> ().FacingRight = false;
		CurrentlyAttacking = false;
		StartCoroutine ("InitialSpawn");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		FacingRight = transform.parent.gameObject.GetComponent<EnemySpawnPoint> ().FacingRight;

		Grounded = Physics2D.OverlapCircle (EnemyGroundCheck.position, groundRadius, WhatIsGround);
		RockManAnim.SetBool ("Grounded", Grounded);

		if (Health <= 0 && !Dieing) {
			Dieing = true;
			BusterShotSoundSource.PlayOneShot(BusterShotSound[2]);
			Invoke ("InvokeDeath", 0.75f);
		}

		if (!Spawning && !Projectile && !Dieing) {
			AttackRadius = Mathf.Abs (transform.position.x	- PlayerPos.position.x);
			//10f
			if(!CurrentlyAttacking && AttackRadius < 15f){
				RockManAnim.SetBool("Attacking", true);
				HitBox.size /= 0.7f;
				CurrentlyAttacking = true;
				Invoke ("SetAttackingFalse", 0.5f);
			}
		}

		if (PlayerPos.position.x - transform.position.x < 0 && FacingRight) {
			Flip ();
		} else if (PlayerPos.position.x - transform.position.x > 0 && !FacingRight) {
			Flip ();		
		}

		//set the public bool in the parent that is referenced by MegamanMovement to it's correct state
		transform.parent.gameObject.GetComponent<EnemySpawnPoint> ().FacingRight = FacingRight;
	}

	//flip the sprites
	void Flip()
	{
		FacingRight = !FacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void SetAttackingFalse(){
		RockManAnim.SetBool ("Attacking", false);
		HitBox.size *= 0.7f;
		StartCoroutine("AttackCooldown");

	}

	void OnCollisionEnter2D(Collision2D collide){
		if (collide.gameObject.tag == "BusterShot") {
			BusterShotSoundSource.PlayOneShot (BusterShotSound[0]);
			Health -= 1f;
		}
		else if (collide.gameObject.tag == "BusterShotMedium") {
			BusterShotSoundSource.PlayOneShot (BusterShotSound[1]);
			Health -= 2f;
		}
		else if (collide.gameObject.tag == "BusterShotLarge") {
			BusterShotSoundSource.PlayOneShot (BusterShotSound[1]);
			Health -= collide.gameObject.GetComponent<LargeBusterShot>().ShotDamage;
		}
	}

	IEnumerator AttackCooldown(){
		Projectile = Instantiate (RockProjectile, new Vector3(transform.position.x,transform.position.y,transform.position.z), Quaternion.identity) as GameObject;
		Projectile.transform.parent = this.transform;
		yield return new WaitForSeconds (AttackCooldownDuration);
		CurrentlyAttacking = false;
	}
	IEnumerator InitialSpawn(){
		Spawning = true;
		RockManAnim.SetBool ("Spawning", Spawning);
		while (!Grounded) {
			yield return null;
		}
		HitBox.size /= 1.5f;
		yield return new WaitForSeconds (0.8f);
		HitBox.size *= 1.5f;
		Spawning = false;
		RockManAnim.SetBool ("Spawning", Spawning);
	}

	void InvokeDeath(){
		transform.parent.gameObject.GetComponent<EnemySpawnPoint>().IsThisEnemyAlive = false;
		Destroy(gameObject);
	}
}
