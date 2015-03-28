using UnityEngine;
using System.Collections;

public class ConveyorBeltAI : MonoBehaviour {

    public float Speed;
    public bool GoingRight;
    public bool PlayerOnMe = false;
    public Transform PlayerPos;
    Vector3 DirectionTranslation;

	// Use this for initialization
	void Start () {
        if (GoingRight)
        {
            DirectionTranslation = (Vector3.right * Speed);
        }
        else
        {
            DirectionTranslation = (Vector3.left * Speed);
        }
	}

    void FixedUpdate()
    {
        if (PlayerOnMe)
        {
            PlayerPos.Translate(DirectionTranslation);
        }
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.tag == "Player")
        {
            PlayerOnMe = true;
        }
    }

    void OnTriggerExit2D(Collider2D trigger)
    {
        if (trigger.tag == "Player")
        {
            PlayerOnMe = false;
        }
    }
}
