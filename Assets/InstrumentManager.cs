using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using System.Xml;
using UnityEngine.Networking;
using UnityEngine.UI;
//----------------------------------------------------
//Could use csv helper, but seems too much for our 
//simple needs and adds unneeded dependency
//https://joshclose.github.io/CsvHelper/
//----------------------------------------------------

[System.Serializable]
public class TaiserSession
{
    public string name;
    public PlayerRoles role;
    public PlayerSpecies teammateSpecies;
    public string dayAndTime;
    public Difficulty gameDifficulty;
    public float whitehatScore;
    public float blackhatScore;
    public string studyId;
    public string settingsId;
    public string gameMode;
    public List<TaiserRecord> records;
}

[System.Serializable]
public class TaiserRecord
{
    /// <summary>
    /// From start of game
    /// </summary>
    public float secondsFromStart;
    public string eventName; //Is the .ToString() of TaiserEventTypes below
    public List<string> eventModifiers; // Only one event type has event modifiers right now
}

[System.Serializable]
public class EventLatencyTracker
{
    public TaiserEventTypes startEventType;
    public List<TaiserEventTypes> endEventTypes;
    public float startEventTime;
    public float endEventTime;
} 

[System.Serializable]
public enum TaiserEventTypes
{
    RuleSpec = 0, //which button?
    Filter,       //which rule?
    MaliciousDestinationClicked,  //which building?
    Menu,
    UserBuiltFirewallCorrectAndSet,
    UserBuiltFirewallIncorrectAndSet,
    PacketInspect,      //Packet info
    StartWave,
    EndWave,
    SetNewMaliciousRule, // set by blackhat
    AdvisedFirewallCorrectAndSet,
    AdvisedFirewallIncorrectAndSet,
    MaliciousPacketFiltered_GoodForUs,
    MaliciousPacketUnfiltered_BadForUs,
    PickedAdvisorType,
    AdviceAppeared,
    BackButtonNoFirewallSet,
    PacketsHovered,

    IgnoredAdvice,
    AdvisorSelectLatency,
    SetFireWallLatency,
    IgnoreAdviceLatency,
    BackButtonLatency,
    EndOfWave
    
}

public class InstrumentManager : MonoBehaviour
{
    public static InstrumentManager inst;
    public void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateOrFindTaiserFolder();
    }

    // Update is called once per frame
    void Update()
    {
        if(UnityEngine.InputSystem.Keyboard.current.homeKey.wasReleasedThisFrame) {
            WriteSession();
        }
    }

     public TaiserEventTypes testEvent = TaiserEventTypes.MaliciousDestinationClicked;

    public List<EventLatencyTracker> eventLatencyIntervals = new List<EventLatencyTracker>();


    public string TaiserFolder;

    public void CreateOrFindTaiserFolder()
    {
        try {
            TaiserFolder = System.IO.Path.Combine(Application.persistentDataPath);
            System.IO.Directory.CreateDirectory(TaiserFolder);
        }
        catch(System.Exception e) {
            Debug.Log("Cannot create Taiser Directory: " + e.ToString());
        }

    }

    //public List<TaiserRecord> records = new List<TaiserRecord>();
    public TaiserSession session = new TaiserSession();


    public void AddRecord(string eventName, string modifier = "")
    {
        TaiserRecord record = new TaiserRecord();
        record.eventName = eventName;
        List<string> mods = new List<string>();
        mods.Add(modifier);
        record.eventModifiers = mods;
        record.secondsFromStart = Time.time; // Time.realtimeSinceStartup;
        session.records.Add(record);
    }

     [ContextMenu("TestAddRecord")]
    // public void TestAddRecord()
    // {
    //     AddRecord(testEvent);
    // }

    //   public void AddRecord(TaiserEventTypes tEventType, string modifier = "")
    // {
    //     EventLatencyTracker elt = eventLatencyIntervals.Find(x => x.startEventType == tEventType);
    //     if(elt != null) {
    //         elt.startEventTime = Time.time;
    //         AddRecord(tEventType.ToString(), modifier);//this will be added multiple times, once for each latency tracked
    //     } 

    //     EventLatencyTracker elt2 = eventLatencyIntervals.Find(x => x.endEventTypes.Contains(tEventType));
    //     if( elt2 != null) {
    //         elt2.endEventTime = Time.time;
    //         float delta = elt2.endEventTime - elt2.startEventTime;
    //         AddRecord(tEventType.ToString(), modifier + ", " + delta.ToString("0.00"));
    //     }

    //     if(elt == null && elt2 == null) {
    //         AddRecord(tEventType.ToString(), modifier);
    //     }
    // }

    // public void AddRecord2(TaiserEventTypes tEventType, string modifier = "")
    // {
    //     List<EventLatencyTracker> startTrackers = eventLatencyIntervals.FindAll(x => x.startEventType == tEventType);
    //     foreach(EventLatencyTracker starter in startTrackers) {
    //         starter.startEventTime = Time.time;
    //         AddRecord(tEventType.ToString(), modifier);
    //     }

    //     List<EventLatencyTracker> endTrackers = eventLatencyIntervals.FindAll(x => x.endEventTypes.Contains(tEventType));
    //     foreach(EventLatencyTracker ender in endTrackers) {
    //         ender.endEventTime = Time.time;
    //         float delta = ender.endEventTime - ender.startEventTime;
    //         string outputEvent = ender.startEventType.ToString() + " latency";
    //         string outputEventLatency = delta.ToString("0.00");
    //         AddRecord(outputEvent, outputEventLatency);
    //     }

    // }  
    
    //-----------------------------------------------------------------

    //public string csvString;
    IEnumerator WriteToServer()
    {
        XmlDocument map = new XmlDocument();
        map.LoadXml("<level></level>");
        byte[] levelData = Encoding.UTF8.GetBytes(MakeHeaderString() + MakeRecords());
        string fileName = new string(session.name.ToCharArray()); // Path.GetRandomFileName().Substring(0, 8);
        fileName = fileName + ".csv";
        Debug.Log("FileName: " + fileName);

        WWWForm form = new WWWForm();
        Debug.Log("Created new WWW Form");
        form.AddField("action", "level upload");
        form.AddField("file", "file");
        form.AddBinaryData("file", levelData, fileName, "text/csv");
        Debug.Log("Binary data added");
        WWW w = new WWW("https://www.cse.unr.edu/~sajal/t2/Dataload.php", form);
        yield return w;

        if(w.error != null) {
            Debug.Log("Error: " + w.error);
            Debug.Log(w.text);
        } else {
            Debug.Log("No errors");
            Debug.Log(w.text);
            if(w.uploadProgress == 1 || w.isDone) {
                yield return new WaitForSeconds(5);
                Debug.Log("Waited five seconds");
            }
        }
    }

    //-----------------------------------------------------------------
    public static bool isDebug = true;
    public void WriteSession()
    {
        session.whitehatScore = NewGameManager.inst.WhitehatScore * 100f; // BlackhatAI.inst.wscore;
        session.blackhatScore = NewGameManager.inst.BlackhatScore * 100f; // BlackhatAI.inst.bscore;
        session.dayAndTime = System.DateTime.Now.ToUniversalTime().ToString();
        //session.gameDifficulty = NewGameManager.inst.difficulty;
        string tmp = System.DateTime.Now.ToLocalTime().ToString();
        session.name = (isDebug ? "sjl" : NewLobbyManager.thisPlayer.name);
        // session.role = PlayerRoles.Whitehat;
        session.teammateSpecies = NewLobbyManager.teammateSpecies;
        session.settingsId = NewGameManager.inst.settingsId;
        session.gameMode =  NewLobbyManager.gameMode.ToString(); 
        session.studyId = NewGameManager.inst.studyId;
        //TODO: send POST request 
        StartCoroutine("PostUserGameData");

        using(StreamWriter sw = new StreamWriter(File.Open(System.IO.Path.Combine(TaiserFolder, session.name+".csv"), FileMode.Create), Encoding.UTF8)) {
            WriteHeader(sw);
            WriteRecords(sw);
        }

        // StartCoroutine("WriteToServer");
    }

     public IEnumerator PostUserGameData() {
        var jsonData = JsonUtility.ToJson(session, true);
        Debug.Log("<><><><><>" + jsonData);
        using (UnityWebRequest req = UnityWebRequest.Post(String.Format("https://taiser2.site:5001/api/createUserGameData"), jsonData))
        {
            req.SetRequestHeader("content-type", "application/json");
            req.uploadHandler.contentType = "application/json";
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            yield return req.Send();
            while(!req.isDone)
                yield return null;
            byte[] result = req.downloadHandler.data;
            string jsonRes = System.Text.Encoding.UTF8.GetString(result);
            Debug.Log(jsonRes);
        }
    }

    
    public void WriteHeader(StreamWriter sw)
    {
        sw.WriteLine(MakeHeaderString());
    }

    string eoln = "\r\n"; //CSV RFC: https://datatracker.ietf.org/doc/html/rfc4180
    public string MakeHeaderString()
    {
        string header = "";
        header += session.name + ", " + session.role + ", "  +  session.dayAndTime + eoln;
        header += "Game Mode: ," + NewLobbyManager.gameMode + eoln;
        //header += "Game Difficulty: ," + session.gameDifficulty + eoln;
        header += "Whitehat Score, " + session.whitehatScore.ToString("00.0") +
            ", Blackhat Score, " + session.blackhatScore.ToString("00.0") + eoln;
        header += "Time, Event, Modifiers" + eoln;
        return header;
    }
    
    public void WriteRecords(StreamWriter sw)
    {
        sw.WriteLine(MakeRecords());
    }

    public string MakeRecords()
    {
        string lines = "";
        foreach(TaiserRecord tr in session.records) {
            string mods = CSVString(tr.eventModifiers);
            lines += tr.secondsFromStart.ToString("0000.0") + ", " + tr.eventName + mods + eoln;
        }
        return lines;

    }

    public string CSVString(List<string> mods)
    {
        string modifiers = "";
        foreach(string mod in mods) {
            modifiers += ", " + mod;
        }
        return modifiers;
    }

}