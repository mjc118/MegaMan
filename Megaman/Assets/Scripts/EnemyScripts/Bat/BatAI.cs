using UnityEngine;
using System.Collections;

public class BatAI : MonoBehaviour {

    Animator BatAnim;

    public float Health;
    public float MoveSpeed;
    public float ChaseDistance;
    bool Dieing = false;
    bool FacingRight;

    bool Chasing = false;
    bool PlayerHit = false;
    float DisapearTime = 0;

    //need to track where player is in regards to spawn point
    Transform PlayerPos;
    Vector3 InitialPosition;

    public GameObject DeathExplosion;

    public AudioSource BusterShotSoundSource;
    public AudioClip[] BusterShotSound;

	// Use this for initialization
	void Start () {
        BatAnim = GetComponent<Animator>();
        PlayerPos = GameObject.Find("Character").transform;
        InitialPosition = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        FacingRight = transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight;

        float DistanceFromPlayer = Mathf.Sqrt(Mathf.Pow(PlayerPos.position.x - transform.position.x, 2) + Mathf.Pow(PlayerPos.position.y - transform.position.y, 2));


        if (Health <= 0 && !Dieing)
        {
            Dieing = true;
            Instantiate(DeathExplosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            InvokeDeath();
        }

        if (!Dieing)
        {
            if (PlayerHit)
            {
                if (DisapearTime < 2f)
                {
                    transform.Translate(((Vector3.up * MoveSpeed)/15) + ((Vector3.right * MoveSpeed) / 30));
                    DisapearTime += Time.deltaTime;

                }
                else
                {
                    transform.parent.gameObject.GetComponent<EnemySpawnPoint>().IsThisEnemyAlive = false;
                    Destroy(gameObject);
                }
            }
            else
            {
                if (DistanceFromPlayer < ChaseDistance)
                {
                    Chasing = true;
                    BatAnim.SetBool("Chasing", Chasing);
                    transform.position = Vector3.MoveTowards(transform.position, PlayerPos.position, Time.deltaTime * MoveSpeed);
                }
                else if (transform.position != InitialPosition)
                {
                    transform.position = Vector3.MoveTowards(transform.position, InitialPosition, Time.deltaTime * MoveSpeed);
                }
                else
                {
                    Chasing = false;
                    BatAnim.SetBool("Chasing", Chasing);
                }
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

    void InvokeDeath()
    {
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().IsThisEnemyAlive = false;
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.tag == "Player")
        {
            PlayerHit = true;
        }

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
}
