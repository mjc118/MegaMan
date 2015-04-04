using UnityEngine;
using System.Collections;

public class MenuSelection : MonoBehaviour {

    public Transform SelectionBox;
    public Transform LocationDot;
    public Vector3[] SelectionBoxCoordinates;
    public Vector3[] LocationDotCoordinates;
    int CurrentSelection;

    public AudioClip[] MenuSounds;
    public AudioSource SoundEffectSource;

    /*
     * 0 index = Toxic SeaHorse
     * 1 index = Armoured Armadillo
     * 2 index = Neon Tiger
     */

	// Use this for initialization
	void Start () {
        //by default our Selection starts on the far left
        CurrentSelection = 0;
        SelectionBox.position = SelectionBoxCoordinates[0];
        //LocationDot.position = LocationDotCoordinates[0];
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Left") && CurrentSelection > 0)
        {
            --CurrentSelection;
            SelectionBox.position = SelectionBoxCoordinates[CurrentSelection];
            //LocationDot.position = LocationDotCoordinates[CurrentSelection];
            SoundEffectSource.PlayOneShot(MenuSounds[0]);
        }
        else if (Input.GetButtonDown("Right") && CurrentSelection < 2)
        {
            ++CurrentSelection;
            SelectionBox.position = SelectionBoxCoordinates[CurrentSelection];
            //LocationDot.position = LocationDotCoordinates[CurrentSelection];
            SoundEffectSource.PlayOneShot(MenuSounds[0]);
        }
	}
}
