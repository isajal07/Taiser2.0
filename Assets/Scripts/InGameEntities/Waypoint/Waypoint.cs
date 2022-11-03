using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name + " Collided with waypoint: " + transform.gameObject.name
        //+" with parent: " + collision.transform.parent.name);
        Packet tp = collision.transform.parent.GetComponent<Packet>();
        if(null != tp) {
            //Debug.Log("Before Setting " + tp.Pid + "'s velocity " + tp.normalizedHeading);
            if(tp.CurrentPath.waypoints.Contains(this))
                tp.SetNextVelocityOnPath();
            //Debug.Log("After  Setting " + tp.Pid + "'s velocity " + tp.normalizedHeading);
        }
    }

}
