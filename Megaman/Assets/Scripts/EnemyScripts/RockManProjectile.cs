using UnityEngine;
using System.Collections;

public class RockManProjectile : MonoBehaviour {

	public float Velocity;

	Transform PlayerPos;
	// Use this for initialization
	void Start () {
		PlayerPos = GameObject.Find ("Character").transform;
		float HorizontalDistance = PlayerPos.position.x - transform.position.x;
		float VerticalDistance = PlayerPos.position.y - transform.position.y;
		float GravitationalForce = rigidbody2D.gravityScale * 10f;
		
		float Discriminant = Mathf.Pow (Velocity, 4) - 2 * Mathf.Pow (Velocity,2) * GravitationalForce * 
							VerticalDistance - Mathf.Pow (GravitationalForce,2) * Mathf.Pow (HorizontalDistance,2);

		float Dividend;
		//if our Discriminant is Negative, then it is not possible to reach our target unless our
		//Projectile had a higher velocity since taking the Square Root of a negative number gives an imaginary result
		if (Discriminant > 0) {
			float QuadraticResult = Mathf.Sqrt (Discriminant);
			//if we wanted our arc to be much higher, then + Quadratic Result instead of -
			Dividend = Mathf.Pow (Velocity,2) - QuadraticResult;
		} 
		else {
			Dividend = Mathf.Pow (Velocity,2) - 1;
		}
			
		//Debug.Log (Dividend);

		float theta = Mathf.Atan (Dividend / (GravitationalForce * HorizontalDistance));
						
						//flip the Vector if our target is on our left
		rigidbody2D.velocity = new Vector2 ((HorizontalDistance > 0 ? 1 : -1) * Velocity * Mathf.Cos (theta),
		                                    (HorizontalDistance > 0 ? 1 : -1) * Velocity * Mathf.Sin (theta));
				//}
		}

	void OnCollisionEnter2D(Collision2D collide)
	{
		if (collide.gameObject.tag == "Player") {
			collide.gameObject.GetComponent<MegamanMovement>().Health -= 2;
			Destroy (gameObject);
		} 
		else if (collide.gameObject.tag == "Terrain") {
			Destroy (gameObject);	
		}
	}
}
