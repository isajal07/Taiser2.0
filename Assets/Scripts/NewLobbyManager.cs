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
        ChooseGameMode,
        WaitingForPlayers,
        Play,
        None
    }

    public Panel StartPanel;
    public Panel EnterAliasPanel;
    // public Panel CreateOrJoinGamePanel;
    public Panel AdminPanel;
    public Panel MissionObjectivePanel;
    public Panel ChooseGameModePanel;    
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
            ChooseGameModePanel.isVisible = (_state == LobbyState.ChooseGameMode);
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
        PlayerRole = PlayerRoles.Whitehat;
        PlrSpecies = PlayerSpecies.Human;

        thisPlayer = new TaiserPlayer(PlayerName, PlayerRole, PlrSpecies);
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
        State = LobbyState.ChooseGameMode;
    }

    public void onBackToEnterAliasPanel(){
        State = LobbyState.EnterAlias;
    }
    //---------------------------------------------------------------------
    //-------------------GameModePanel-------------------------------------
    public static GameMode gameMode = GameMode.Session;
    public GameMode publicGameMode;
    public void OnChoseGameMode(bool isPractice)
    {
        if(isPractice)
            gameMode = GameMode.Practice;
        else
            gameMode = GameMode.Session;
        publicGameMode = gameMode;
        State = LobbyState.WaitingForPlayers;
        StartCoroutine("CreateGameAndWaitForPlayers");
    }

    public void onBackToMissionObjectivePanel(){
        State = LobbyState.MissionObjective;
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
    


    IEnumerator CreateGameAndWaitForPlayers()
    {

        bool RandBool = utils.generateBoolean();

        PlayerNameText.text = PlayerName;
        HumanTeammateNameText.text = utils.generatePlayerName();
        AITeammateNameText.text = utils.generatePlayerName();
        Opponent1NameText.text = utils.generatePlayerName();
        Opponent2NameText.text = utils.generatePlayerName();
        
        float opponent1WaitingTime = utils.generateWaitingTime();
        float teamateWaitingTime = utils.generateWaitingTime();

        yield return new WaitForSeconds(teamateWaitingTime);
        HumanTeammate.SetActive(true);
        
        yield return new WaitForSeconds(opponent1WaitingTime);
        Opponent1.SetActive(true);
        
        if (RandBool) {
            float opponent2WaitingTime = utils.generateWaitingTime();
            yield return new WaitForSeconds(opponent2WaitingTime);
            Opponent2.SetActive(RandBool);
        }

        SpinnerPanel.gameObject.SetActive(false);
        StartButton.SetActive(true);
        ReadyMessage.text = "Ready!";
    }
    public void onStartButton()
    {
        Debug.Log("Creating Game");
        State = LobbyState.Play;
        InstrumentManager.isDebug = false;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }

    public void onBackToGameModePanel(){
        State = LobbyState.ChooseGameMode;
    }

    
    //------Admin--------------------------
    public void OnAdminStoreButton()
        {
            AdminManager.inst.WriteParamsToServer();
            State = LobbyState.StartOrQuit;
        }
    
    //---------------------------------------------------------------------

}
