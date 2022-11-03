using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewEntityManager : MonoBehaviour
{
    public static NewEntityManager inst;
    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Packets.Clear();
        InitPools();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //public TSource SourcePrefab;
    //public TDestination DestinationPrefab;

    public Packet CapsulePrefab;
    public Packet CubePrefab;
    public Packet SpherePrefab;

    public GameObject PacketPoolParent;

    public List<Packet> Packets = new List<Packet>();
    public static uint packetId = 0;
    public Packet InstantiatePacket(PacketShape shape, PacketColor color = PacketColor.Blue,
        PacketSize size = PacketSize.Large)    {

        //Debug.Log("Creating packet with default parent");
        Packet tPacket = null;
        switch(shape) {
            case PacketShape.Capsule:
                tPacket = Instantiate<Packet>(CapsulePrefab, PacketPoolParent.transform);
                break;
            case PacketShape.Cube:
                tPacket = Instantiate<Packet>(CubePrefab, PacketPoolParent.transform);
                break;
            case PacketShape.Sphere:
                tPacket = Instantiate<Packet>(SpherePrefab, PacketPoolParent.transform);
                break;
            default:
                Debug.LogWarning("Unknown packet shape: " + shape.ToString());
                tPacket = Instantiate<Packet>(CapsulePrefab, PacketPoolParent.transform);
                break;
        }
        if(null != tPacket) {
            tPacket.Init(packetId++, color, size);
            Packets.Add(tPacket);
        }
        return tPacket;
    }

    public List<Packet> CubePacketPool;
    public List<Packet> SpherePacketPool;
    public List<Packet> CapsulePacketPool;
    public Dictionary<PacketShape, List<Packet>> PacketPools = new Dictionary<PacketShape, List<Packet>>();
    public int PoolLimit = 10;

    [ContextMenu("InitPools")]
    public void InitPools()
    {
        Debug.Log("initializing pools");
        foreach(PacketShape shape in System.Enum.GetValues(typeof(PacketShape))) {
            PacketPools.Add(shape, new List<Packet>());
            FillPool(shape, PacketPools[shape], PoolLimit);
        }
        CubePacketPool = new List<Packet>(PacketPools[PacketShape.Cube]);
        SpherePacketPool = new List<Packet>(PacketPools[PacketShape.Sphere]);
        CapsulePacketPool = new List<Packet>(PacketPools[PacketShape.Capsule]);
    }

    public void FillPool(PacketShape shape, List<Packet> pool, int limit)
    {
        pool.Clear();
        for(int i = 0; i < limit; i++) {
            pool.Add(InstantiatePacket(shape));
        }
    }

    public Packet CreatePacket(PacketShape shape, PacketColor color = PacketColor.Blue,
        PacketSize size = PacketSize.Large)
    {
        Packet packet = null;

        List<Packet> packetPool = PacketPools[shape];
        if(packetPool.Count > 0) {
            packet = packetPool[0];
            packet.ReInit(color, size); //only for pool packet, InstantiatePacket calls Init for new packets
            packetPool.RemoveAt(0);
        } else {
            Debug.Log("Creating new packet for empty pool");
            packet = InstantiatePacket(shape, color, size);
            PacketPools[shape].Add(packet);
        }
        return packet;
    }
    
    public void ReturnPoolPacket(Packet packet)
    {
        PacketPools[packet.packet.shape].Add(packet);
        packet.transform.SetParent(PacketPoolParent.transform);
        packet.normalizedHeading = Vector3.zero;
        packet.transform.localPosition = Vector3.zero;

    }



}
