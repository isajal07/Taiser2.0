using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class NewLobbyManager : MonoBehaviour
{
    public static NewLobbyManager inst;
    public Utils utils;
    // Start is called before the first frame update
    public void Awake()
    {
        inst = this;
        utils = new Utils();
    }

    void Start()
    {
        State = LobbyState.StartOrQuit;
        TRandom = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        public enum LobbyState
    {
        StartOrQuit = 0,
        EnterAlias,
        Admin,
        CreateOrJoin,
        MissionObjective,
        // ChooseDifficulty,
        WaitingForPlayers,
        Play,
        None
    }

    public Panel StartPanel;
    public Panel EnterAliasPanel;
    // public Panel CreateOrJoinGamePanel;
    public Panel AdminPanel;
    public Panel MissionObjectivePanel;
    // public Panel ChooseDifficultyPanel;
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
            AdminPanel.isVisible = (_state == LobbyState.Admin);
            MissionObjectivePanel.isVisible = (_state == LobbyState.MissionObjective);
            // ChooseDifficultyPanel.isVisible = (_state == LobbyState.ChooseDifficulty);
            WaitingForPlayersPanel.isVisible = (_state == LobbyState.WaitingForPlayers);
            // CreateOrJoinGamePanel.isVisible = (_state == LobbyState.CreateOrJoin);
        }
    }

    //-------------------StartPanel------------------------------------
    public void OnPlayButton()
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
    // public Text PlayerNameText;
    public Text GameNamePlaceholderText;
    public GameObject JoinButton;
    public static string PlayerName = "sjl";
    
    public PlayerRoles PlayerRole;
    public PlayerSpecies PlrSpecies;
    public static TaiserPlayer thisPlayer;
    public static PlayerSpecies teammateSpecies;

    public void OnJoinButton()
    {
        PlayerName = AliasInputFieldText.text.Trim();
        // Debug.Log(PlayerName);
        // PlayerNameText.text = PlayerName;
        // PlayerRole = PlayerRoles.Whitehat;
        // PlrSpecies = PlayerSpecies.Human;

        // thisPlayer = new TaiserPlayer(PlayerName, PlayerRole, PlrSpecies);
        // // gameDifficulty = Difficulty.Novice;
        teammateSpecies = PlayerSpecies.Human;
        // GameName = PlayerName + "_Taiser";
        // GameNamePlaceholderText.text = GameName;        

        if(PlayerName.Trim().ToLower().Contains("admin")) {
            State = LobbyState.Admin;
        } else {
            State = LobbyState.MissionObjective;
        }
    }    
    //------------------------------------------------------------------
    
    //-------------------MissionObjectivePanel-------------------------------------
    public void onUnderstoodButton()
    {
        State = LobbyState.WaitingForPlayers;
        CreateGameAndWaitForPlayers();
    }

    public void onBackToEnterAliasPanel(){
        State = LobbyState.EnterAlias;
    }
    //---------------------------------------------------------------------
    //-------------------WaitingPlayerPanel--------------------------------

    public Text PlayerNameText;
    public Text HumanTeammateNameText;
    public Text AITeammateNameText;
    public Text Opponent1NameText;
    public Text Opponent2NameText;
    public GameObject Player;
    public GameObject HumanTeammate;
    public GameObject AITeammate;
    public GameObject Opponent1;
    public GameObject Opponent2;
    public RectTransform SpinnerPanel;
    public System.Random TRandom;
    public GameObject StartButton;
    public Text ReadyMessage;

    public async void CreateGameAndWaitForPlayers()
    {

        bool RandBool = utils.generateBoolean();

        PlayerNameText.text = PlayerName;
        HumanTeammateNameText.text = utils.generatePlayerName();
        AITeammateNameText.text = utils.generatePlayerName();
        Opponent1NameText.text = utils.generatePlayerName();
        Opponent2NameText.text = utils.generatePlayerName();
        
        int opponent1WaitingTime = utils.generateWaitingTime();
        int teamateWaitingTime = utils.generateWaitingTime();

        await Task.Delay(teamateWaitingTime);
        HumanTeammate.SetActive(true);
        
        await Task.Delay(opponent1WaitingTime);
        Opponent1.SetActive(true);
        
        if (RandBool) {
            int opponent2WaitingTime = utils.generateWaitingTime();
            await Task.Delay(opponent2WaitingTime);
            Opponent2.SetActive(RandBool);
        }

        SpinnerPanel.gameObject.SetActive(false);
        StartButton.SetActive(true);
        ReadyMessage.text = "Ready!";
    }
    public void onStartButton()
    {
        // Debug.Log("STARTED!!");
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }

    public void onBackToMissionObjectivePanel(){
        State = LobbyState.MissionObjective;
    }

    public void OnAdminStoreButton()
        {
            AdminManager.inst.WriteParamsToServer();
            State = LobbyState.StartOrQuit;
        }
    
    //---------------------------------------------------------------------

}
