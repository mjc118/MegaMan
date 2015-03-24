using UnityEngine;
using System.Collections;

public class SeaHorseAI : MonoBehaviour {

    Animator SeaHorseAnim;

    public float Health;
    public float MoveSpeed;
    bool Dieing = false;
    bool FacingRight;

    bool Spawning = true;
    bool Attacking = false;

    //need to track where player is in regards to spawn point
    Transform PlayerPos;
    float InitialXPosition;

    public float InitialYPosition;
    public GameObject DeathExplosion;

    public AudioSource BusterShotSoundSource;
    public AudioClip[] BusterShotSound;
	
    // Use this for initialization
	void Start () {
        SeaHorseAnim = GetComponent<Animator>();
        PlayerPos = GameObject.Find("Character").transform;
        InitialYPosition = transform.position.y;
        InitialXPosition = transform.position.x;

        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = false;
        StartCoroutine("InitialSpawn");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        FacingRight = transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight;

        if (Mathf.Abs(transform.position.x - InitialXPosition) > 15f)
        {
            InvokeDeath();
        }

        if (Health <= 0 && !Dieing)
        {
            Dieing = true;
            Instantiate(DeathExplosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            InvokeDeath();
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
        else
        {
            if (!FacingRight) 
            {
                transform.Translate(Vector3.left * MoveSpeed);
            }
            else
            {
                transform.Translate(Vector3.right * MoveSpeed);
            }
        }

        //set the public bool in the parent that is referenced by MegamanMovement to it's correct state
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = FacingRight;

	}

    IEnumerator InitialSpawn()
    {
        Spawning = true;
        SeaHorseAnim.SetBool("Spawning", Spawning);

        while (Mathf.Abs(transform.position.y - InitialYPosition) < 5f)
        {
            transform.Translate(Vector3.up * MoveSpeed);
            yield return 0;
        }

        yield return new WaitForSeconds(0.5f);
        
        Spawning = false;
        SeaHorseAnim.SetBool("Spawning", Spawning);

        yield return new WaitForSeconds(2f);

        Attacking = true;
    }

    //flip the sprites
    void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
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
