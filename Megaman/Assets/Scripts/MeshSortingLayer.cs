using UnityEngine;
using System.Collections;

public class MeshSortingLayer : MonoBehaviour {

	public string sortingLayerName;       // The name of the sorting layer .
	public int sortingOrder;      //The sorting order

	public float WaveAmplitude;
	public float WaveVelocity;
	public float Lambda;
	public float DecaySpeed;

	public bool BeginWaveCreation = false;
	public Vector3 OriginPosition;
	float ElapsedTime;
	public float InitialCreationTime;

	void Start ()
	{
		// Set the sorting layer and order.
		renderer.sortingLayerName = sortingLayerName;
		renderer.sortingOrder=sortingOrder;
	}

	void FixedUpdate(){
		//wave amplitude
		if (Input.GetKeyDown ("a")) {
			if(Input.GetKey (KeyCode.LeftShift)){
				WaveAmplitude += 1f;
			}
			else if(WaveAmplitude > 0){
				WaveAmplitude -= 1f;
			}
		}
		//wave length
		if (Input.GetKeyDown ("l")) {
			if(Input.GetKey (KeyCode.LeftShift)){
				Lambda += 0.1f;
			}
			else if(WaveAmplitude > 0){
				Lambda -= 0.1f;
			}
		}
		//wave velocity
		if (Input.GetKeyDown ("v")) {
			if(Input.GetKey (KeyCode.LeftShift)){
				WaveVelocity += 0.1f;
			}
			else if(WaveAmplitude > 0){
				WaveVelocity -= 0.1f;
			}
		}
	}

	void Update(){
		float Radius;
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices = mesh.vertices;

		if (BeginWaveCreation) {
			ElapsedTime = Time.time - InitialCreationTime;
			for(int i = 0; i < vertices.Length; ++i) {
				Radius = Mathf.Sqrt(Mathf.Pow (vertices[i].x - ((OriginPosition.x + 50f)/10) ,2f) + Mathf.Pow (vertices[i].z - (OriginPosition.z/10),2f));
				//if our wave has been in existence enough to have a radius as large as the possible
				//alter those vertices
				if(Radius >= WaveVelocity * ElapsedTime){
					vertices[i].y = WaveAmplitude * Mathf.Exp (-Radius - (DecaySpeed * ElapsedTime)) *
						Mathf.Cos (2 * Mathf.PI * ((Radius - (WaveVelocity * ElapsedTime)/Lambda)));
				}else{
					vertices[i].y /= 4f;
				}
			}
				
			if(ElapsedTime > 5f){
				BeginWaveCreation = false;
				for(int i = 0; i < vertices.Length; ++i) {
					vertices[i].y /= 4f;
				}

			}	

			mesh.vertices = vertices;
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
		}
	}
}
