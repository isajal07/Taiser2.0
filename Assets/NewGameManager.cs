using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//-------------------------------------------------------------------------------------------
[System.Serializable]
public class LightWeightPacket
{
    public PacketSize size;
    public PacketColor color;
    public PacketShape shape;
    public Destination destination = null;

    public LightWeightPacket(LightWeightPacket lwp = null)
    {
        if(lwp != null)
            copy(lwp);
    }

    public bool isMalicious
    {
        get {
            return isEqual(destination.MaliciousRule);
        }
    }

    public bool isEqual(LightWeightPacket other)
    {
        return (color == other.color && shape == other.shape && size == other.size);
    }
    public override string ToString()
    {
        return "" + size.ToString() + ", " + color.ToString() + ", " + shape.ToString() + 
            (destination == null ? "." :  ", " + destination.inGameName);
    }

    public void copy(LightWeightPacket other)
    {
        color = other.color;
        shape = other.shape;
        size = other.size;
        destination = other.destination;
    }
}
//-------------------------------------------------------------------------------------------
[System.Serializable]
public class Path
{
    public Source source;
    public Destination destination;
    public List<Waypoint> waypoints;
}
//-------------------------------------------------------------------------------------------
[System.Serializable]
public class SourcePathDebugMap
{
    public Source source;
    public List<Path> paths;
}
//-------------------------------------------------------------------------------------------

[System.Serializable]
public enum Difficulty
{
    Novice = 0,
    Intermediate,
    Advanced
}

[System.Serializable]
public class DifficultyParameters
{
    public Difficulty levelName;
    public int initTime;
    public int meanTimeInterval;
    public int timeSpread;
}


//-------------------------------------------------------------------------------------------
public class NewGameManager : MonoBehaviour
{

    public static NewGameManager inst;
    private void Awake()
    {
        inst = this;
        GatherSourcesDestinations();
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        State = GameState.WaveStart;

    }
    public void Initialize()
    {
        InitSizeColorDictionary();
        TRandom = new System.Random(RandomSeed);
        if(!isRandomSeedInitialized) {
            Debug.Log("Initializing Advice Randomizers");
            isRandomSeedInitialized = true;
            AIDecisionRandomizer = new System.Random(AIRandomizerSeed);
            HumanDecisionRandomizer = new System.Random(HumanRandomizerSeed);
        }
        // HidePrototypes(); TODO: Remove this issue!

        StartWave();
    }

    [ContextMenu("TestFlip")]
    public void TestFlip()
    {
        int count = 0;
        int max = 100;
        for(int i = 0; i < max; i++) {
            if(Flip(0.8f)) count++;
        }
        Debug.Log("Flip Prob: " + count / (float) max);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        if(endedSources >= Sources.Count) { //}|| isCorrectIndex >= isCorrectList.Count) {
            EndWave();
        }
        //if(State == GameState.InWave) {
        //BlackhatAI.inst.DoWave();
        //}
        //if(State == GameState.FlushingSourcesToEndWave)
        //if(Sources[0].gameObject.GetComponentsInChildren<TPacket>().Length <= 0)
        //EndWave();

    }

    //-------------------------------------------------------------------------------------------------
    public Difficulty difficulty;
    public List<DifficultyParameters> difficultyParamaters = new List<DifficultyParameters>();
    public void SetDifficulty(Difficulty level)
    {
        Debug.Log("Setting difficulty to: " + level);
        difficulty = level;
        DifficultyParameters parms = difficultyParamaters.Find(x => x.levelName == level);
        foreach(Destination destination in Destinations) {
            destination.timeInterval = parms.meanTimeInterval;
            destination.timeSpread = parms.timeSpread;
            destination.initTime = parms.initTime;
        }

    }

    //-------------------------------------------------------------------------------------------------


    public int RandomSeed = 1234;
    public int AIRandomizerSeed = 4321;
    public int HumanRandomizerSeed = 6789;

    public float CorrectAdviceProbability = 0.8f;
    public float HumanCorrectAdviceProbability = 0.8f;
    public float AICorrectAdviceProbability = 0.8f;
    public static bool isRandomSeedInitialized = false;


    //int maxSpawns = 40;
    public System.Random TRandom;
    public System.Random AIDecisionRandomizer;
    public System.Random HumanDecisionRandomizer;

    //----------------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns true with probability prob
    /// </summary>
    /// <param name="prob">Probability of returning true</param>
    /// <returns></returns>
    public bool Flip(float prob)
    {
        return (TRandom.NextDouble() < prob);
    }

    /// <summary>
    /// Like Flip but takes a System.Random as first param
    /// </summary>
    /// <param name="Randomizer"></param>
    /// <param name="prob"> Probability of returning true </param> 
    /// <returns></returns>
    public bool AdviceRandomizerFlip(System.Random Randomizer, float prob)
    {
        return (Randomizer.NextDouble() < prob);
    }

    //----------------------------------------------------------------------------------------------------
    public int maxWaves = 3;
    public int currentWaveNumber = 0;

    public void StartWave()
    {
        // State = GameState.WaveStart;
        // SetDifficulty(NewLobbyManager.gameDifficulty);

        Debug.Log("Startwave: " + currentWaveNumber + TaiserEventTypes.StartWave.ToString());
        InstrumentManager.inst.AddRecord(TaiserEventTypes.StartWave.ToString());

        CountdownLabel.text = timerSecs.ToString("0");
        InvokeRepeating("CountdownLabeller", 0.1f, 1f);
    }

    int timerSecs = 5;
    public Text CountdownLabel;

    void CountdownLabeller()
    {
        //Debug.Log("Calling invoke repeating: timesecs: " + timerSecs);
        if(timerSecs <= 0) {
            CancelInvoke("CountdownLabeller");
            State = GameState.InWave;
            StartWaveAtSources();
            StartWaveAtDestinations();
            timerSecs = 5;
        } else {
            // NewAudioMgr.inst.PlayOneShot(NewAudioMgr.inst.Countdown);
            timerSecs -= 1;
            CountdownLabel.text = timerSecs.ToString("0");
        }
        
    }

    public void StartWaveAtSources()
    {
        foreach(Source ts in Sources) {
            ts.StartWave();
        }
    }

    public void StartWaveAtDestinations()
    {
        foreach(Destination destination in Destinations) {
            destination.StartWave();
        }
    }

    public void EndWaveAtSources()
    {
        foreach(Source ts in Sources) {
            ts.EndWave();
        }

    }

    public void EndWaveAtDestinations()
    {
        foreach(Destination destination in Destinations) {
            destination.EndWave();
        }

    }

    void PauseDestinationsMaliciousRuleCreationClocks()
    {
        foreach(Destination destination in Destinations) {
            destination.PauseMaliciousClock();
        }
    }

    void UnPauseDestinationsMaliciousRuleCreationClocks()
    {
        foreach(Destination destination in Destinations) {
            destination.UnPauseMaliciousClock();
        }
    }
    /// <summary>
    /// Before you end a wave, make sure that each source has called EndSpawningAtSource
    /// by counting number of times this method is called by sources.
    /// If this is > number of sources, reset to 0 and actually end the wave
    /// </summary>
    int endedSources = 0;
    public void EndSpawningAtSources()
    {
        endedSources += 1;
        if(endedSources >= Sources.Count) {
            endedSources = 0; // not needed, done in EndWave->ResetVars()
            EndWave();
        }
    }

    public void ResetVars()
    {
        endedSources = 0;
        //isCorrectIndex = 0;
    }

    public Text VictoryOrDefeatText;
    public Text AnotherWaveAwaitsMessageText;
    public void EndWave()
    {
//        Debug.Log("Ending Wave: " + currentWaveNumber + ", isCorrectIndex: " + isCorrectIndex + ", endedSrcs: " + endedSources);
        Debug.Log("Ending Wave: " + currentWaveNumber + ", endedSrcs: " + endedSources);
        State = GameState.WaveEnd;
        ResetVars();
        SetWaveEndScores();
        EndWaveAtSources();
        //We call EndWaveAtDestinations in WaitToStartNextWave to give packets time to reach destinations
        PauseDestinationsMaliciousRuleCreationClocks();

        if(WhitehatScore > BlackhatScore) { 
            VictoryOrDefeatText.text = "Victory!";
            // NewAudioMgr.inst.PlayOneShot(NewAudioMgr.inst.Winning, 1.0f);
        } else {
            VictoryOrDefeatText.text = "Defeat!";
            // NewAudioMgr.inst.PlayOneShot(NewAudioMgr.inst.Losing, 5.0f);
        }
        currentWaveNumber += 1;
        if(currentWaveNumber >= maxWaves)
            AnotherWaveAwaitsMessageText.text = "Wave Goodbye! Bye Bye now...";
        else
            AnotherWaveAwaitsMessageText.text = "Get Ready for Wave " + (1+currentWaveNumber).ToString("0")
                + " of " + maxWaves.ToString("0");

        Invoke("WaitToStartNextWave", 5f);
    }

    public RectTransform blackhatBarPanel;
    public Vector3 waveEndBlackhatScoreScaler = Vector3.one;
    public RectTransform whiteHatBarPanel;
    public Vector3 waveEndWhitehatScoreScaler = Vector3.one;
    public void SetWaveEndScores()
    {
        SetScores(BlackhatScore, WhitehatScore);
        waveEndBlackhatScoreScaler.y = BlackhatScore;
        blackhatBarPanel.localScale = waveEndBlackhatScoreScaler;
        waveEndWhitehatScoreScaler.y = WhitehatScore;
        whiteHatBarPanel.localScale = waveEndWhitehatScoreScaler;
    }

    void WaitToStartNextWave()
    {
        EndWaveAtDestinations(); //Give a chance for all packets to get to destinations
        SetWaveEndScores();
        Debug.Log("Waiting to start next wave: " + currentWaveNumber);
        if(currentWaveNumber < maxWaves) {
            StartWave();//Startwave unpauses destination clocks
        } else {
            InstrumentManager.inst.WriteSession();
            State = GameState.Menu;
            ResetGame();
        }
    }

    void ResetGame()
    {
        foreach(Destination destination in Destinations) {
            destination.Reset();
        }
        foreach(Source source in Sources) {
            source.Reset();
        }
    }

    

    //----------------------------------------------------------------------------------------------------
    public List<GameObject> ToHide = new List<GameObject>();
    public void HidePrototypes()
    {
        foreach(GameObject go in ToHide) {
            foreach(MeshRenderer mr in go.GetComponentsInChildren<MeshRenderer>()) {
                // Debug.Log("MR:::::::: " + mr.gameObject.name.Contains("CubePath"));
                if(!mr.gameObject.name.Contains("CubePath"))
                    mr.enabled = false;
            }
        }
    }
    
  
    //public void PrintSourceD(Dictionary<TSource, List<TPath>> spd)
    //{
    //    foreach(TSource key in spd.Keys) {
    //        List<TPath> paths = spd[key];
    //        foreach(TPath path in paths) {
    //            Debug.Log("Path: source: " + path.source.myId + ", dest: " + path.destination.myId
    //                + ", wpCount: " + path.waypoints.Count);
    //        }
    //    }
    //}

    //-------------------------------------------------------------------------------------
    //-------Packet properties for all packets
    public List<PacketShape> PacketShapes = new List<PacketShape>();
    public List<PacketColor> PacketColors = new List<PacketColor>();
    public List<PacketSize> PacketSizes = new List<PacketSize>();

    public Color blue;
    public Color green;
    public Color pink;
    public Dictionary<PacketColor, Color> ColorVector = new Dictionary<PacketColor, Color>();

    public Vector3 smallScale = new Vector3(0.3f, 0.3f, 0.3f);
    public Vector3 mediumScale = new Vector3(0.7f, 0.7f, 0.7f);
    public Vector3 largeScale = Vector3.one;
    public Dictionary<PacketSize, Vector3> SizesVector = new Dictionary<PacketSize, Vector3>();
    public void InitSizeColorDictionary()
    {
        SizesVector.Clear();
        SizesVector.Add(PacketSize.Small, smallScale);
        SizesVector.Add(PacketSize.Medium, mediumScale);
        SizesVector.Add(PacketSize.Large, largeScale);
        ColorVector.Clear();
        ColorVector.Add(PacketColor.Blue, blue);
        ColorVector.Add(PacketColor.Green, green);
        ColorVector.Add(PacketColor.Pink, pink);
    }

    public Color testColor;
    [ContextMenu("TestSetColor")]
    public void TestSetColor()
    {
        transform.GetComponentInChildren<Renderer>().material.color = testColor;
    }

    public Vector3 XOrientation = Vector3.zero;
    public Vector3 ZOrientation = new Vector3(0, 90, 0);

    //---------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------

    //public int NumberOfPaths = 1;
    public List<Path> Paths = new List<Path>();

    public List<Source> Sources = new List<Source>();
    public List<Destination> Destinations = new List<Destination>();


    public GameObject SourcesRoot;
    public GameObject DestinationsRoot;

    [ContextMenu("GatherSourcesDestinations")]
    public void GatherSourcesDestinations()
    {
        Sources.Clear();
        int index = 0;
        foreach(Source ts in SourcesRoot.GetComponentsInChildren<Source>()) {
            Sources.Add(ts);
            ts.myId = index++;
        }
        Destinations.Clear();
        index = 0;
        foreach(Destination td in DestinationsRoot.GetComponentsInChildren<Destination>()) {
            Destinations.Add(td);
            td.myId = index++;
        }
        SetSourcePaths();//must do this every time something changes in 
        //sources or destinations so making it an automatic call
    }
    public List<SourcePathDebugMap> SourcePathDebugList = new List<SourcePathDebugMap>();
    public Dictionary<Source, List<Path>> SourcePathDictionary = new Dictionary<Source, List<Path>>();
    [ContextMenu("SetSourcePaths")]
    public void SetSourcePaths()
    {
        SourcePathDebugList.Clear();//Lists show in editor for debugging
        SourcePathDictionary.Clear();
        foreach(Source ts in Sources) {
            if(IsSourceInPaths(ts)) { 
                SourcePathDebugMap spl = new SourcePathDebugMap();
                spl.source = ts;
                spl.paths = new List<Path>();
                SourcePathDebugList.Add(spl);

                SourcePathDictionary.Add(ts, new List<Path>());
            }
        }
        foreach(Path tp in Paths) {
            SourcePathDebugList.Find(s => s.source.myId == tp.source.myId).paths.Add(tp);
            SourcePathDictionary[tp.source].Add(tp);
        }
        //
    }

    public bool IsSourceInPaths(Source ts)
    {
        foreach(Path path in Paths) {
            if(path.source.myId == ts.myId)
                return true;
        }
        return false;
    }
    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------
    public enum GameState
    {
        Start = 0,
        WaveStart,
        InWave,
        FlushingSourcesToEndWave,
        WaveEnd,
        PacketExamining,
        BeingAdvised,
        Menu,
        None
    }
    public Panel PacketExaminerPanel;
    // public Panel StartPanel;
    public Panel WatchingPanel;
    public Panel WaveStartPanel;
    public Panel WaveEndPanel;
    public Panel MenuPanel;

    public GameState _state;
    public GameState PriorState;
    public GameState State
    {
        get { return _state; }
        set
        {
            // PriorState = _state;
            _state = value;

            WaveStartPanel.isVisible = (_state == GameState.WaveStart);
            WaveEndPanel.isVisible = (_state == GameState.WaveEnd);
            
            PacketExaminerPanel.isVisible = (_state == GameState.PacketExamining);
            // StartPanel.isVisible = (_state == GameState.Start);
            WatchingPanel.isVisible = (_state == GameState.InWave || _state == GameState.FlushingSourcesToEndWave);
            //add menu panel
            MenuPanel.isVisible = (_state == GameState.Menu);

            if(_state != GameState.PacketExamining)
                UnExamineAllDestinations();

        }
    }
    //-------------------------------------------------------------------------------------

    public void UnExamineAllDestinations()
    {
        foreach(Destination destination in Destinations) {
            destination.isBeingExamined = false;
        }
    }

    //-------------------------------------------------------------------------------------
    //--- When a packet button in the examining panel is clicked

    public void OnPacketClicked(LightWeightPacket packet)
    {
        DisplayPacketInformation(packet, ClickedPacketRuleTextList); // expand on this
        InstrumentManager.inst.AddRecord(TaiserEventTypes.PacketInspect.ToString(), packet.ToString());
    }

    public void DisplayPacketInformation(LightWeightPacket packet, List<Text> RuleTextList)
    {
        RuleTextList[0].text = packet.size.ToString();
        RuleTextList[0].fontSize = FontSizes[(int) packet.size];
        RuleTextList[1].text = packet.color.ToString();
        RuleTextList[1].color = TextColors[(int) packet.color];
        RuleTextList[2].text = packet.shape.ToString();
    }

    public void ClearPacketInformation(List<Text> RuleTextList)
    {
        RuleTextList[0].text = "";
        RuleTextList[1].text = "";
        RuleTextList[2].text = "";

    }

    public List<int> FontSizes = new List<int>();
    public List<Color> TextColors = new List<Color>();

    public List<Text> ClickedPacketRuleTextList = new List<Text>();
    public List<Text> AdvisorRuleTextList = new List<Text>();


    public GameObject PacketRuleTextListRoot;
    public GameObject AdvisorRuleTextListRoot;
    [ContextMenu("SetupRuleTextLists")]
    public void SetupRuleTextLists()
    {
        ClickedPacketRuleTextList.Clear();
        foreach(Text t in PacketRuleTextListRoot.GetComponentsInChildren<Text>()) {
            ClickedPacketRuleTextList.Add(t);
        }
        AdvisorRuleTextList.Clear();
        foreach(Text t in AdvisorRuleTextListRoot.GetComponentsInChildren<Text>()) {
            AdvisorRuleTextList.Add(t);
        }
    }

    //-------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------
    // PROBLEM: Double indirection to handle design issues

    public Text FilterRuleSpecTitle;
    //public List<bool> isCorrectList = new List<bool>(); // which advice is correct, set by programmer in editor
    //public int isCorrectIndex = 0;
    public Text teammateNameText;
    public float DelayInSeconds = 3.5f;

    public void OnAttackableDestinationClicked(Destination destination)
    {
        destination.isBeingExamined = true;
        PacketButtonManager.inst.SetupPacketButtonsForInspection(destination); // Setup packet buttons on the top panel
        ClearPacketInformation(ClickedPacketRuleTextList);


        //RuleSpecButtonMgr.inst.SetDestAndAdvisorRule(destination, isCorrectList[isCorrectIndex++]);
        StartCoroutine(ProvideAdviceWithDelay(destination));
        
        FilterRuleSpecTitle.text = destination.inGameName;
        InstrumentManager.inst.AddRecord(TaiserEventTypes.MaliciousDestinationClicked.ToString(), destination.inGameName);
        State = GameState.PacketExamining;
    }

    IEnumerator ProvideAdviceWithDelay(Destination destination)
    {
        ClearPacketInformation(AdvisorRuleTextList);
        RuleSpecButtonManager.inst.AcceptAdviceButton.interactable = false;
        yield return new WaitForSeconds(DelayInSeconds);
        RuleSpecButtonManager.inst.SetDestAndAdvisorRule(destination,
            // (NewLobbyManager.teammateSpecies == PlayerSpecies.AI ?
            // AdviceRandomizerFlip(AIDecisionRandomizer, AICorrectAdviceProbability) :
            AdviceRandomizerFlip(HumanDecisionRandomizer, HumanCorrectAdviceProbability));
        SetTeammateName();
        DisplayPacketInformation(RuleSpecButtonManager.inst.AdvisorRuleSpec, AdvisorRuleTextList);
        RuleSpecButtonManager.inst.AcceptAdviceButton.interactable = true;
    }

    public void SetTeammateName()
    {
        // teammateNameText.text = NewLobbyManager.teammateName + "'s advice";
                teammateNameText.text =  "'s advice";

    }

    public void ApplyFirewallRule(Destination destination, LightWeightPacket packet, bool isAdvice)
    {
        if(packet == null) return; //------------------------------------------

        destination.FilterOnRule(packet);

        if(packet.isEqual(destination.MaliciousRule)) {
            if(isAdvice)
                InstrumentManager.inst.AddRecord(TaiserEventTypes.AdvisedFirewallCorrectAndSet.ToString());
            else
                InstrumentManager.inst.AddRecord(TaiserEventTypes.UserBuiltFirewallCorrectAndSet.ToString());
            // EffectsManager.inst.GoodFilterApplied(destination, packet);
            //NewAudioMgr.inst.PlayOneShot(NewAudioMgr.inst.GoodFilterRule);
        } else {
            if(isAdvice)
                InstrumentManager.inst.AddRecord(TaiserEventTypes.AdvisedFirewallIncorrectAndSet.ToString());
            else
                InstrumentManager.inst.AddRecord(TaiserEventTypes.UserBuiltFirewallIncorrectAndSet.ToString());
            // EffectsMgr.inst.BadFilterApplied(destination, packet);
            //NewAudioMgr.inst.source.PlayOneShot(NewAudioMgr.inst.BadFilterRule);
        }

        destination.isBeingExamined = false;
        State = GameState.InWave;
    }


    //-------------------------------------------------------------------------------------
    public RectTransform WhitehatWatchingScorePanel;
    public RectTransform BlackhatWatchingScorePanel;
    public float minScaley = 0.0f;
    public void SetScores(float blackhatScore, float whitehatScore)
    {
        SetBars(BlackhatWatchingScorePanel, blackhatScore, WhitehatWatchingScorePanel, whitehatScore);
    }
    public Vector3 inWaveWhitehatScaler = Vector3.one;
    public Vector3 inWaveBlackhatScaler = Vector3.one;
    public void SetBars(RectTransform blackhatBarPanel, float blackhatScore, 
        RectTransform whitehatBarPanel, float whitehatScore)
    {
        inWaveWhitehatScaler.x = whitehatScore + minScaley;
        whitehatBarPanel.localScale = inWaveWhitehatScaler;
        inWaveBlackhatScaler.x = blackhatScore + minScaley;
        blackhatBarPanel.localScale = inWaveBlackhatScaler;
    }

    public float WhitehatScore = 0;
    public float BlackhatScore = 0;

    public int totalMaliciousCount;
    public int totalMaliciousFilteredCount; //over all destinations
    public int totalMaliciousUnFilteredCount; //over all destinations

    public void UpdateScore()
    {
        totalMaliciousFilteredCount = 0;
        totalMaliciousCount = 0;
        totalMaliciousUnFilteredCount = 0;
        foreach(Destination destination in Destinations) {
            totalMaliciousFilteredCount += destination.maliciousFilteredCount;
            totalMaliciousUnFilteredCount += destination.maliciousUnfilteredCount; //is also malicious - filtered
            totalMaliciousCount += destination.maliciousCount;
        }

        WhitehatScore = totalMaliciousFilteredCount / (totalMaliciousCount + 0.000001f);
        BlackhatScore = totalMaliciousUnFilteredCount / (totalMaliciousCount + 0.000001f);
        SetScores(BlackhatScore, WhitehatScore);
    }



    //-------------------------------------------------------------------------------------
    public void OnMenuBackButton()
    {
        State = PriorState;
    }

    public void QuitToWindows()
    {
        Application.Quit();
    }
    public void QuitRoom()
    {
        //InstrumentMgr.inst.WriteSession();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OnMenuButtonClicked()
    {
        InstrumentManager.inst.AddRecord(TaiserEventTypes.Menu.ToString());
        State = GameState.Menu;
    }

   



    public Path FindRandomPath(Source source)
    {
        //SourcePathDebugMap map = SourcePathDebugList.Find(p => source.myId == p.source.myId);
        //Debug.Log("Paths for source: " + source.myId + ", path count: " + map.paths.Count);
        //int index = TRandom.Next(0, map.paths.Count);
        //return map.paths[index];

        int index = TRandom.Next(0, SourcePathDictionary[source].Count);//Must ensure key exists
        return (SourcePathDictionary[source])[index]; //dictionary access
    }

    //----------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------

}
