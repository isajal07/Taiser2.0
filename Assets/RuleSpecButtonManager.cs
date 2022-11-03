using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleSpecButtonManager : MonoBehaviour
{
    public static RuleSpecButtonManager inst;
    public void Awake()
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

    [System.Serializable]
    public class ButtonList
    {
        public List<Button> buttons = new List<Button>();
    }

    public GameObject ButtonsRoot;
    public List<ButtonList> RuleButtons2d = new List<ButtonList>();
    public int nrows = 3;
    public int ncols = 3;

    public Button AcceptAdviceButton;

    [ContextMenu("SetupButtons")]
    public void SetupButtons()
    {
        RuleButtons2d.Clear();
        for(int i = 0; i < nrows; i++) {
            RuleButtons2d.Add(new ButtonList());
        }
        int row = 0;
        int index = 0;
        foreach(Button b in ButtonsRoot.GetComponentsInChildren<Button>()) {
            string bt = b.GetComponentInChildren<Text>().text.ToLower();
            if(bt != "Size".ToLower() && bt != "Color".ToLower() && bt != "Shape".ToLower()) {
                row = index / nrows;
                RuleButtons2d[row].buttons.Add(b);
                index++;
            }
        }
    }

    public Destination CurrentDestination;//Set by NewGameMgr OnAttackableDestinationClicked
    public LightWeightPacket PlayerRuleSpec = new LightWeightPacket();
    public LightWeightPacket AdvisorRuleSpec = new LightWeightPacket();
    public void OnSizeClick(int size)
    {
        PlayerRuleSpec.size = (PacketSize) size;
        InstrumentManager.inst.AddRecord(TaiserEventTypes.RuleSpec.ToString(), PlayerRuleSpec.size.ToString());
    }
    public void OnColorClick(int color)
    {
        PlayerRuleSpec.color = (PacketColor) color;
        InstrumentManager.inst.AddRecord(TaiserEventTypes.RuleSpec.ToString(), PlayerRuleSpec.color.ToString());
    }
    public void OnShapeClick(int shape)
    {
        PlayerRuleSpec.shape = (PacketShape) shape;
        InstrumentManager.inst.AddRecord(TaiserEventTypes.RuleSpec.ToString(), PlayerRuleSpec.shape.ToString());
    }


    public void SetDestAndAdvisorRule(Destination destination, bool isCorrect)
    {
        CurrentDestination = destination;
        if(isCorrect) {
            AdvisorRuleSpec = destination.MaliciousRule;
        } else {
            AdvisorRuleSpec = BlackhatAI.inst.CreateNonMaliciousPacketRuleForDestination(destination);
        }

    }

    /// <summary>
    /// Called from TaiserFilterRuleSpecPanel from SetFirewall button
    /// </summary>
    public void ApplyCurrentUserRule()
    {
        NewGameManager.inst.ApplyFirewallRule(CurrentDestination, PlayerRuleSpec, false);

    }


    /// <summary>
    /// Called from TaiserFilterRuleSpecPanel from Accept Advice button
    /// </summary>
    public void ApplyAdvice()
    {
        NewGameManager.inst.ApplyFirewallRule(CurrentDestination, AdvisorRuleSpec, true);
    }
}
