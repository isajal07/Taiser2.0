using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsManager : MonoBehaviour
{

    public static EffectsManager inst;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NewRule(Destination destination, LightWeightPacket maliciousRule)
    {
        NewAudioManager.inst.source.PlayOneShot(NewAudioManager.inst.MaliciousRuleChanged);
    }

    public void MaliciousUnfilteredPacket(Destination destination, LightWeightPacket maliciousPacket)
    {
        NewAudioManager.inst.source.PlayOneShot(NewAudioManager.inst.maliciousUnfiltered);
    }

    public void MaliciousFilteredPacket(Destination destination, LightWeightPacket maliciousPacket)
    {
        NewAudioManager.inst.source.PlayOneShot(NewAudioManager.inst.maliciousFiltered);
    }

    public void GoodFilterApplied(Destination destination, LightWeightPacket filterRule)
    {
        NewAudioManager.inst.PlayOneShot(NewAudioManager.inst.GoodFilterRule);
    }

    public void BadFilterApplied(Destination destination, LightWeightPacket filterRule)
    {
        NewAudioManager.inst.PlayOneShot(NewAudioManager.inst.BadFilterRule);
    }

}
