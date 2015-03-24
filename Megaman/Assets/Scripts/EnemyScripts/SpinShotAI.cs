using UnityEngine;
using System.Collections;

public class SpinShotAI : MonoBehaviour {

	public float Health;
	bool Dieing = false;
	bool FacingRight;
	//keep track of whether we are diagonal or not
	//beginning state is we are not diagonal
	bool LeftRight;
	Animator SpinShotAnim;

	//prefab for our projectile
	public GameObject SpinShotProjectile;

	Transform PlayerPos;
	float AttackRadius;
	bool CurrentlyAttacking;
	float ShotHeightAdjustment = 0.3f;
	GameObject [] Projectile = new GameObject[2];
	public float AnimationDelay;

	public AudioSource BusterShotSoundSource;
	public AudioClip [] BusterShotSound;

	// Use this for initialization
	void Start () {
		SpinShotAnim = GetComponent<Animator> ();
		PlayerPos = GameObject.Find ("Character").transform;
		CurrentlyAttacking = false;
		transform.parent.gameObject.GetComponent<EnemySpawnPoint> ().FacingRight = false;
		LeftRight = true;
		if (transform.GetComponentInParent<EnemySpawnPoint> ().UpsideDown) {
			FlipVertically();		
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		FacingRight = transform.parent.gameObject.GetComponent<EnemySpawnPoint> ().FacingRight;

		if (Health <= 0 && !Dieing) {
			Dieing = true;
			BusterShotSoundSource.PlayOneShot(BusterShotSound[2]);
			Invoke ("InvokeDeath", 0.75f);
		}

		AttackRadius = Mathf.Abs (transform.position.x	- PlayerPos.position.x);
		if(!CurrentlyAttacking && !Dieing && AttackRadius < 10){
			CurrentlyAttacking = true;
			StartCoroutine("AttackCooldown");
		}

		if (PlayerPos.position.x - transform.position.x < 0 && FacingRight) {
			Flip ();
		} 
		else if (PlayerPos.position.x - transform.position.x > 0 && !FacingRight) {
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

	void FlipVertically()
	{
		//FacingRight = !FacingRight;
		Vector3 theScale = transform.localScale;
		theScale.y *= -1;
		transform.localScale = theScale;
		ShotHeightAdjustment *= -1;
	}

	IEnumerator AttackCooldown(){
		if(LeftRight){//left right shots
			Projectile[0] = Instantiate (SpinShotProjectile, new Vector3(transform.position.x + 0.6f,transform.position.y,transform.position.z), Quaternion.identity) as GameObject;
			Projectile[0].transform.parent = this.transform;

			Projectile[1] = Instantiate (SpinShotProjectile, new Vector3(transform.position.x - 0.6f,transform.position.y,transform.position.z), Quaternion.identity) as GameObject;
			Projectile[1].transform.parent = this.transform;
		}else{//diagonal shots
			Projectile[0] = Instantiate (SpinShotProjectile, new Vector3(transform.position.x + 0.35f,transform.position.y + ShotHeightAdjustment,transform.position.z), Quaternion.identity) as GameObject;
			Projectile[0].transform.parent = this.transform;
			
			Projectile[1] = Instantiate (SpinShotProjectile, new Vector3(transform.position.x - 0.35f,transform.position.y + ShotHeightAdjustment,transform.position.z), Quaternion.identity) as GameObject;
			Projectile[1].transform.parent = this.transform;
		}

		//need code to rotate sprite here
		LeftRight = !LeftRight;

		yield return new WaitForSeconds (0.4f);
		SpinShotAnim.SetBool ("Rotate", true);
		yield return new WaitForSeconds (AnimationDelay);
		SpinShotAnim.SetBool ("Rotate", false);

		if (!LeftRight) {
			SpinShotAnim.SetBool ("ReadyToAttackDiagonal", true);
		}else{
			SpinShotAnim.SetBool ("ReadyToAttackLeftRight", true);
		}


		yield return new WaitForSeconds (AnimationDelay);

		if (!LeftRight) {
			SpinShotAnim.SetBool ("ReadyToAttackDiagonal", false);
		}else{
			SpinShotAnim.SetBool ("ReadyToAttackLeftRight", false);
		}
		CurrentlyAttacking = false;
	}

	void OnTriggerEnter2D(Collider2D trigger){
		if (trigger.gameObject.tag == "BusterShot") {
			BusterShotSoundSource.PlayOneShot (BusterShotSound[0]);
			Health -= 1f;
		}
		else if (trigger.gameObject.tag == "BusterShotMedium") {
			BusterShotSoundSource.PlayOneShot (BusterShotSound[1]);
			Health -= 2f;
		}
		else if (trigger.gameObject.tag == "BusterShotLarge") {
			BusterShotSoundSource.PlayOneShot (BusterShotSound[1]);
			Health -= trigger.gameObject.GetComponent<LargeBusterShot>().ShotDamage;
		}
	}

	void InvokeDeath(){
		transform.parent.gameObject.GetComponent<EnemySpawnPoint>().IsThisEnemyAlive = false;
		Destroy(gameObject);
	}
}
