using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceSink : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 Offset;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(transform.parent.gameObject.name + " end: " +  name + ": TSourceSink, collided with " + collision.gameObject.name);
        Packet packet = collision.transform.parent.gameObject.GetComponent<Packet>();
        if(null != packet) {
            Debug.Log("Packet: " + packet.ToString());
            NewEntityManager.inst.ReturnPoolPacket(packet);// reparent, set normalizedHeading/vel direction vector to zero (no movement)
        }
    }
}
