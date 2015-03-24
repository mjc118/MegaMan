using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {

    public float LifeDuration;
    float InitialCreationTime;

	void Start () {
        InitialCreationTime = Time.time;
	}
	
	void FixedUpdate () {
        if (Time.time - InitialCreationTime > LifeDuration)
        {
            Destroy(gameObject);
        }
	}
}
