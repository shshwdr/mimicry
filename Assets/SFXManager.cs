using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : Singleton<SFXManager>
{
    public AudioClip boxMove;
    public AudioClip boxDie;
    public AudioClip enemyDie;
    public AudioClip boxAttack;
    public AudioClip finishLevel;
    public AudioClip positive;
    public AudioClip negative;

    public void PlayerSFX(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
