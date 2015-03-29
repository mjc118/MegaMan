using UnityEngine;
using System.Collections;

public class AxeAI : MonoBehaviour {

    Animator AxeAnim;
    public Transform TerrainCheck;
    public bool HitTerrain;
    public LayerMask WhatIsTerrain;

    bool Spawning = true;

    Vector3 ArcBegin, ArcEnd; //VectorPositions that mark the start and end
    public float ArcHeight; //desired parabola height
    float ArcTimeElapsed = 0;//How many seconds into the current jump we are
    public float ArcDuration;//#seconds a jump takes to complete

	// Use this for initialization
	void Start () {
        if (GetComponentInParent<MinerAI>().FacingRight)
        {
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        ArcBegin = transform.position;
        ArcEnd = GameObject.Find("Character").transform.position;
	    AxeAnim = GetComponent<Animator>();
        Invoke("SetSpawnFalse", 0.25f);
	}
	
	// Update is called once per frame
	void Update () {
	    HitTerrain = Physics2D.OverlapCircle(TerrainCheck.position, 0.1f, WhatIsTerrain);

        if(HitTerrain){
            this.GetComponentInParent<MinerAI>().AxeIsAlive = false;
            Destroy(gameObject);
        }

        if (!Spawning)
        {
            float objectT = ArcTimeElapsed % ArcDuration; //completes the parabola trip in JumpDuration Time
            transform.position = ArcParabola(ArcBegin, ArcEnd, ArcHeight, (objectT / ArcDuration) * 4);

            ArcTimeElapsed += Time.deltaTime;
        }
	}

    void SetSpawnFalse(){
        Spawning = false;
        AxeAnim.SetBool("Spawning", Spawning);
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.tag == "Player")
        {
            if (!trigger.gameObject.GetComponent<MegamanMovement>().CurrentlyInvulnerable)
            {
                trigger.GetComponent<MegamanMovement>().CurrentlyInvulnerable = true;
                trigger.GetComponent<MegamanMovement>().Health -= 2;
                trigger.GetComponent<MegamanMovement>().StartCoroutine("InvulnerabilityFrames", this.GetComponentInParent<MinerAI>().FacingRight);
            }
            this.GetComponentInParent<MinerAI>().AxeIsAlive = false;
            Destroy(gameObject);
        }
    }

    Vector3 ArcParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;

        if (true)//Mathf.Abs(start.y - end.y) < 0.1f)
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
}
