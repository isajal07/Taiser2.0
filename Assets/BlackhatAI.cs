using System.Collections.Generic;
using UnityEngine;

public class BlackhatAI : MonoBehaviour
{
    public static BlackhatAI inst;
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

    public LightWeightPacket CreateRandomRuleForDestination(Destination destination)
    {
        LightWeightPacket lwp = new LightWeightPacket();
        lwp.shape = (PacketShape) NewGameManager.inst.TRandom.Next(0, NewGameManager.inst.PacketShapes.Count);
        lwp.color = (PacketColor) NewGameManager.inst.TRandom.Next(0, NewGameManager.inst.PacketColors.Count);
        lwp.size =  (PacketSize) NewGameManager.inst.TRandom.Next(0, NewGameManager.inst.PacketSizes.Count);
        lwp.destination = null;
        return lwp;
    }

    public LightWeightPacket CreateNonMaliciousPacketRuleForDestination(Destination destination) //SetCurrentPacketRule()
    {
        LightWeightPacket lwp = CreateRandomRuleForDestination(destination);
        while(lwp.isEqual(destination.MaliciousRule)) lwp = CreateRandomRuleForDestination(destination); //any packet except the malicious packet
        return lwp;
    }



    public LightWeightPacket CreateMaliciousPacketRuleForDestination(Destination destination = null)
    {
        LightWeightPacket lwp = CreateRandomRuleForDestination(destination);
        InstrumentManager.inst.AddRecord(TaiserEventTypes.SetNewMaliciousRule.ToString(), destination.inGameName); // For each destination
        // EffectsMgr.inst.NewRule(destination, lwp);
        //NewAudioMgr.inst.source.PlayOneShot(NewAudioMgr.inst.MaliciousRuleChanged);
        return lwp;
    }

}

