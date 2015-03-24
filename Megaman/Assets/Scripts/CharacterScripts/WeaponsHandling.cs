using UnityEngine;
using System.Collections;

public class WeaponsHandling : MonoBehaviour {

    public bool CurrentlyDashing;
    public bool Respawning;
    public bool IsBossSpawning;
    public bool onWall;
    public bool OnGround;

    Animator anim;

    //used for firing Projectiles
    public GameObject[] BusterShotPrefabs;
    public float CurrentShotsInPlay;
    public float ShotLimit;
    public float ChargeShotReq;//default time that is used to reset our Current
    private float CurrentChargeShotTimeLeft;//time that actually decreases to zero
    public AudioSource SoundEffects;
    public AudioSource BusterChargeSource;
    public AudioClip[] BusterSounds;
    bool InitialChargeSoundPlayed = false;

    void Awake()
    {
        BusterChargeSource.enabled = false;
    }

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        CurrentShotsInPlay = 0;
        CurrentChargeShotTimeLeft = ChargeShotReq;
	}
	
	// Update is called once per frame
	void Update () {
        CurrentlyDashing = GetComponentInParent<MegamanMovement>().CurrentlyDashing;
        Respawning = GetComponentInParent<MegamanMovement>().Respawning;
        IsBossSpawning = GetComponentInParent<MegamanMovement>().IsBossSpawning;
        onWall = GetComponentInParent<MegamanMovement>().onWall;
        OnGround = GetComponentInParent<MegamanMovement>().OnGround;

        if (!CurrentlyDashing && !Respawning && !IsBossSpawning)
        {
            if (Input.GetButtonDown("BusterShot") && (CurrentShotsInPlay < ShotLimit))
            {
                FireBuster("SmallShot");
            }

            if (Input.GetButton("BusterShot"))
            {
                if (!InitialChargeSoundPlayed && CurrentChargeShotTimeLeft < ChargeShotReq - 0.25f)
                {
                    SoundEffects.PlayOneShot(BusterSounds[1]);
                    InitialChargeSoundPlayed = true;
                }
                else if (InitialChargeSoundPlayed && CurrentChargeShotTimeLeft < ChargeShotReq - 1.15f)
                {
                    BusterChargeSource.enabled = true;
                }
                CurrentChargeShotTimeLeft -= Time.deltaTime;
            }
            else if (CurrentChargeShotTimeLeft < (ChargeShotReq / 2) && CurrentChargeShotTimeLeft > 0)
            {
                FireBuster("MediumShot");
                CurrentChargeShotTimeLeft = ChargeShotReq;
                BusterChargeSource.enabled = false;
                InitialChargeSoundPlayed = false;
            }
            else if (CurrentChargeShotTimeLeft < 0)
            {
                FireBuster("LargeShot");
                CurrentChargeShotTimeLeft = ChargeShotReq;
                BusterChargeSource.enabled = false;
                InitialChargeSoundPlayed = false;
            }
            else if (CurrentChargeShotTimeLeft != ChargeShotReq)
            {
                CurrentChargeShotTimeLeft = ChargeShotReq;
                BusterChargeSource.enabled = false;
                InitialChargeSoundPlayed = false;
            }
        }
	}

    void FireBuster(string WhichShot)
    {
        ++CurrentShotsInPlay;
        anim.SetBool("FiringBuster", true);

        StartCoroutine("InstantiateShot", WhichShot);

        //delay setting our bool back to false so animation has a chance to play
        Invoke("SetFiringBusterToFalse", 0.2f);
    }

    void SetFiringBusterToFalse()
    {
        anim.SetBool("FiringBuster", false);
    }

    IEnumerator InstantiateShot(string WhichShot)
    {
        //edits initial position so shot starts at his buster
        Vector3 ShotPos;
        //delay the shot being created so the animation lines up
        yield return new WaitForSeconds(0.1f);
        if (WhichShot == "SmallShot")
        {
            ShotPos = new Vector3(transform.position.x + (((onWall && !OnGround) ? -1f : 1f) * (transform.localScale.x / 5)), transform.position.y + 0.1f);
            SoundEffects.PlayOneShot(BusterSounds[0]);
            Instantiate(BusterShotPrefabs[0], ShotPos, Quaternion.identity);
        }
        else if (WhichShot == "MediumShot")
        {
            ShotPos = new Vector3(transform.position.x + (((onWall && !OnGround) ? -1f : 1f) * (transform.localScale.x / 3.5f)), transform.position.y + 0.1f);
            SoundEffects.PlayOneShot(BusterSounds[3]);
            Instantiate(BusterShotPrefabs[1], ShotPos, Quaternion.identity);
        }
        else if (WhichShot == "LargeShot")
        {
            ShotPos = new Vector3(transform.position.x + (((onWall && !OnGround) ? -1f : 1f) * (transform.localScale.x / 3.25f)), transform.position.y + 0.1f);
            SoundEffects.PlayOneShot(BusterSounds[4]);
            Instantiate(BusterShotPrefabs[2], ShotPos, Quaternion.identity);
        }
    }
}
