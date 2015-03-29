using UnityEngine;
using System.Collections;

public class MinerAI : MonoBehaviour {

    Animator MinerAnim;

    public float Health;
    bool Dieing = false;
    public bool FacingRight;

    //need to track where player is in regards to spawn point
    Transform PlayerPos;

    public GameObject DeathExplosion;
    public GameObject AxePrefab;
    public bool AxeIsAlive = false;
    float AttackCooldown = 3f;

    public AudioSource BusterShotSoundSource;
    public AudioClip[] BusterShotSound;

	// Use this for initialization
	void Start () {
        MinerAnim = GetComponent<Animator>();
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
            if (PlayerPos.position.x - transform.position.x < 0 && FacingRight)
            {
                Flip();
            }
            else if (PlayerPos.position.x - transform.position.x > 0 && !FacingRight)
            {
                Flip();
            }


            if (!AxeIsAlive)
            {
                if (Mathf.Abs(transform.position.x - PlayerPos.position.x) < 15f)
                {
                    if (AttackCooldown > 0)
                    {
                        AttackCooldown -= Time.deltaTime;
                    }
                    else
                    {
                        AttackCooldown = 3f;
                        AxeIsAlive = true;
                        MinerAnim.SetBool("Attack", true);
                        Invoke("ThrowAxe", 0.3f);
                    }
                }
            }
        }
        //set the public bool in the parent that is referenced by MegamanMovement to it's correct state
        transform.parent.gameObject.GetComponent<EnemySpawnPoint>().FacingRight = FacingRight;
	}

    void ThrowAxe()
    {
        Vector3 AxePos;
        
        AxePos = new Vector3(transform.position.x + (transform.localScale.x/-10.4727745098f), transform.position.y + 0.4041f, transform.position.z);

        GameObject Axe = Instantiate(AxePrefab, AxePos, Quaternion.identity) as GameObject;
        Axe.transform.parent = this.transform;

        Invoke("SetAttackFalse", 1f);
    }

    void SetAttackFalse()
    {
        MinerAnim.SetBool("Attack", false);
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
