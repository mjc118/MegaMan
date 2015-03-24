using UnityEngine;
using System.Collections;

public class NeonTigerAI : MonoBehaviour {

	Animator NeonTigerAnim;

	public float Health;
	bool Dieing = false;
    public float DeathAnimDuration;
	public float InvulnerabilityFrame;
	public bool CurrentlyInvulnerable = false;
	public float MoveSpeed;

	public Transform GroundCheck;
	public Transform WallCheck;
	public bool OnGround;
	public bool OnWall;
	public LayerMask WhatIsTerrain;
	public bool FacingRight;
	public bool FlippedSpritesForHittingWall = false;
	public bool JumpingToWall = false;//used to transition from ground to wall
	public bool JumpingOffWall = false;//used to transition from wall to ground

	public bool Spawning = true;
	bool Attacking = false;
	//float AttackRoll;//randomly generate a number to determine our attack

	Transform PlayerPos;
	public AudioClip[] SoundClips;
	public AudioSource SoundEffects;
	AudioSource MainAudio;

	public GameObject NeonTigerProjectilePrefab;
    public GameObject[] DeathExplosions;

	void Awake(){
		MainAudio = GameObject.Find ("Character").GetComponent<AudioSource> ();
		MainAudio.clip = SoundClips [0];//boss entrance clip
		MainAudio.Play ();
		Spawning = true;
		GameObject.Find ("Character").GetComponent<Animator>().SetBool ("BossIsSpawning", true);
	}
	// Use this for initialization
	void Start () {
		NeonTigerAnim = GetComponent<Animator> ();
		PlayerPos = GameObject.Find ("Character").transform;
		transform.parent.gameObject.GetComponent<EnemySpawnPoint> ().FacingRight = false;
		StartCoroutine ("InitialSpawn");
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Dieing)
        {
            //controls how long our death animation plays before destroying our game object
            DeathAnimDuration -= Time.deltaTime;
            if (DeathAnimDuration < 0)
            {
                GetComponentInParent<EnemySpawnPoint>().IsThisEnemyAlive = false;
                GetComponentInParent<EnemySpawnPoint>().SpawnBoss = false;
                Destroy(gameObject);
            }
        }

        if (Health <= 0 && !Dieing)
        {
            Dieing = true;
            NeonTigerAnim.SetBool("TakeDamage", true);
            MainAudio.Stop();
            StartCoroutine("Death");
        }

		FacingRight = transform.parent.gameObject.GetComponent<EnemySpawnPoint> ().FacingRight;

		float Movement = (MoveSpeed * Time.deltaTime);
		Vector3 DirectionTranslation;

		//Returns True/False WhereCircleIsGenerated, Radius, WhatIsAllowedToCollideWithUs
		OnGround = Physics2D.OverlapCircle (GroundCheck.position, 0.2f, WhatIsTerrain);
		NeonTigerAnim.SetBool ("OnGround", OnGround);
		
		OnWall = Physics2D.OverlapCircle (WallCheck.position, 0.2f, WhatIsTerrain);
		NeonTigerAnim.SetBool ("OnWall", OnWall);
        if (!Dieing)
        {
            if (JumpingToWall && !FlippedSpritesForHittingWall && OnWall)
            {
                //set this to false when you jump off the wall
                FlippedSpritesForHittingWall = true;
                JumpingToWall = false;
                Flip();
                Invoke("DelaySetAttackToFalse", 1.5f);
            }

            if (JumpingToWall && !OnWall)
            {
                if (!FacingRight)
                {
                    DirectionTranslation = (Vector3.left * Movement) + ((Vector3.up * Movement) - ((Vector3.up * Movement) / 3));
                }
                else
                {
                    DirectionTranslation = (Vector3.right * Movement) + ((Vector3.up * Movement) - ((Vector3.up * Movement) / 3));
                }

                transform.Translate(DirectionTranslation);

            }
            else if (JumpingOffWall && !OnGround)
            {
                if (!FacingRight)
                {
                    DirectionTranslation = /*((Vector3.left * Movement)/12) + */((Vector3.down * Movement) - ((Vector3.down * Movement) / 2));
                }
                else
                {
                    DirectionTranslation = /*((Vector3.right * Movement)/12) + */((Vector3.down * Movement) - ((Vector3.down * Movement) / 2));
                }

                transform.Translate(DirectionTranslation);

            }
            else if (!OnWall && OnGround)
            {
                if (JumpingOffWall)
                {
                    JumpingOffWall = false;
                    NeonTigerAnim.SetBool("JumpingOffWall", false);
                    Invoke("DelaySetAttackToFalse", 1.5f);
                }

                if (PlayerPos.position.x - transform.position.x < 0 && FacingRight)
                {
                    Flip();
                }
                else if (PlayerPos.position.x - transform.position.x > 0 && !FacingRight)
                {
                    Flip();
                }
            }

            if (!Spawning && !Attacking && !JumpingToWall && !JumpingOffWall && !Dieing)
            {
                Attacking = true;

                AttackRolling(Random.Range(1, 10));
            }
        }
		transform.parent.gameObject.GetComponent<EnemySpawnPoint> ().FacingRight = FacingRight;
	}

	void Flip()
	{
		FacingRight = !FacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void AttackRolling(float Roll){
		if(Roll < 7){
			if(OnGround){
				ScatterShot();
			}
			else{
				JumpingOffWall = true;
				NeonTigerAnim.SetBool("JumpingOffWall", true);
				FlippedSpritesForHittingWall = false;
			}	
		}else{
			if(OnGround){
				JumpingToWall = true;
			}
			else{//FireFromWall
				ScatterShot();
			}	
		}
	}

	void DelaySetAttackToFalse(){
		Attacking = false;
	}

	public void MegamanDied(){
		transform.parent.gameObject.GetComponent<EnemySpawnPoint>().IsThisEnemyAlive = false;
		Destroy (gameObject);
	}

	void ScatterShot(){

		Vector3 ShotPos;
		if(OnGround){
			NeonTigerAnim.SetBool ("GroundShot", true);
			ShotPos = new Vector3(transform.position.x - (transform.localScale.x/2.3f),transform.position.y - (transform.localScale.y/6.5f),transform.position.z);
		}else{
			NeonTigerAnim.SetBool ("WallShot", true);
			ShotPos = new Vector3(transform.position.x - (transform.localScale.x/7.146420284f),transform.position.y + (transform.localScale.y/40.85022866f),transform.position.z);
		}

		StartCoroutine ("InstantiateShots", ShotPos);
	}

	IEnumerator InstantiateShots(Vector3 ShotPos){

		GameObject Projectile;

		if(OnGround){
			yield return new WaitForSeconds(0.5f);
			//Instantiate our five shots with the proper delay
			for(int i = 1; i < 6; ++i){
				Projectile = Instantiate (NeonTigerProjectilePrefab, ShotPos, Quaternion.identity) as GameObject;
				Projectile.transform.parent = this.transform;
				Projectile.GetComponent<NeonTigerProjectileAI>().SetShotNumber (i);
				yield return new WaitForSeconds(0.5f);
			}
		}else{//WeAreInstantiating a WallShot
			yield return new WaitForSeconds(0.5f);
			for(int i = 6; i < 11; ++i){
				Projectile = Instantiate (NeonTigerProjectilePrefab, ShotPos, Quaternion.identity) as GameObject;
				Projectile.transform.parent = this.transform;
				Projectile.GetComponent<NeonTigerProjectileAI>().SetShotNumber (i);
				yield return new WaitForSeconds(0.5f);
			}
		}

		Attacking = false;
		NeonTigerAnim.SetBool ("GroundShot", false);
		NeonTigerAnim.SetBool ("WallShot", false);
	}

	IEnumerator InitialSpawn(){
		yield return new WaitForSeconds(2f);

		//Spawning = true;
		NeonTigerAnim.SetBool ("Spawning", Spawning);

		while(!OnGround){
			float Movement = Time.deltaTime * MoveSpeed;
			transform.Translate(Vector3.down * Movement);
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		Spawning = false;
		GameObject.Find ("Character").GetComponent<Animator>().SetBool ("BossIsSpawning", false);
		NeonTigerAnim.SetBool ("Spawning", Spawning);
		MainAudio.clip = SoundClips [1];//boss entrance clip
		MainAudio.volume = 0.1f;
		MainAudio.Play ();
		//delay for player to situate themselves
	}

	IEnumerator InvulnerabilityFrames(){
		float InitialTime = InvulnerabilityFrame;

		while (InitialTime > 0) {
			InitialTime -= Time.deltaTime;
			yield return 0;
		}

		CurrentlyInvulnerable = false;
	}

    IEnumerator Death()
    {
        int WhichExplosion = 0;
        while (true)
        {
            Instantiate(DeathExplosions[WhichExplosion], new Vector3(transform.position.x + Random.Range(-1f, 0.75f), transform.position.y + Random.Range(-1f, 0.7f), transform.position.z), Quaternion.identity);
            
            //alternate our explosions
            if (WhichExplosion == 0) { ++WhichExplosion; }
            else { --WhichExplosion; }

            yield return new WaitForSeconds(0.25f);
        }

    }

	void OnTriggerEnter2D(Collider2D trigger){
		if(!CurrentlyInvulnerable){
			if (trigger.gameObject.tag == "BusterShot") {
				CurrentlyInvulnerable = true;
				StartCoroutine("InvulnerabilityFrames");
				SoundEffects.PlayOneShot (SoundClips[2]);
				Health -= 1f;
			}
			else if (trigger.gameObject.tag == "BusterShotMedium") {
				CurrentlyInvulnerable = true;
				StartCoroutine("InvulnerabilityFrames");
				SoundEffects.PlayOneShot (SoundClips[3]);
				Health -= 2f;
			}
			else if (trigger.gameObject.tag == "BusterShotLarge") {
				CurrentlyInvulnerable = true;
				StartCoroutine("InvulnerabilityFrames");
				SoundEffects.PlayOneShot (SoundClips[3]);
				Health -= trigger.gameObject.GetComponent<LargeBusterShot>().ShotDamage;
			}
		}
	}

	void OnTriggerExit2D(Collider2D trigger){
		//used so the boss can fall through the roof of the room
		if (trigger.gameObject.name == "BossCeilingTrigger") {
			trigger.gameObject.layer = 10;//10 = terrain
			trigger.isTrigger = false;
			transform.GetComponentInParent<EnemySpawnPoint>().BossCeilingCollider = trigger;
			GameObject.Find ("EnemySpawnPoints/Boss/NoForwardMovement").collider2D.enabled = false;
		}
	}
}
