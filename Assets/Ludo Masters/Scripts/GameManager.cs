using System;
using UnityEngine;
//using ExitGames.Client.Photon.Chat;
using System.Collections.Generic;


public class GameManager
{    
    public int readyPlayersCount = 1;
    public int menuLoadCount = 0;
    public List<int> botDiceValues = new List<int>();
    public List<float> botDelays = new List<float>();
    public bool needToKillOpponentToEnterHome = false;
    public List<PlayerObject> playerObjects;
    public PlayerObject currentPlayer;
    public Sprite facebookAvatar = null;
    public MyPlayerData myPlayerData = new MyPlayerData();
    public string privateRoomID;
    public string[] scenes = new string[] { "GameScene", "CheckersScene", "TheMillScene", "SoccerScene" };
    public string[] gamesNames = new string[] { "GOMOKU", "CHECKERS", "THE MILL", "SOCCER" };
    public string GameScene = "SoccerScene";
    private static GameManager instance;
    public List<Sprite> opponentsAvatars = new List<Sprite>() { null, null, null };
    public List<string> opponentsNames = new List<string>() { null, null, null };
    public List<string> opponentsIDs = new List<string>() { null, null, null };
    public GameObject myAvatarGameObject;
    public GameObject myNameGameObject;
    public int requiredPlayers = 4;
    public int firstPlayerInGame = 0;
    public int readyPlayers = 0;
    public int currentPlayersCount = 0;
    public bool offlineMode = false;
    public AdsController adsController;
    public int myPlayerIndex = 0;
    public float playerTime = 20.0f; // player time in seconds
    public bool readyToAnimateCoins = false;
    public bool showTargetLines = false;
    public bool callPocketBlack = false;
    public bool callPocketAll = false;
    public bool LinkFbAccount = false;
    public bool inviteFriendActivated = false;
    public InitMenuScript initMenuScript;
    public string challengedFriendID;
    public GameObject tablesCanvas;
    public bool stopTimer = false;
    public bool ownSolids = false;
    public bool playersHaveTypes = false;
    public bool firstBallTouched = false;
    public bool wasFault = false;
    public bool validPot = false;
    public int validPotsCount = 0;

    
    public bool referCodeCheck = false;
    public string myphoneNum = "";
    public string faultMessage = "";
    public FacebookFriendsMenu facebookFriendsMenu;
    public GameObject matchPlayerObject;
    public GameObject backButtonMatchPlayers;
    public GameObject MatchPlayersCanvas;
    public GameObject reconnectingWindow;
    public GameControllerScript gameControllerScript;
    public FacebookManager facebookManager;
    public GameObject whiteBall;
    public bool testValue;
    public bool hasCueInHand = false;
    public GameObject FacebookLinkButton;
    public int shotPower;
    public bool ballsStriked = false;
    public List<String> ballTouchBeforeStrike = new List<String>();
    public GameObject ballHand;
    public bool iWon = false;
    public bool iLost = false;
    public bool iDraw = false;
    public bool calledPocket = false;
    public int solidPoted = 0;
    public int stripedPoted = 0;
    public bool noTypesPotedStriped = false;
    public bool noTypesPotedSolid = false;
    public GameObject usingCueText;
    public int ballTouchedBand = 0;
    public bool receivedInitPositions = false;
    public Vector3[] initPositions;
    public GameObject[] balls;
    public bool logged = false;
    public List<string> friendsIDForStatus = new List<string>();

    public string nameMy;    
    public string emailMy;
    public Sprite avatarMy;
    public string avatarMyUrl;
    public GameObject dialog;

    public string nameOpponent;
    public Sprite avatarOpponent;

    public string opponentPlayFabID;
    public int offlinePlayerTurn = 1;
    public bool offlinePlayer1OwnSolid = true;
    public string facebookIDMy;
    public bool playerDisconnected = false;

    public GameObject invitationDialog;
    /*public ChatClient chatClient;*/

    public int coinsCount;
    public bool roomOwner = false;
    public float linesLength = 5.0f;
    public int avatarMoveSpeed = 15;
    public bool opponentDisconnected = false;
    public CueController cueController;
    public GameObject friendButtonMenu;
    public GameObject smallMenu;
    public PlayFabManager playfabManager;
    public float messageTime = 0;

    public int tableNumber = 0;
    public AudioSource[] audioSources;
    public int calledPocketID = 0;
    public GameObject coinsTextMenu;
    public GameObject coinsTextShop;
    public int cueIndex = 0;
    public int cuePower = 0;
    public int cueAim = 0;
    public int cueTime = 0;
    public IAPController IAPControl;
    public GameObject cueObject;
    public List<string[]> friendsStatuses = new List<string[]>();
    public int opponentCueIndex = 0;
    public int opponentCueTime = 0;
    public ControlAvatars controlAvatars;
   // public InterstitialAdsControllerScript interstitialAds;
   // public AdMobObjectController adsScript;
    public ConnectionLostController connectionLost;
    public bool opponentActive = true;
    public IMiniGame miniGame;

    public bool myTurnDone = false;
    public string myReferCode = "";
    public string ReferCoins;

    

    public string invitationID = "";
    public MyGameMode mode;
    public MyGameType type;
    public bool isMyTurn = false;
    public bool diceShot = false;
    public string[] PlayersIDs;
    public bool gameSceneStarted = false;
    // Game settings

    // 25, 50, 100, 250, 500, 1000, 2000, 3000, 4000, 5000, 10000
    public int payoutCoins = 10000;
    public bool JoinedByID = false;

    public void resetAllData()
    {
        readyPlayersCount = 1;
        gameSceneStarted = false;
        opponentsIDs = new List<string>() { null, null, null };
        opponentsAvatars = new List<Sprite>() { null, null, null };
        opponentsNames = new List<string>() { null, null, null };
        currentPlayersCount = 0;
        myTurnDone = false;
        opponentActive = true;
        readyToAnimateCoins = false;
        opponentDisconnected = false;
        offlinePlayerTurn = 1;
        offlinePlayer1OwnSolid = true;
        offlineMode = false;
        solidPoted = 0;
        stripedPoted = 0;
        messageTime = 0.0f;
        stopTimer = false;
        ownSolids = false;
        playersHaveTypes = false;
        firstBallTouched = false;
        wasFault = false;
        validPot = false;
        validPotsCount = 0;
        faultMessage = "";
        hasCueInHand = false;
        ballsStriked = false;
        ballTouchBeforeStrike = new List<String>();
        PlayersIDs = null;



        ballTouchedBand = 0;
        receivedInitPositions = false;
    }




    private GameManager() { }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    public void resetTurnVariables()
    {
        stopTimer = false;
    }

    public void increaseReadyPlayer()
    {
        readyPlayers++;
    }

}
