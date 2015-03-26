using UnityEngine;
using System.Collections;

public class ToxicSeaHorseAI : MonoBehaviour {

    Animator ToxicSeaHorseAnim;
    Animator BossHealthBar;

    public float Health;
    bool Dieing = false;
    public float DeathAnimDuration;
    public float InvulnerabilityFrame;
    public bool CurrentlyInvulnerable = false;

    public Transform GroundCheck;
    public bool OnGround;
    public LayerMask WhatIsTerrain;

    public bool FacingRight;
    bool Jumping = false;
    bool Attacking = false;

    public bool Spawning = true;

    /*
     * Following variables are used for the construction of the parabolas used in the 
     * bosses jump animation
     */
    float objectT = 0; //timer for that object
    public Vector3 JumpBegin, JumpEnd; //transforms that mark the start and end
    public float JumpHeight; //desired parabola height
    Vector3 a, b; //Vector positions for start and end
    public float JumpTimeElapsed;//How many seconds into the current jump we are
    public float JumpDuration;//#seconds a jump takes to complete

    Transform PlayerPos;
    public AudioClip[] SoundClips;
    public AudioSource SoundEffects;
    AudioSource MainAudio;

    //public GameObject ToxicSeaHorseProjectilePrefab;
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
	void Start () {
        ToxicSeaHorseAnim = GetComponent<Animator>();
        GameObject.Find("Main Camera/BossHealthBar").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("Main Camera/BossHealthBar/HealthMissing").GetComponent<SpriteRenderer>().enabled = true;
        BossHealthBar = GameObject.Find("Main Camera/BossHealthBar/HealthMissing").GetComponent<Animator>();
        PlayerPos = GameObject.Find("Character").transform;
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = false;
        StartCoroutine("InitialSpawn");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
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
            //ToxicSeaHorseAnim.SetBool("TakeDamage", true);
            MainAudio.Stop();
            StartCoroutine("Death");
        }

        //Returns True/False WhereCircleIsGenerated, Radius, WhatIsAllowedToCollideWithUs
        OnGround = Physics2D.OverlapCircle(GroundCheck.position, 0.2f, WhatIsTerrain);
        ToxicSeaHorseAnim.SetBool("OnGround", OnGround);

        if (!Dieing && !Spawning)
        {
            if (Jumping)
            {
                a = JumpBegin; //Get vectors from the transforms
                b = JumpEnd;

                JumpTimeElapsed += Time.deltaTime;
                objectT = JumpTimeElapsed % JumpDuration; //completes the parabola trip in JumpDuration Time
                
                if (JumpTimeElapsed >= JumpDuration)
                {
                    Jumping = false;
                    ToxicSeaHorseAnim.SetBool("Jumping", Jumping);
                    transform.position = SampleParabola(a, b, JumpHeight, 1);
                }
                else
                {
                    transform.position = SampleParabola(a, b, JumpHeight, objectT / JumpDuration);
                }
                
            }

            if (!Attacking && !Jumping)
            {
                Jumping = true;
                JumpTimeElapsed = 0;
                ToxicSeaHorseAnim.SetBool("Jumping", Jumping);
                Attacking = true;
                JumpBegin = transform.position;
                Vector3 Temp = transform.position;
                Temp.x -= 5f;
                JumpEnd = Temp;
                
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

        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = FacingRight;
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

        //Spawning = true;
        ToxicSeaHorseAnim.SetBool("Spawning", Spawning);

        while (!OnGround)
        {
            float Movement = Time.deltaTime * 7f;
            transform.Translate(Vector3.down * Movement);
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        Spawning = false;
        GameObject.Find("Character").GetComponent<Animator>().SetBool("BossIsSpawning", false);
        ToxicSeaHorseAnim.SetBool("Spawning", Spawning);
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
        if (!CurrentlyInvulnerable)
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

    #region Parabola sampling function
    /// <summary>
    /// Get position from a parabola defined by start and end, height, and time
    /// </summary>
    /// <param name='start'>
    /// The start point of the parabola
    /// </param>
    /// <param name='end'>
    /// The end point of the parabola
    /// </param>
    /// <param name='height'>
    /// The height of the parabola at its maximum
    /// </param>
    /// <param name='t'>
    /// Normalized time (0->1)
    /// </param>S
    Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        //float parabolicT = t * 2 - 1;
        float parabolicT = t * 2 - 1;

        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }
    #endregion
}
