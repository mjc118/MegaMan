using UnityEngine;
using System.Collections;

public class EnemySpawnPoint : MonoBehaviour {

    public float CollisionDamage;

	public bool FacingRight = false;
	public bool IsThisEnemyAlive = false;
	public bool UpsideDown = false;
	public bool IsBoss = false;
	public bool SpawnBoss = false;
	public GameObject EnemyPrefab;
	public Collider2D BossCeilingCollider;
	Vector3 SpawnPosition;
	float SpawnBoundaryCheck;
    public float SpawnBoundaryUpperLimit;
    public float SpawnBoundaryLowerLimit;

	//need to track where player is in regards to spawn point
	Transform PlayerPos;
	// Use this for initialization
	void Start () {
		SpawnPosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		IsThisEnemyAlive = false;
		PlayerPos = GameObject.Find ("Character").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!IsBoss){
			if (!IsThisEnemyAlive) {
				//11f, 10f
				//calculate if player is within boundary to merit spawning an enemy
				SpawnBoundaryCheck = Mathf.Abs(transform.position.x	- PlayerPos.position.x);
                if (SpawnBoundaryCheck < SpawnBoundaryUpperLimit && SpawnBoundaryCheck > SpawnBoundaryLowerLimit)
                {
					Debug.Log ("Spawn");
					GameObject child = Instantiate (EnemyPrefab, SpawnPosition, Quaternion.identity) as GameObject;
					child.transform.parent = this.transform;
					IsThisEnemyAlive = true;
				}
			}
		}else{
			if(!IsThisEnemyAlive && SpawnBoss){
				GameObject child = Instantiate (EnemyPrefab, SpawnPosition, Quaternion.identity) as GameObject;
				child.transform.parent = this.transform;
				IsThisEnemyAlive = true;
				SpawnBoss = false;

			}
		}
	}
}
