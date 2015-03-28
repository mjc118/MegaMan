using UnityEngine;
using System.Collections;

public class DirtyWaterAI : MonoBehaviour {

    Animator DirtyWaterAnim;

    public float MoveSpeed;
    public float GravityChange;

    public LayerMask WhatIsGround;
    public Transform GroundCheck;
    bool OnGround = false;

    bool Spawning = true;
    bool DestroyingSelf = false;

	// Use this for initialization
	void Start () {
        DirtyWaterAnim = this.GetComponent<Animator>();
        Invoke("SetSpawningFalse", 1f);

	}
	
	// Update is called once per frame
	void Update () {
        OnGround = Physics2D.OverlapCircle(GroundCheck.position, 0.1f, WhatIsGround);

        if (!DestroyingSelf)
        {
            if (OnGround)
            {
                DestroyingSelf = true;
                Invoke("DestroySelf", 1f);
                DirtyWaterAnim.SetBool("HitGround", OnGround);
            }
            else if (!Spawning)
            {
                Vector3 DirectionTranslation = (Vector3.down * MoveSpeed);
                transform.Translate(DirectionTranslation);
            }
        }

        
	}

    void DestroySelf()
    {
        this.GetComponentInParent<DirtyWaterSpawn>().AmIAlive = false;
        Destroy(gameObject);
    }

    void SetSpawningFalse()
    {
        DirtyWaterAnim.SetBool("Spawning", false);
        Spawning = false;
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.tag == "Player")
        {
            trigger.rigidbody2D.gravityScale = GravityChange;
        }
    }

    void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.tag == "Player")
        {
            trigger.rigidbody2D.gravityScale = 1f;
        }
    }
}
