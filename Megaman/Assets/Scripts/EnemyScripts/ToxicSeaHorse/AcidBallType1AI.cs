using UnityEngine;
using System.Collections;

public class AcidBallType1AI : MonoBehaviour {

    public float Health;
    public float MoveSpeed;
    public int MaxWallHits;//#of wall hits before we destroy ourselves
    public bool InitialDirectionIsR; //Set by ToxicSeaHorse on Instansiation

    public bool PrevMoveDirectionIsR;//Keeps track of the direction we were moving before hitting a wall so we know which direction to move after
    bool Spawning = true;

    public Transform TerrainCheck;
    public bool HitTerrain;
    public LayerMask WhatIsTerrain;

    Vector3 DirectionTranslation;

	// Use this for initialization
	void Start () {
        StartCoroutine("DelayInitialMovement");
        PrevMoveDirectionIsR = InitialDirectionIsR;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Health <= 0 || MaxWallHits == 0)
        {
            Destroy(gameObject);
        }

        if (!Spawning)
        {
            //Returns True/False WhereCircleIsGenerated, Radius, WhatIsAllowedToCollideWithUs
            HitTerrain = Physics2D.OverlapCircle(TerrainCheck.position, 0.2f, WhatIsTerrain);
            if (HitTerrain)//if we hit a wall we need to change our direction
            {
                --MaxWallHits;
                if (PrevMoveDirectionIsR)//if we were previously moving right we will now move left
                {
                    DirectionTranslation = (Vector3.left * MoveSpeed) + ((Vector3.up * MoveSpeed)/10);
                }
                else
                {
                    DirectionTranslation = (Vector3.right * MoveSpeed) + ((Vector3.up * MoveSpeed) / 10);
                }

                PrevMoveDirectionIsR = !PrevMoveDirectionIsR;
            }

            //perform our movement
            transform.Translate(DirectionTranslation);
        }
	}

    IEnumerator DelayInitialMovement()
    {
        yield return new WaitForSeconds(0.8f);
        if (InitialDirectionIsR)
        {
            DirectionTranslation = (Vector3.right * MoveSpeed);
        }
        else
        {
            DirectionTranslation = (Vector3.left * MoveSpeed);
        }
        Spawning = false;
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Player")
        {
            trigger.gameObject.GetComponent<MegamanMovement>().Health -= 2;
            Destroy(gameObject);
        } 

        if (trigger.gameObject.tag == "BusterShot")
        {
            Health -= 1f;
        }
        else if (trigger.gameObject.tag == "BusterShotMedium")
        {
            Health -= 2f;
        }
        else if (trigger.gameObject.tag == "BusterShotLarge")
        {
            Health -= 4f;
        }
        
    }
}
