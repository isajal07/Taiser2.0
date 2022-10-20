using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewLobbyManager : MonoBehaviour
{
    public static NewLobbyManager inst;
    // Start is called before the first frame update
    public void Awake()
    {
        inst = this;
    }

    void Start()
    {
        State = LobbyState.StartOrQuit;
        SetPriorStateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        public enum LobbyState
    {
        StartOrQuit = 0,
        EnterAlias,
        CreateOrJoin,
        MissionObjective,
        ChooseTeammateSpecies,
        WaitingForPlayers,
        Play,
        None
    }

    public Panel StartPanel;
    public Panel EnterAliasPanel;
    // public Panel CreateOrJoinGamePanel;
    public Panel MissionObjectivePanel;
    public Panel ChooseTeammateSpeciesPanel;
    public Panel WaitingForPlayersPanel;
    public LobbyState _state;
    public LobbyState State
    {
        get { return _state; }
        set
        {
            _state = value;
            StartPanel.isVisible = (_state == LobbyState.StartOrQuit);
            EnterAliasPanel.isVisible = (_state == LobbyState.EnterAlias);
            MissionObjectivePanel.isVisible = (_state == LobbyState.MissionObjective);
            ChooseTeammateSpeciesPanel.isVisible = (_state == LobbyState.ChooseTeammateSpecies);
            WaitingForPlayersPanel.isVisible = (_state == LobbyState.WaitingForPlayers);
            // CreateOrJoinGamePanel.isVisible = (_state == LobbyState.CreateOrJoin);
        }
    }

    public Dictionary<LobbyState, LobbyState> PriorStateMap = new Dictionary<LobbyState, LobbyState>();
    public void SetPriorStateMap()
    {
        PriorStateMap.Add(LobbyState.StartOrQuit, LobbyState.StartOrQuit);
        PriorStateMap.Add(LobbyState.EnterAlias, LobbyState.StartOrQuit);
        PriorStateMap.Add(LobbyState.CreateOrJoin, LobbyState.EnterAlias);
        PriorStateMap.Add(LobbyState.MissionObjective, LobbyState.CreateOrJoin);
        // PriorStateMap.Add(LobbyState.ChooseTeammateSpecies, LobbyState.MissionObjective);
        // PriorStateMap.Add(LobbyState.WaitingForPlayers, LobbyState.ChooseTeammateSpecies);
        // PriorStateMap.Add(LobbyState.Play, LobbyState.WaitingForPlayers);
        //PriorStateMap.Add(LobbyState.Play, LobbyState.Play);
    }

    //-------------------StartPanel------------------------------------
    public void OnStartButton()
    {
        State = LobbyState.EnterAlias;
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
    
    //---------------------------------------------------------------------

    //-------------------EnterAliasPanel------------------------------------
    public InputField AliasInputFieldText;
    public Text PlayerNameText;
    public Text GameNamePlaceholderText;
    public GameObject JoinButton;
    public static string PlayerName = "sjl";
    public void OnJoinButton()
    {
    //   if (AliasInputFieldText.isFocused)
    //     {
    //         Debug.Log("clickeedd");
    //         AliasInputFieldText.placeholder.GetComponent<Text>().text = "";
    //         AliasInputFieldText.caretWidth = 2;
    //     }
    //     else if (AliasInputFieldText.text == "" && !AliasInputFieldText.isFocused) {
    //         AliasInputFieldText.placeholder.GetComponent<Text>().text = "Enter your alias...";
    //     }

        // PlayerName = AliasInputFieldText.text.Trim();
        // PlayerNameText.text = PlayerName;
        // PlayerRole = PlayerRoles.Whitehat;
        // PlrSpecies = PlayerSpecies.Human;

        // thisPlayer = new TaiserPlayer(PlayerName, PlayerRole, PlrSpecies);
        // gameDifficulty = Difficulty.Novice;
        // teammateSpecies = PlayerSpecies.Human;

        // GameName = PlayerName + "_Taiser";
        // GameNamePlaceholderText.text = GameName;
        //NetworkingManager.instance.JoinLobby();

        //if(!NetworkingManager.gameOpened)
        //    NetworkingManager.gameOpened = true;

        
        State = LobbyState.MissionObjective;
        AliasInputFieldText.caretBlinkRate = 10;
    }

    // public void showJoinButton(bool isActive) {
    //     JoinButton.SetActive(isActive);
    // }
    
    //------------------------------------------------------------------
    
    //-------------------MissionObjectivePanel-------------------------------------
    public void onUnderstoodButton()
    {
        State = LobbyState.ChooseTeammateSpecies;
    }
    
    //---------------------------------------------------------------------

    //-------------------MissionObjectivePanel-------------------------------------
    public void onChooseTeammateButton()
    {
        State = LobbyState.WaitingForPlayers;
    }
    
    //---------------------------------------------------------------------

}
