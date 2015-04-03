using UnityEngine;
using System.Collections;

public class ArmouredArmadilloAI : MonoBehaviour {

    Animator ArmadilloAnim;
    Animator BossHealthBar;

    public float Health;
    bool Dieing = false;
    public float DeathAnimDuration;
    public float InvulnerabilityFrame;
    public bool CurrentlyInvulnerable = false;
    public bool ArmorRaised = false;

    public Transform GroundCheck;
    public bool OnGround;
    public LayerMask WhatIsTerrain;

    public bool FacingRight;
    bool Attacking = false;

    public bool Spawning = true;

    public float RollSpeed;

    bool Rolling = false;
    bool InitialRollTowardsPlayer = true;//makes the initial direction be left or right towards player
    float GroundYPosition;
    int CurrentWallHits = 0;
    int MaxWallHits;
    /*Two bools to keep track of what direction our rolling should go based on what
     * direction we were previously going.
     * If we hit the wall triggers named "Left" or "Right" we invert CurrentDirectionIsLeft assuming since the boss
     * Initially Spawns on the right side of the room he will initially roll left and upward
     * If we hit a wall trigger with the name "Ceiling" or "Floor" we will invert CurrentDirectionIsUp
     */
    bool CurrentDirectionIsLeft = true;
    bool CurrentDirectionIsUp = true;
    
    Vector3 DirectionTranslation;
    //effects how much our Roll is pulled to the left or right
    float VerticalPull = 5;
    //effects how much our Roll is pulled upward or downard vs left or right
    float HorizontalPull = 10;

    int MaxProjectilesToFire;

    Transform PlayerPos;
    public AudioClip[] SoundClips;
    public AudioSource SoundEffects;
    AudioSource MainAudio;

    public GameObject[] ArmadilloProjectilePrefab;
    public GameObject[] DeathExplosions;

    void Awake()
    {
        MainAudio = GameObject.Find("Character").GetComponent<AudioSource>();
        MainAudio.clip = SoundClips[0];//boss entrance clip
        MainAudio.Play();
        Spawning = true;
        GameObject.Find("Character").GetComponent<Animator>().SetBool("BossIsSpawning", true);
    }

    // Use this for initialization
    void Start()
    {
        ArmadilloAnim = GetComponent<Animator>();
        GameObject.Find("Main Camera/BossHealthBar").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Main Camera/BossHealthBar/HealthMissing").GetComponent<SpriteRenderer>().enabled = true;
        BossHealthBar = GameObject.Find("Main Camera/BossHealthBar/HealthMissing").GetComponent<Animator>();
        PlayerPos = GameObject.Find("Character").transform;
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = false;
        StartCoroutine("InitialSpawn");
    }
	
	// Update is called once per frame
	void Update () {
        FacingRight = transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight;

        BossHealthBar.SetInteger("HealthMissing", 30 - (int)Health);

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
            ArmadilloAnim.SetBool("Dieing", true);
            MainAudio.Stop();
            StartCoroutine("Death");
        }

        //Returns True/False WhereCircleIsGenerated, Radius, WhatIsAllowedToCollideWithUs
        OnGround = Physics2D.OverlapCircle(GroundCheck.position, 0.1f, WhatIsTerrain);
        ArmadilloAnim.SetBool("OnGround", OnGround);

        if (!Spawning && !Dieing)
        {
            if (Rolling)
            {
                if (CurrentWallHits > MaxWallHits && OnGround)
                {
                    Rolling = false;
                    StartCoroutine("ExitRoll");
                }
                else if (InitialRollTowardsPlayer)
                {
                    if (CurrentDirectionIsLeft)
                    {
                        DirectionTranslation = ((Vector3.left * RollSpeed)/10);
                    }
                    else
                    {
                        DirectionTranslation = ((Vector3.right * RollSpeed)/10);
                    }
                    transform.Translate(DirectionTranslation);
                }
                else
                {

                    if (CurrentDirectionIsLeft && CurrentDirectionIsUp)//we are going left and up
                    {
                        DirectionTranslation = (((Vector3.left * RollSpeed) / VerticalPull) + ((Vector3.up * RollSpeed) / HorizontalPull));
                    }
                    else if (CurrentDirectionIsLeft && !CurrentDirectionIsUp)//we are going left and down
                    {
                        DirectionTranslation = (((Vector3.left * RollSpeed) / VerticalPull) + ((Vector3.down * RollSpeed) / HorizontalPull));
                    }
                    else if (!CurrentDirectionIsLeft && CurrentDirectionIsUp)//we are going right and up
                    {
                        DirectionTranslation = (((Vector3.right * RollSpeed) / VerticalPull) + ((Vector3.up * RollSpeed) / HorizontalPull));
                    }
                    else//we are going right and down
                    {
                        DirectionTranslation = (((Vector3.right * RollSpeed) / VerticalPull) + ((Vector3.down * RollSpeed) / HorizontalPull));
                    }

                    //transform.position = Vector3.MoveTowards(transform.position, transform.position + DirectionTranslation, Time.deltaTime * RollSpeed);
                    transform.Translate(DirectionTranslation);
                }
            }

            if (!Rolling)
            {
                if (PlayerPos.position.x - transform.position.x < 0 && FacingRight)
                {
                    Flip();
                }
                else if (PlayerPos.position.x - transform.position.x > 0 && !FacingRight)
                {
                    Flip();
                }
            }

            if (!Attacking)
            {
                Attacking = true;
                int AttackRoll = Random.Range(0, 11);
                Debug.Log(AttackRoll);

                if (AttackRoll < 5)
                {
                    if(FacingRight){ CurrentDirectionIsLeft = false; }
                    else{CurrentDirectionIsLeft = true;}

                    InitialRollTowardsPlayer = true;
                    CurrentDirectionIsUp = true;

                    CurrentWallHits = 0;
                    MaxWallHits = Random.Range(7, 11);
                    StartCoroutine("EnterRoll");
                }
                else if(AttackRoll < 8)//Firing Projectiles
                {
                    MaxProjectilesToFire = Random.Range(3, 7);
                    StartCoroutine("FireProjectiles");
                }
                else { Attacking = false; }
            }
        }
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = FacingRight;
	}

    IEnumerator FireProjectiles()
    {
        Vector3 ShotPos;
        GameObject Projectile;
        int CurrentProjectilesFired = 0;
        bool CurrentlyFiring = false;
        while (CurrentProjectilesFired < MaxProjectilesToFire)
        {
            if (!CurrentlyFiring)
            {
                ArmadilloAnim.SetBool("FiringProjectile", true);
                CurrentlyFiring = true;
                yield return new WaitForSeconds(0.25f);
                ++CurrentProjectilesFired;
                if (FacingRight)
                {
                    ShotPos = new Vector3(transform.position.x + 0.2f, transform.position.y + 0.2f, transform.position.z);
                    Projectile = Instantiate(ArmadilloProjectilePrefab[0], ShotPos, Quaternion.identity) as GameObject;
                    Projectile.transform.parent = this.transform;
                    Projectile.GetComponent<ArmadilloShot>().SetShotNumber(1);
                }
                else 
                {
                    ShotPos = new Vector3(transform.position.x - 0.2f, transform.position.y + 0.2f, transform.position.z);
                    Projectile = Instantiate(ArmadilloProjectilePrefab[0], ShotPos, Quaternion.identity) as GameObject;
                    Projectile.transform.parent = this.transform;
                    Projectile.GetComponent<ArmadilloShot>().SetShotNumber(2);
                }

                yield return new WaitForSeconds(0.75f);
                ArmadilloAnim.SetBool("FiringProjectile", false);
                yield return new WaitForSeconds(0.25f);
               
                CurrentlyFiring = false;
            }
        }
        Invoke("DelaySettingAttackFalse", 1.5f);
        yield return 0;
    }

    IEnumerator EnterRoll()
    {
        ArmadilloAnim.SetBool("Rolling", true);
        float EnterRollTime = 0;

        while (EnterRollTime < 0.5f)
        {
            transform.Translate(Vector3.up * 0.01f);
            EnterRollTime += Time.deltaTime;
            yield return 0;
        }

        ArmadilloAnim.SetBool("EnterRoll", true);
        yield return new WaitForSeconds(1f);
        Rolling = true;
    }

    IEnumerator ExitRoll()
    {
        ArmadilloAnim.SetBool("EnterRoll", false);
        ArmadilloAnim.SetBool("ExitRoll", true);
        yield return new WaitForSeconds(1f);
        Vector3 TargetPosition = transform.position;
        TargetPosition.y = GroundYPosition;

        while (transform.position != TargetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, Time.deltaTime * 8f);
            //transform.Translate(Vector3.down * 0.01f);
            yield return 0;
        }

        ArmadilloAnim.SetBool("ExitRoll", false);
        Invoke("DelaySettingRollingAnim", 1f);
        Invoke("DelaySettingAttackFalse", 1.5f);

    }

    IEnumerator InvulnerabilityFrames()
    {
        float InitialTime = InvulnerabilityFrame;

        while (InitialTime > 0)
        {
            InitialTime -= Time.deltaTime;
            yield return 0;
        }

        CurrentlyInvulnerable = false;
    }

    void DelaySettingRollingAnim()
    {
        ArmadilloAnim.SetBool("ExitRoll", false);
        ArmadilloAnim.SetBool("Rolling", Rolling);
    }

    void DelaySettingAttackFalse()
    {
        Attacking = false;
    }

    public void MegamanDied()
    {
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().IsThisEnemyAlive = false;
        GameObject.Find("Main Camera/BossHealthBar").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.Find("Main Camera/BossHealthBar/HealthMissing").GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject);
    }

    void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    IEnumerator InitialSpawn()
    {
        yield return new WaitForSeconds(2f);

        ArmadilloAnim.SetBool("Spawning", Spawning);

        while (!OnGround)
        {
            float Movement = Time.deltaTime * 7f;
            transform.Translate(Vector3.down * Movement);
            yield return null;
        }
        GroundYPosition = transform.position.y;
        yield return new WaitForSeconds(2f);
        Spawning = false;
        GameObject.Find("Character").GetComponent<Animator>().SetBool("BossIsSpawning", false);
        ArmadilloAnim.SetBool("Spawning", Spawning);
        MainAudio.clip = SoundClips[1];//boss entrance clip
        MainAudio.volume = 0.1f;
        MainAudio.Play();
        //delay for player to situate themselves
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

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (Rolling)//we don't want to invert these unless the boss is actually rolling
        {
            bool HitSomething = false;
            if (trigger.name == "LeftWall" && CurrentDirectionIsLeft)
            {
                HitSomething = true;
                CurrentDirectionIsLeft = !CurrentDirectionIsLeft;
            }
            else if (trigger.name == "RightWall" && !CurrentDirectionIsLeft)
            {
                HitSomething = true;
                CurrentDirectionIsLeft = !CurrentDirectionIsLeft;
            }

            if (trigger.name == "Ceiling" && CurrentDirectionIsUp)
            {
                HitSomething = true;
                CurrentDirectionIsUp = !CurrentDirectionIsUp;
            }
            else if (trigger.name == "Floor" && !CurrentDirectionIsUp)
            {
                HitSomething = true;
                CurrentDirectionIsUp = !CurrentDirectionIsUp;
            }

            if (HitSomething)
            {
                InitialRollTowardsPlayer = false;
                ++CurrentWallHits;
                //VerticalPull = Random.Range(15, 26);
                VerticalPull = Random.Range(35, 41);
                HorizontalPull = Random.Range(10, 21);
            }
        }

        if (ArmorRaised)
        {
            if (trigger.gameObject.tag == "BusterShotMedium" || trigger.gameObject.tag == "BusterShotLarge")
            {
                //do delta attk if hit change sound
                SoundEffects.PlayOneShot(SoundClips[3]);
            }
        }
        else if (!CurrentlyInvulnerable)
        {
            if (trigger.gameObject.tag == "BusterShot")
            {
                CurrentlyInvulnerable = true;
                StartCoroutine("InvulnerabilityFrames");
                SoundEffects.PlayOneShot(SoundClips[2]);
                Health -= 1f;
            }
            else if (trigger.gameObject.tag == "BusterShotMedium")
            {
                CurrentlyInvulnerable = true;
                StartCoroutine("InvulnerabilityFrames");
                SoundEffects.PlayOneShot(SoundClips[3]);
                Health -= 2f;
            }
            else if (trigger.gameObject.tag == "BusterShotLarge")
            {
                CurrentlyInvulnerable = true;
                StartCoroutine("InvulnerabilityFrames");
                SoundEffects.PlayOneShot(SoundClips[3]);
                Health -= 3f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D trigger)
    {
        //used so the boss can fall through the roof of the room
        if (trigger.gameObject.name == "BossCeilingTrigger")
        {
            trigger.gameObject.layer = 10;//10 = terrain
            trigger.isTrigger = false;
            transform.GetComponentInParent<EnemySpawnPoint>().BossCeilingCollider = trigger;
            GameObject.Find("EnemySpawnPoints/Boss/NoForwardMovement").collider2D.enabled = false;
        }
    }
}
