using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Difficulty
{
    Novice = 0,
    Intermediate,
    Advanced
}

public class NewGameManager : MonoBehaviour
{

    public static NewGameManager inst;
    private void Awake()
    {
        inst = this;
        // GatherSourcesDestinations();
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void Initialize() {
        Countdown();
    }
    public Difficulty difficulty;
    public enum GameState
    {
        Countdown,
        InWave,
        FlushingSourcesToEndWave,
        WaveEnd,
        PacketExamining,
        BeingAdvised,
        Menu,
        None
    }

    public Panel CountdownPanel;
    public Panel PacketExaminerPanel;
    public Panel WatchingPanel;
    // public Panel WaveStartPanel;
    public Panel WaveEndPanel;
    public Panel MenuPanel;
    public GameState _state;
    public GameState State
    {
        get { return _state; }
        set
        {
            _state = value;

            CountdownPanel.isVisible = (_state == GameState.Countdown);
            // WaveEndPanel.isVisible = (_state == GameState.WaveEnd);

            // PacketExaminerPanel.isVisible = (_state == GameState.PacketExamining);
            // // CountdownPanel.isVisible = (_state == GameState.Start);
            // WatchingPanel.isVisible = (_state == GameState.InWave || _state == GameState.FlushingSourcesToEndWave);
            // //add menu panel
            // MenuPanel.isVisible = (_state == GameState.Menu);

            // if(_state != GameState.PacketExamining)
            //     UnExamineAllDestinations();

        }
    }


    int timerSecs = 5;
    public Text CountdownLabel;
    public void Countdown() {
        State = GameState.Countdown;

        CountdownLabel.text = timerSecs.ToString("0");
        InvokeRepeating("CountdownLabeller", 0.1f, 1f);
    }

    void CountdownLabeller()
    {
        //Debug.Log("Calling invoke repeating: timesecs: " + timerSecs);
        if(timerSecs <= 0) {
            CancelInvoke("CountdownLabeller");
            State = GameState.InWave;
            // StartWaveAtSources();
            // StartWaveAtDestinations();
            timerSecs = 5;
        } else {
            // NewAudioMgr.inst.PlayOneShot(NewAudioMgr.inst.Countdown);
            timerSecs -= 1;
            CountdownLabel.text = timerSecs.ToString("0");
        }
        
    }


}