using UnityEngine;
using System.Collections;

public class JellyFishAI : MonoBehaviour {

    Animator JellyFishAnim;

    public float AttackDurationTime;//keeps track of how long it originally was
    float AttackDurationTimeLeft;//decrements in our coroutine
    public float AttackCooldownDuration;
    float AttackCooldownLeft = 0;

    public float Health;
    public float MoveSpeed;
    bool Dieing = false;
    bool FacingRight;

    bool Attacking = false;

    //need to track where player is in regards to spawn point
    Transform PlayerPos;

    public GameObject DeathExplosion;

    public AudioSource BusterShotSoundSource;
    public AudioClip[] BusterShotSound;

    // Use this for initialization
    void Start()
    {
        JellyFishAnim = GetComponent<Animator>();
        PlayerPos = GameObject.Find("Character").transform;

        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = false;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        FacingRight = transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight;

        if (Health <= 0 && !Dieing)
        {
            Dieing = true;
            Instantiate(DeathExplosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            InvokeDeath();
        }

        if (!Dieing)
        {
            if (AttackCooldownLeft > 0) { AttackCooldownLeft -= Time.deltaTime; }

            float DistanceFromPlayer = Mathf.Sqrt(Mathf.Pow(PlayerPos.position.x - transform.position.x, 2) + Mathf.Pow(PlayerPos.position.y - transform.position.y, 2));

            if (!Attacking && AttackCooldownLeft <= 0 && DistanceFromPlayer < 3f)
            {
                if (Random.Range(1, 10) < 4)
                {
                    Attacking = true;
                    JellyFishAnim.SetBool("Attack", Attacking);
                    AttackDurationTimeLeft = AttackDurationTime;
                    StartCoroutine("Attack");
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, PlayerPos.position, Time.deltaTime * MoveSpeed);
                }
            }
            else if (!Attacking && DistanceFromPlayer < 15f)
            {
                transform.position = Vector3.MoveTowards(transform.position, PlayerPos.position, Time.deltaTime * MoveSpeed);
            }

            if (!Attacking)
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
        }
        //set the public bool in the parent that is referenced by MegamanMovement to it's correct state
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = FacingRight;
    }

    //flip the sprites
    void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    IEnumerator Attack()
    {
        while (AttackDurationTimeLeft > 0)
        {
            AttackDurationTimeLeft -= Time.deltaTime;
            yield return 0;
        }
        Attacking = false;
        JellyFishAnim.SetBool("Attack", Attacking);

        AttackCooldownLeft = AttackCooldownDuration;
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "BusterShot")
        {
            BusterShotSoundSource.PlayOneShot(BusterShotSound[0]);
            Health -= 1f;
        }
        else if (trigger.gameObject.tag == "BusterShotMedium")
        {
            BusterShotSoundSource.PlayOneShot(BusterShotSound[1]);
            Health -= 2f;
        }
        else if (trigger.gameObject.tag == "BusterShotLarge")
        {
            BusterShotSoundSource.PlayOneShot(BusterShotSound[1]);
            Health -= trigger.gameObject.GetComponent<LargeBusterShot>().ShotDamage;
        }
    }

    void InvokeDeath()
    {
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().IsThisEnemyAlive = false;
        Destroy(gameObject);
    }
}
