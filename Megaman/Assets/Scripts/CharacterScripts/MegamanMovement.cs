using UnityEngine;
using System.Collections;

public class MegamanMovement : MonoBehaviour {
	public float Health;
	public float InvulnerabilityFrame;
	public Vector2 KnockBackForce;
	public bool CurrentlyInvulnerable = false;

	//used to handle ladders
	public float climbSpeed;
	public bool OnLadder = false;
	//needed due to be able to jump on ladder
	public bool TouchingLadder = false;
	private float verticalMovement;

	public float maxSpeed;
    public float JumpSpeed;
    public float JumpDuration;//const
    float jmpDuration;//modified
    bool JumpOnCooldown = false;
    
	public bool FacingRight = true;

	Animator anim;
    Animator HealthBarAnim;

	//for sliding
	public bool onWall = false;
	public Transform wallCheck;

	//for jump/fall animation
	public bool OnGround = false;
	public Transform groundCheck;
	float groundRadius = 0.2f;
	public LayerMask WhatIsGround;
	public LayerMask WhatIsLadder;
    

	//for dashing
	public Vector2 dashSpeed;
	public bool canDash = true;
	public bool CurrentlyDashing = false;
	public float dashCooldown = 2f;

	//used to edit our Players colliders based on actions they are taking
	public BoxCollider2D HeadBoxCollider;
	public CircleCollider2D DashCircleCollider;

	//used to handle respawning and the animation
	public bool Respawning = true;

	public AudioSource SoundEffects;
	public AudioClip[] MegaManSoundClips;

	//used to handle respawning
	public Transform SpawnPoint;
	public AudioSource DeathSound;

	bool InWater = false;
	public bool IsBossSpawning = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
        HealthBarAnim = GameObject.Find("Main Camera/MMXHealthBar/HealthMissing").GetComponent<Animator>();
		StartCoroutine ("InitialSpawn");
	}

	void FixedUpdate()
	{		
		HealthBarAnim.SetInteger ("Health", 16 - (int)Health);
		if (Health <= 0) {
			StartCoroutine("Death");		
		}

		//Returns True/False WhereCircleIsGenerated, Radius, WhatIsAllowedToCollideWithUs
		OnGround = Physics2D.OverlapCircle (groundCheck.position, groundRadius, WhatIsGround);
		anim.SetBool ("Ground", OnGround);

		onWall = Physics2D.OverlapCircle (wallCheck.position, groundRadius, WhatIsGround);
		anim.SetBool ("Wall", onWall);

		//used to determine if we are allowed to jump from being on a ladder
		TouchingLadder = Physics2D.OverlapCircle (groundCheck.position, groundRadius, WhatIsLadder);

		//for jumping speed
		anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);

		float move = Input.GetAxis ("Horizontal");

		verticalMovement = Input.GetAxis ("Vertical");

		if (!CurrentlyDashing && !Respawning && !IsBossSpawning) { //don't allow movement while we are dashing or boss is spawning
						anim.SetFloat ("Speed", Mathf.Abs (move));
											
						if(OnLadder){
							rigidbody2D.velocity = new Vector2(0, verticalMovement * climbSpeed);
						}
						else{
							rigidbody2D.velocity = new Vector2 (move * maxSpeed, rigidbody2D.velocity.y);
						}

						if (move > 0 && !FacingRight) {
								Flip ();
								dashSpeed.x *= -1;
						} else if (move < 0 && FacingRight) {
								Flip ();
								dashSpeed.x *= -1;
						}
				}
	}

	void Flip()
	{
		FacingRight = !FacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	// Update is called once per frame
	void Update () 
	{
        //check if our boss is alive so we can restrict our movement when he spawns
        if (GameObject.Find("EnemySpawnPoints/Boss").GetComponent<EnemySpawnPoint>().IsThisEnemyAlive)
        {
            if (Application.loadedLevelName == "NeonTigerStage")
            {
                IsBossSpawning = GameObject.Find("EnemySpawnPoints/Boss").GetComponentInChildren<NeonTigerAI>().Spawning;
            }
            else if (Application.loadedLevelName == "WaterStage")
            {
                IsBossSpawning = GameObject.Find("EnemySpawnPoints/Boss").GetComponentInChildren<ToxicSeaHorseAI>().Spawning;
            }
            else if (Application.loadedLevelName == "TunnelRhinoStage")
            {
                IsBossSpawning = GameObject.Find("EnemySpawnPoints/Boss").GetComponentInChildren<ArmouredArmadilloAI>().Spawning;
            }
            //else if(){} fill in our other levels
        }

		if (!CurrentlyDashing && !Respawning && !IsBossSpawning) {
			if ((OnGround && canDash) && Input.GetButtonDown ("Dash")) {
				StartCoroutine ("Dash", 0.005f);//start our Coroutine to handle Dashing
			}

			if ((OnGround || TouchingLadder) && Input.GetButtonDown ("Jump")) {
				anim.SetBool ("Ground", false);
                
				if(OnLadder){
					OnLadder = false;
					anim.SetBool("OnLadder", OnLadder);
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpSpeed);
                    rigidbody2D.gravityScale = 1f;
                    jmpDuration = 0f;
				}
			}

            if (Input.GetButton("Jump") && (jmpDuration < JumpDuration / 1000))
            {
                //jmpDuration += Time.deltaTime;
                
                if ((onWall || OnGround) && !OnLadder)
                {

                    float horizontal = Input.GetAxis("Horizontal");

                    bool wallHit = false;
                    int wallHitDirection = 0;
                    if (onWall && horizontal > 0 && FacingRight)
                    {
                        wallHit = true;
                        wallHitDirection = -1;
                    }
                    else if(onWall && horizontal < 0 && !FacingRight)
                    {
                        wallHit = true;
                        wallHitDirection = 1;
                    }

                    if (!wallHit)
                    {
                        if (OnGround)
                        {
                            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpSpeed);
                            //jmpDuration = 0f;
                        }
                    }
                    else
                    {
                        rigidbody2D.velocity = new Vector2(JumpSpeed * wallHitDirection, JumpSpeed);
                        //jmpDuration = 0;
                    }

                }
                else
                {
                    jmpDuration += Time.deltaTime;
                    Debug.Log(jmpDuration);
                    if (jmpDuration < JumpDuration / 1000)
                    {
                        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JumpSpeed);
                    }
                }

                if (jmpDuration > JumpDuration / 1000)
                {
                    JumpOnCooldown = true;
                    Invoke("JumpCooldown", 0.5f);
                }
            }
            else if(!JumpOnCooldown)
            {
                if (jmpDuration > 0)
                {
                    jmpDuration -= Time.deltaTime;
                }
            }
            
		}
	}

    void JumpCooldown()
    {
        JumpOnCooldown = false;
    }

	IEnumerator InitialSpawn(){
		Respawning = true;
		anim.SetBool ("Respawning", Respawning);
		while (!OnGround) {
			yield return null;
				}
		Respawning = false;
		anim.SetBool ("Respawning", Respawning);
		}

	IEnumerator Dash(float DashDuration)
	{
		float time = 0;
		//used to keep track of original value
		float Temp = DashCircleCollider.radius;

		DashCircleCollider.radius = 0.055f;//shift circle collider on feet so sprite isn't floating
		HeadBoxCollider.enabled = false;//disable box collider on our head so we dont' hit anything
		CurrentlyDashing = true;//set to disallow movement
		canDash = false;//don't allow chain dashing
		anim.SetBool ("CurrentlyDashing", CurrentlyDashing);

		while (DashDuration > time) 
		{
			time += Time.deltaTime;
			rigidbody2D.velocity = dashSpeed;
			yield return 0;
		}

		yield return new WaitForSeconds (dashCooldown);
		canDash = true;
		CurrentlyDashing = false;
		anim.SetBool ("CurrentlyDashing", CurrentlyDashing);
		HeadBoxCollider.enabled = true;
		DashCircleCollider.radius = Temp;
	}

	void OnCollisionEnter2D(Collision2D collide)
	{
		if (collide.gameObject.tag == "Spikes") 
		{
			StartCoroutine("Death"); 
		}

		if (collide.gameObject.tag == "RockManProjectile") {
			SoundEffects.PlayOneShot (MegaManSoundClips [0]);
		} 
		else if (collide.gameObject.tag == "Enemy" && !CurrentlyInvulnerable) {
			CurrentlyInvulnerable = true;
            Health -= collide.gameObject.GetComponentInParent<EnemySpawnPoint>().CollisionDamage;
			StartCoroutine("InvulnerabilityFrames", collide.transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight);
		}
	}

	void OnTriggerEnter2D(Collider2D trigger){

        if (trigger.tag == "Spikes")
        {
            StartCoroutine("Death");
        }

        if (!CurrentlyInvulnerable)
        {
            if (trigger.gameObject.tag == "Enemy")
            {
                CurrentlyInvulnerable = true;
                Health -= trigger.gameObject.GetComponentInParent<EnemySpawnPoint>().CollisionDamage;
                StartCoroutine("InvulnerabilityFrames", trigger.GetComponentInParent<EnemySpawnPoint>().FacingRight);
            }
            else if (trigger.gameObject.tag == "SpinShotProjectile" || trigger.gameObject.tag == "AcidBall")
            {
                SoundEffects.PlayOneShot(MegaManSoundClips[0]);
            }
        }

	}

	void OnTriggerStay2D(Collider2D trigger){
		if (trigger.gameObject.tag == "Ladder" && verticalMovement != 0f) {
			OnLadder = true;
			rigidbody2D.gravityScale = 0;
			anim.SetBool("OnLadder", OnLadder);
		}
        if (!CurrentlyInvulnerable)
        {
            if (trigger.gameObject.tag == "Enemy")
            {
                CurrentlyInvulnerable = true;
                Health -= trigger.gameObject.GetComponentInParent<EnemySpawnPoint>().CollisionDamage;
                StartCoroutine("InvulnerabilityFrames", trigger.transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight);
            }
        }
	}

	void OnTriggerExit2D(Collider2D trigger){
		if (trigger.gameObject.tag == "Ladder") {
			OnLadder = false;
			rigidbody2D.gravityScale = 1;
			anim.SetBool("OnLadder", OnLadder);
		}

		if (trigger.gameObject.tag == "WaterEntry") {
			rigidbody2D.gravityScale = 0.75f;
			InWater = true;
			if(!trigger.gameObject.GetComponentInParent<MeshSortingLayer>().BeginWaveCreation){
				trigger.gameObject.GetComponentInParent<MeshSortingLayer>().BeginWaveCreation = true;
				trigger.gameObject.GetComponentInParent<MeshSortingLayer>().InitialCreationTime = Time.time;
				trigger.gameObject.GetComponentInParent<MeshSortingLayer>().OriginPosition = transform.position;
			}
		}
		else if(trigger.gameObject.tag == "WaterExit"){
			InWater = false;
			rigidbody2D.gravityScale = 1;
		}
	}

	IEnumerator InvulnerabilityFrames(bool EnemyFacingRight){
		float time = 0;

		if (EnemyFacingRight && KnockBackForce.x < 0) {
			KnockBackForce.x *= -1;		
		} else if (!EnemyFacingRight && KnockBackForce.x > 0) {
			KnockBackForce.x *= -1;		
		}

		rigidbody2D.AddForce (KnockBackForce * 10);
		
		SoundEffects.PlayOneShot (MegaManSoundClips [0]);

		while (InvulnerabilityFrame > time) 
		{
			time += Time.deltaTime;
			yield return 0;
		}
		CurrentlyInvulnerable = false;
	}

	IEnumerator Death()
	{ 
		//Reset the Boss Room
		Collider2D BossCeilingCollider = GameObject.Find ("EnemySpawnPoints/Boss").GetComponent<EnemySpawnPoint> ().BossCeilingCollider;
		if(BossCeilingCollider != null){
			BossCeilingCollider.gameObject.layer = 15;//bossRoomLayer
			BossCeilingCollider.isTrigger = true;
			GameObject.Find ("EnemySpawnPoints/Boss/BossSpawnTrigger").collider2D.isTrigger = true;
			GameObject.Find ("EnemySpawnPoints/Boss/NoForwardMovement").collider2D.enabled = true;
			if(GameObject.Find ("EnemySpawnPoints/Boss").GetComponent<EnemySpawnPoint>().IsThisEnemyAlive){
				gameObject.GetComponent<AudioSource>().clip = MegaManSoundClips[1];
				gameObject.GetComponent<AudioSource>().Play ();
                if (Application.loadedLevelName == "NeonTigerStage")
                {
                    GameObject.Find("EnemySpawnPoints/Boss").GetComponentInChildren<NeonTigerAI>().MegamanDied();
                }
                else if (Application.loadedLevelName == "WaterStage")
                {
                    GameObject.Find("EnemySpawnPoints/Boss").GetComponentInChildren<ToxicSeaHorseAI>().MegamanDied();
                }
                else if (Application.loadedLevelName == "TunnelRhinoStage")
                {
                    GameObject.Find("EnemySpawnPoints/Boss").GetComponentInChildren<ArmouredArmadilloAI>().MegamanDied();
                }
                //else if(){} fill in our other levels
			}

		}

		//handle megaman's death
		DeathSound.Play ();
		anim.SetBool ("Death", true);
		yield return new WaitForSeconds(0.5f);
		
		//immediately say we are respawning so we don't initiate falling animation
		Respawning = true;
		anim.SetBool ("Respawning", Respawning);
		//anim.SetBool ("Ground", false);
		anim.SetBool ("Death", false);
		transform.position = SpawnPoint.position;
		StartCoroutine ("Respawn");
	}
	
	IEnumerator Respawn(){
		Health = 16f;
		//delay so that OnGround doesn't return true instantly
		yield return new WaitForSeconds (0.5f);
		while (!OnGround) {
			yield return null;
		}
		//yield return new WaitForSeconds (1f);
		Respawning = false;
		anim.SetBool ("Respawning", Respawning);
	}
}
