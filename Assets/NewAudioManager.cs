using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAudioManager : MonoBehaviour
{

    public static NewAudioManager inst;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayAmbient();
    }

    // Update is called once per frame
    void Update()
    {
        if(UnityEngine.InputSystem.Keyboard.current.endKey.wasReleasedThisFrame) {
            source.PlayOneShot(testClip, testScale);
        }
    }
    public float testScale;
    public AudioClip testClip;

    public AudioSource source;
    public AudioClip maliciousUnfiltered;
    public AudioClip maliciousFiltered;
    public AudioClip MaliciousRuleChanged;
    public AudioClip BadFilterRule;
    public AudioClip GoodFilterRule;

    public AudioClip PenaltyBoing;

    public AudioClip Countdown;
    public AudioClip Winning;
    public AudioClip Losing;

    public AudioSource ambient;
    //public AudioClip ambientClip;

    public void PlayOneShot(AudioClip clip, float vScale = 1f)
    {
        source.PlayOneShot(clip, vScale);
    }
    
    void PlayAmbient()
    {
        //ambient.PlayDelayed(10f);
        ambient.Play();
    }

}
