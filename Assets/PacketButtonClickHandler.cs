using UnityEngine;
using UnityEngine.UI;

public class PacketButtonClickHandler : MonoBehaviour
{

    public Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonColorBlock = button.colors;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LightWeightPacket packet;
    public void OnPacketButtonClick()
    {
        RuleSpecButtonManager.inst.OnPacketClicked(packet);
    }

    public ColorBlock buttonColorBlock;
    public void SetHighlightColor()
    {
        ColorBlock bcb = button.colors;
        if(packet.isMalicious) {
            bcb.highlightedColor = Color.red;
        } else {
            bcb.highlightedColor = Color.green;
        }
        button.colors = bcb;

    }
    public void SetGreenHighlightColor()
    {
        ColorBlock bcb = button.colors;
        bcb.highlightedColor = Color.green;
        button.colors = bcb;
    }

}
