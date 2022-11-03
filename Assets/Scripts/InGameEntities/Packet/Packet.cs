using System.Collections.Generic;
using UnityEngine;

//packet prefab
public enum PacketColor
{
    Green = 0,
    Blue,
    Pink,
}

public enum PacketSize
{
    Small = 0,
    Medium,
    Large,
}

public enum PacketShape
{
    Cube = 0,
    Sphere,
    Capsule,
}


public class Packet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveTo();
    }


    public uint Pid;
    //    public PacketColor TColor;
    //    public PacketSize Size;
    //    public PacketShape Shape;
    public LightWeightPacket packet = new LightWeightPacket();


    public GameObject ShapeRef; //for scaling size

    public Path CurrentPath;

    public Vector3 normalizedHeading = Vector3.zero;
    public float speed;

    public void Init(uint pid, PacketColor color, PacketSize size)
    {
        Pid = pid;

        ReInit(color, size);
    }

    public void ReInit(PacketColor color, PacketSize size)
    {
        packet.color = color;
        packet.size = size;
        transform.localPosition = Vector3.zero;
        SetColor(color);
        SetSize(size);
}
    
    public void SetSize(PacketSize size)
    {
        ShapeRef.transform.localScale = NewGameManager.inst.SizesVector[size];//!This depends on enum type mapping to int
    }

    public void SetColor(PacketColor col)
    {
        transform.GetComponentInChildren<Renderer>().material.color = NewGameManager.inst.ColorVector[col];
        transform.GetComponentInChildren<Renderer>().material.SetColor("_EmissiveColor", NewGameManager.inst.ColorVector[col] * 1.5f);

    }

    /// <summary>
    /// Used by (BlackhatAIs, ...) to generate a set of headings for packet as it traverses path. Each waypoint in the path 
    /// changes its heading. Assumes NS and WE grid of network lines. This should work in general but assumes no obstacles between 
    /// between any two successive locations to go to.
    /// </summary>
    /// <param name="path"></param>
    public void InitPath(Path path)
    {
        CurrentPath = path;
        transform.localPosition = Vector3.zero;

        float y = 0; //do not change your height
        NextHeadings.Clear();
        NextHeadings.Add(GetNextRoundedDiff(path.source.transform.position, path.waypoints[0].transform.position, y));
        for(int i = 0; i < path.waypoints.Count - 1; i++) {
            NextHeadings.Add(GetNextRoundedDiff(path.waypoints[i].transform.position, 
                path.waypoints[i + 1].transform.position, y));
        }
        NextHeadings.Add(GetNextRoundedDiff(path.waypoints[path.waypoints.Count - 1].transform.position, 
            path.destination.transform.position, y));
        NextIndex = 0;
    }

    public Vector3 GetNextRoundedDiff(Vector3 start, Vector3 dest, float y)
    {
        Vector3 diff = dest - start;
        diff.Normalize();
        diff.x = Mathf.Round(diff.x);
        diff.z = Mathf.Round(diff.z);
        diff.y = y;
        return diff;
    }

    /// <summary>
    /// Straight line motion
    /// </summary>
    public void MoveTo()
    {
        transform.position += speed * normalizedHeading * Time.deltaTime;
    }

    public List<Vector3> NextHeadings = new List<Vector3>();
    public int NextIndex = 0;
    public void SetNextVelocityOnPath()
    {
        if(NextIndex < NextHeadings.Count) {
            // NextHeadings.ForEach(a=> Debug.Log("dudududud: " + a));
            normalizedHeading = NextHeadings[NextIndex++];
            // set rotations only when vel changes
            if(normalizedHeading.x != 0)//orients correctly by being multiplied by normalizedHeading which is +1 or -1
                transform.localEulerAngles = NewGameManager.inst.XOrientation;
            if(normalizedHeading.z != 0)
                transform.localEulerAngles = NewGameManager.inst.ZOrientation;
        }
    }


}
