using System.Collections.Generic;
using UnityEngine;

public class PacketButtonManager : MonoBehaviour
{
    public static PacketButtonManager inst;
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
    //-------------------------------------------------------------------------------------
    //public GameObject PacketButtonPrefab;
    public Transform PacketButtonParent;
    public List<PacketButtonClickHandler> packetButtons = new List<PacketButtonClickHandler>();
    
    [ContextMenu("LinkButtonList")]//For editor setup of buttons
    public void LinkButtonList()
    {
        packetButtons.Clear();
        foreach(PacketButtonClickHandler pbch in PacketButtonParent.GetComponentsInChildren<PacketButtonClickHandler>()) {
            packetButtons.Add(pbch);
        }
        Debug.Log("Number of buttons: " + packetButtons.Count);
    }

    public void ResetPacketButtons()
    {
        foreach(PacketButtonClickHandler pbch in packetButtons) {
            pbch.SetGreenHighlightColor();//togreen
            pbch.transform.parent.gameObject.SetActive(false);
        }
    }

    //gameplay packetButtons.count <= PacketQueue limit
    public void SetupPacketButtonsForInspection(Destination destination)
    {
        int index = 0;
        ResetPacketButtons(); // make all taiser button panels invisible
        destination.PacketQueue.Reverse();
        foreach (LightWeightPacket lwp in destination.PacketQueue) {
            //Debug.Log("Button for: " + lwp.size + ", " + lwp.color + ", " + lwp.shape);
            packetButtons[index].packet.copy(lwp);
            //Debug.Log("Dest: " + destination.gameName + ", packetDest: " + lwp.destination.gameName + ", isMal: " + lwp.isMalicious );
            packetButtons[index].SetHighlightColor();
            packetButtons[index].transform.parent.gameObject.SetActive(true); //make this button panel visible

            index += 1;
            if(index >= packetButtons.Count) // only have this many buttons available!
                break;
        }
        destination.PacketQueue.Clear(); // Once you click on a button, you lose all packets
    }

    //To Do
    public void ResetHighlightColor()//should be called if game state is packet inspection and new mal rule arrives with destination
    {
        foreach(PacketButtonClickHandler pbch in packetButtons) {
            if(pbch.isActiveAndEnabled)
                pbch.SetHighlightColor();
        }
    }


}
