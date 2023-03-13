using System.Collections.Generic;
using UnityEngine;

public enum DestinationStates
{
    Idle = 0,
    Up,
    Paused,
    Down,
    isBeingExamined,
    isFilteringAndBeingExamined
}

public class Destination : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResetCounts();
        originalCubeScale = maliciousCube.transform.localScale;
    }

    public void Reset()
    {
        ResetCounts();
        maliciousCube.transform.localScale = originalCubeScale;
        isBeingExamined = false;
        isFilterValid = false;
        dt = minTimeInterval;
        destinationState = DestinationStates.Idle;
    }

    public void StartWave()
    {
        destinationState = DestinationStates.Up;
        dt = minTimeInterval;
        MaliciousRule = BlackhatAI.inst.CreateMaliciousPacketRuleForDestination(this);
        isBeingExamined = false;
        isFilterValid = false;// ?
    }

    public void EndWave()
    {
        destinationState = DestinationStates.Idle;
    }

    public void PauseMaliciousClock()
    {
        destinationState = DestinationStates.Paused;
    }
    public void UnPauseMaliciousClock()
    {
        destinationState = DestinationStates.Up;
    }

    //----------------------------------------------------------------------------
    public LightWeightPacket MaliciousRule;
    public float dt = 0;
    public int initTime = 20;
    public int minTimeInterval = 12; //For mal rule change, set this in editor to tune game, every 25 seconds change rule
    public int maxTimeInterval = 22;
    public bool isBeingExamined = false;

    public DestinationStates destinationState = DestinationStates.Idle;

    // Update is called once per frame
    void Update()
    {
        if(destinationState == DestinationStates.Up) {
            if(dt <= 0) {
                dt = GenerateTimeInterval();
                MaliciousRule = BlackhatAI.inst.CreateMaliciousPacketRuleForDestination(this);
                if(NewGameManager.inst.State == NewGameManager.GameState.PacketExamining && isBeingExamined) {// This needs to be tested (5/25/2022)
                    PacketButtonManager.inst.ResetHighlightColor();
                    //Debug.Log(inGameName + ": Resetting highlight colors");
                }
                //Debug.Log(gameName + " dest, created new Mal rule: " + MaliciousRule.ToString());
            } else {
                dt -= Time.deltaTime;
            }
        }
    }

    float GenerateTimeInterval()
    {
        return (float) NewGameManager.inst.TRandom.Next(minTimeInterval, maxTimeInterval);
    }

    public int myId;
    public string inGameName;

    public void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("TDestination Collided with " + collision.gameObject.name);
        Packet tPack = collision.transform.parent.gameObject.GetComponent<Packet>();
        if(null != tPack) {
            packetCount += 1;
            //if packet is not malicious
            //Debug.Log("Collided packet: " + tPack.packet.ToString() + ", isMalicious: " + tPack.packet.isMalicious);
            if(!tPack.packet.isMalicious) {
                TLogPacket(tPack);
            } else if (tPack.packet.isMalicious && !isPacketFiltered(tPack)) {
                maliciousCount += 1;
                maliciousUnfilteredCount += 1;
                GrowCube();
                TLogPacket(tPack);
                EffectsManager.inst.MaliciousUnfilteredPacket(this, tPack.packet);
                // InstrumentManager.inst.AddRecord(TaiserEventTypes.MaliciousPacketUnfiltered_BadForUs.ToString(), inGameName);
                // NewAudioManager.inst.source.PlayOneShot(NewAudioManager.inst.maliciousUnfiltered);

            } else if (tPack.packet.isMalicious && isPacketFiltered(tPack)) {
                maliciousCount += 1;
                maliciousFilteredCount += 1;
                ShrinkCube();
                EffectsManager.inst.MaliciousFilteredPacket(this, tPack.packet);
                // InstrumentManager.inst.AddRecord(TaiserEventTypes.MaliciousPacketFiltered_GoodForUs.ToString(), inGameName);
                // NewAudioManager.inst.source.PlayOneShot(NewAudioManager.inst.maliciousFiltered);

            } // ! malicious but filtered ==> oopsie penalty in score
            NewEntityManager.inst.ReturnPoolPacket(tPack); // return to pool: reparent, set velocity to zero
        }
    }

    public GameObject maliciousCube;
    public Vector3 scaleCubeDelta;
    public Vector3 maxCubeScale;
    public Vector3 originalCubeScale;
    public void GrowCube()
    {
        if(maliciousCube?.transform.localScale.y < maxCubeScale.y)
            maliciousCube.transform.localScale += scaleCubeDelta;
    }
    public void ShrinkCube()
    {
        Vector3 newScale = maliciousCube.transform.localScale - scaleCubeDelta;
        if(maliciousCube?.transform.localScale.y > originalCubeScale.y && newScale.y > 0.1)
            maliciousCube.transform.localScale -= scaleCubeDelta;
    }
    public void ResetMaliciousCube(LightWeightPacket lwp)
    {
        maliciousCube.transform.localScale = originalCubeScale;
    }

    public int QueueSizeLimit = 21;
    public List<LightWeightPacket> PacketQueue = new List<LightWeightPacket>();

    public void TLogPacket(Packet taiserPacket)
    {
        LightWeightPacket packet = new LightWeightPacket(taiserPacket.packet);
        AddFIFOSizeLimitedQueue(PacketQueue, packet, QueueSizeLimit);
        // limit is what can be displayed in the button list 
    }
    void AddFIFOSizeLimitedQueue(List<LightWeightPacket> packetList, LightWeightPacket packet, int limit)
    {
        if(packetList.Count >= limit)
            packetList.RemoveAt(0);
        packetList.Add(packet);
    }

    //public void OnAttackableDestinationClicked()
    //{
    //    if(PacketQueue.Count > 0) {
    //        Debug.Log("Destination: " + myId + " clicked, Packet Queue[0]: " + PacketQueue[0].ToString());
    //        NewGameManager.inst.OnAttackableDestinationClicked(this);
    //    }
    //}

    public LightWeightPacket CurrentFilter;
    public bool isFilterValid = false; //set to false at the beginning of every wave

    public void FilterOnRule(LightWeightPacket lwp)
    {
        isFilterValid = true;
        CurrentFilter.copy(lwp);
    }
    public int maliciousFilteredCount = 0;
    public int maliciousCount = 0;
    public int maliciousUnfilteredCount = 0;
    public int packetCount = 0;

    public bool isPacketFiltered(Packet tPack)
    {
        return isFilterValid && tPack.packet.isEqual(CurrentFilter);
    }

    public void ResetCounts()
    {
        maliciousCount = 0;
        maliciousFilteredCount = 0;
        maliciousUnfilteredCount = 0;
    }


}
