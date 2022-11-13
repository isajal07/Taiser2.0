using UnityEngine;

public enum SourceStates
{
    Idle = 0,
    Spawning,
}

public class Source : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //dt = timeInterval;
        //sourceState = SourceStates.Idle;
        Reset();
        Debug.Log(gameName + ": " + myId + ": " + timeInterval.ToString() + ", " + transform.parent.gameObject.name);

    }

    public float malPacketProbability = 0.25f;

    float dt;
    public float timeInterval = 0.5f; // between spawns, Set this in editor to tune game
    public int maxPackets;// = 30; //Set this in editor to tune game
    public SourceStates sourceState;
    public int myId;
    public string gameName;

    public int packetCount = 0;

    // Update is called once per frame
    void Update()
    {
        if(sourceState == SourceStates.Spawning) {
            dt -= Time.deltaTime;
            if(dt <= 0) {
                dt = timeInterval;
                SetupAndSpawnPacket();
                packetCount++;
            }
        }
        if(packetCount >= maxPackets) {
            NewGameManager.inst.EndSpawningAtSources();
        }
    }

    public void StartWave()
    {
        sourceState = SourceStates.Spawning;
        dt = timeInterval;
        //packetCount = 0;
    }

    public void EndWave()
    {
        Debug.Log("Ending wave at source: " + gameObject.name);
        Reset();
    }

    public void Reset()
    {
        dt = timeInterval;
        sourceState = SourceStates.Idle;
        packetCount = 0;
    }

    public void SetupAndSpawnPacket()
    {
        Path path = NewGameManager.inst.FindRandomPath(this);
        Destination destination = path.destination;

        if(NewGameManager.inst.Flip(malPacketProbability)) {
            SpawnPacket(destination.MaliciousRule, path);
        } else {
            SpawnPacket(BlackhatAI.inst.CreateNonMaliciousPacketRuleForDestination(destination), path);
        }
    }



    public void SpawnPacket(LightWeightPacket lwp, Path path)// = null)
    {

        Packet tp = NewEntityManager.inst.CreatePacket(lwp.shape, lwp.color, lwp.size);//from pool
        tp.packet.destination = path.destination;

        tp.transform.parent = this.transform;
        tp.InitPath(path); // set heading changes at waypoints
        if(tp.NextHeadings[0] == Vector3.zero)
            Debug.LogError("No path for this packet: " + tp.Pid);
        tp.SetNextVelocityOnPath(); //start moving

    }

    public bool Equals(Source tsA, Source tsB)
    {
        return tsA.myId == tsB.myId;
    }
    public int GetHashCode(Source x)
    {
        return x.myId.GetHashCode();
    }
}
