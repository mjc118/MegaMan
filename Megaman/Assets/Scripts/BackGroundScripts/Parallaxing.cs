using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour {
	public Transform[] backgrounds;
	private float[] parallaxScales;	//The proportion of the camera's movement to move the backgrounds by
	public float smoothing = 1f;	//How smooth the parallax is going to be

	private Transform cam;			//reference to main camera
	private Vector3 previousCamPos; //position of camera in previous frame

	void Awake(){
		cam = Camera.main.transform;
	}
	// Use this for initialization
	void Start () {

		previousCamPos = cam.position;

		parallaxScales = new float[backgrounds.Length];

		for (int i = 0; i < backgrounds.Length; ++i) {
			parallaxScales[i] = backgrounds[i].position.z * -1;		
		}
	}
	
	// Update is called once per frame
	void Update () {
		//for each background
		for (int i = 0; i < backgrounds.Length; ++i) {
			float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];

			//set a target x position which is the current position plus the parallax
			float backgroundTargetPosX = backgrounds[i].position.x + parallax;

			//create a target position which is the background's current position with it's target x position
			Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
		
			//fade between current position and the target position using Lerp
			backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
		}

		//set the previousCamPos to camera's position at the end of frame
		previousCamPos = cam.position;
	}
}
