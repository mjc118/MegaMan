﻿using UnityEngine;
using System.Collections;

public class ArmadilloShot : MonoBehaviour {

    public float ShotSpeed;

    private Vector2 ParentPos;
    private Vector2 InitialPos;
    public float ShotDestructionDistance = 10f;
    float DistanceGone;

    bool ParentFacingRight;
    bool FacingRight = false;

    Vector3 DirectionTranslation;
    //from 1-5, used to identify which shot I am to
    //deduce which direction i should go
    float ShotNumber = 0;
    float DeltaShotSpeed;

	// Use this for initialization
	void Start () {
	    InitialPos.x = transform.position.x;
		InitialPos.y = transform.position.y;
		ParentPos.x = transform.parent.transform.position.x;
		ParentPos.y = transform.parent.transform.position.y;

        DeltaShotSpeed = GetComponentInParent<ArmouredArmadilloAI>().DeltaAttackShotSpeed;

        ParentFacingRight = GetComponentInParent<ArmouredArmadilloAI>().FacingRight;
        if (ParentFacingRight)
            Flip();

		//loop until our ShotNumberIsAssigned
		while (ShotNumber == 0);

        this.transform.parent = null;//so our parent flipping doesn't effect us

		float Movement = ShotSpeed * Time.deltaTime;

        if (ShotNumber == 1)//firing right
        {
            if (ShotSpeed == DeltaShotSpeed && !ParentFacingRight) { Flip(); }
            DirectionTranslation = (Vector3.right * Movement);
        }
        else if (ShotNumber == 2)//firing left
        {
            if (ShotSpeed == DeltaShotSpeed && ParentFacingRight) { Flip(); }
            DirectionTranslation = (Vector3.left * Movement);
        }
        else if (ShotNumber == 3)//firing diagonally left
        {
            if (ShotSpeed == DeltaShotSpeed && ParentFacingRight) { Flip(); }
            transform.eulerAngles = new Vector3(0, 0, -45);
            DirectionTranslation = (Vector3.left * Movement);
        }
        else if (ShotNumber == 4)//firing up
        {
            if (ShotSpeed == DeltaShotSpeed && ParentFacingRight) { Flip(); }
            transform.eulerAngles = new Vector3(0, 0, -90);
            DirectionTranslation = (Vector3.left * Movement);
        }
        else
        {// == 5 firing diagonally right
            transform.eulerAngles = new Vector3(0, 0, -135);
            if (ShotSpeed == DeltaShotSpeed && ParentFacingRight) { Flip(); }
            DirectionTranslation = (Vector3.left * Movement);
        }
	}

    void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Update()
    {
        //perform our movement
        transform.Translate(DirectionTranslation);

        DistanceGone = Mathf.Sqrt(Mathf.Pow((transform.position.x - InitialPos.x), 2) + Mathf.Pow((transform.position.y - InitialPos.y), 2));
        if (DistanceGone > ShotDestructionDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "Player")
        {
            if (!trigger.gameObject.GetComponent<MegamanMovement>().CurrentlyInvulnerable)
            {
                trigger.gameObject.GetComponent<MegamanMovement>().CurrentlyInvulnerable = true;
                trigger.gameObject.GetComponent<MegamanMovement>().Health -= 2;
                trigger.gameObject.GetComponent<MegamanMovement>().StartCoroutine("InvulnerabilityFrames", FacingRight);
            }
            Destroy(gameObject);
        }
    }

    public void SetShotNumber(float WhichNumber)
    {
        ShotNumber = WhichNumber;
    }

    public void SetShotSpeed(float Speed)
    {
        ShotSpeed = Speed;
    }
}
