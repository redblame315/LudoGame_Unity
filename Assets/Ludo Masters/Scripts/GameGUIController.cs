using System;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using Facebook.Unity;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Globalization;
using SocketIO;



public class GameGUIController : MonoBehaviour
{

    public GameObject TIPButtonObject;
    public GameObject TIPObject;
    public GameObject firstPrizeObject;
    public GameObject SecondPrizeObject;
    public GameObject firstPrizeText;
    public GameObject secondPrizeText;

    public AudioSource WinSound;
    public AudioSource myTurnSource;
    public AudioSource oppoTurnSource;
    private bool AllPlayersReady = false;
    // LUDO
    public MultiDimensionalGameObject[] PlayersPawns;
    public GameObject[] PlayersDices;
    public GameObject[] HomeLockObjects;

	public GameObject MissedModalPanel;
	public Button closeBtn;
	public Text textMsg;

    [System.Serializable]
    public class MultiDimensionalGameObject
    {
        public GameObject[] objectsArray;
    }

    public GameObject ludoBoard;
    public GameObject[] diceBackgrounds;
    public MultiDimensionalGameObject[] playersPawnsColors;
    public MultiDimensionalGameObject[] playersPawnsMultiple;
    private Color colorRed = new Color(250.0f / 255.0f, 12.0f / 255, 12.0f / 255);
    private Color colorBlue = new Color(0, 86.0f / 255, 255.0f / 255);
    private Color colorYellow = new Color(255.0f / 255.0f, 163.0f / 255, 0);
    private Color colorGreen = new Color(8.0f / 255, 174.0f / 255, 30.0f / 255);


    // END LUDO

    public GameObject GameFinishWindow;
    public GameObject ScreenShotController;
    public GameObject invitiationDialog;
    public GameObject addedFriendWindow;
    public GameObject PlayerInfoWindow;
    public GameObject ChatWindow;
    public GameObject ChatButton;
    private bool SecondPlayerOnDiagonal = true;

    private List<string> PlayersIDs;
    public GameObject[] Players;
    public GameObject[] PlayersTimers;
    public GameObject[] PlayersChatBubbles;
    public GameObject[] PlayersChatBubblesText;
    public GameObject[] PlayersChatBubblesImage;
    private GameObject[] ActivePlayers;
    public GameObject[] PlayersAvatarsButton;

    private List<Sprite> avatars;
    private List<string> names;

    private List<PlayerObject> playerObjects;
    private int myIndex;
    private string myId;


    private Color[] borderColors = new Color[4] { Color.yellow, Color.green, Color.red, Color.blue };

    private int currentPlayerIndex;

    private int ActivePlayersInRoom;

    private Sprite[] emojiSprites;

    private string CurrentPlayerID;

    private List<PlayerObject> playersFinished = new List<PlayerObject>();


    private bool iFinished = false;
    private bool FinishWindowActive = false;

    private int firstPlacePrize;
    private int secondPlacePrize;

    private int requiredToStart = 0;
    private SocketIOComponent socketIO;



    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }



    int currentCoin = 0;
    // Use this for initialization
    void Start()
    {
        Debug.Log("GAME START IN GAME GUICONTROLLER 1");
        socketIO.On("LOGIC_EVENT", OnEvent);
        socketIO.On("DELETE_LEAVE_USER", OnDeletePlayerResult);

        Dictionary<string, string> dicData = new Dictionary<string, string>();

        dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
        currentCoin = GameManager.Instance.myPlayerData.GetCoins() - GameManager.Instance.myPlayerData.GetEntryCoin();
        
        dicData.Add("cur_coin", "" + currentCoin);
        JSONObject jsonData = new JSONObject(dicData);

        socketIO.Emit("MINUS_COIN", jsonData);

        playersFinished = new List<PlayerObject>();



        StaticStrings.missedChance = 0;
        requiredToStart = GameManager.Instance.requiredPlayers;

        if (GameManager.Instance.type == MyGameType.Private)
        {
            requiredToStart = 2;
        }
        
        // LUDO
        // Rotate board and set colors

        int rotation = UnityEngine.Random.Range(0, 4);
        Debug.Log("GAME START IN GAME GUICONTROLLER 2");


        Color[] colors = null;

        if (rotation == 0)
        {
            colors = new Color[] { colorYellow, colorGreen, colorRed, colorBlue };
        }
        else if (rotation == 1)
        {
            colors = new Color[] { colorBlue, colorYellow, colorGreen, colorRed };
            ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -90.0f);
        }
        else if (rotation == 2)
        {
            colors = new Color[] { colorRed, colorBlue, colorYellow, colorGreen };
            ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -180.0f);
        }
        else
        {
            colors = new Color[] { colorGreen, colorRed, colorBlue, colorYellow };
            ludoBoard.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -270.0f);
        }

        for (int i = 0; i < diceBackgrounds.Length; i++)
        {
            diceBackgrounds[i].GetComponent<Image>().color = colors[i];
        }

        for (int i = 0; i < playersPawnsColors.Length; i++)
        {
            for (int j = 0; j < playersPawnsColors[i].objectsArray.Length; j++)
            {
                playersPawnsColors[i].objectsArray[j].GetComponent<Image>().color = colors[i];
                playersPawnsMultiple[i].objectsArray[j].GetComponent<Image>().color = colors[i];
            }
        }


        // END LUDO
        Debug.Log("GAME START IN GAME GUICONTROLLER 3");



        // Update player data in
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() - GameManager.Instance.payoutCoins).ToString());
        data.Add(MyPlayerData.GamesPlayedKey, (GameManager.Instance.myPlayerData.GetPlayedGamesCount() + 1).ToString());


        //GameManager.Instance.myPlayerData.UpdateUserData(data);

        currentPlayerIndex = 0;
        emojiSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emoji;
        myId = GameManager.Instance.myPlayerData.GetPlayerId();
        playerObjects = new List<PlayerObject>();
        //avatars = GameManager.Instance.opponentsAvatars;
        //avatars.Insert(0, GameManager.Instance.avatarMy);

        avatars = new List<Sprite>();
        for (int i = 0; i < GameManager.Instance.opponentsAvatars.Count; i++)
        {
            if (GameManager.Instance.opponentsAvatars[i] != null)
            {
                Debug.Log("OPPOMEMT IS NOT NULL ");
                avatars.Add(GameManager.Instance.opponentsAvatars[i]);
            }
        }

        avatars.Insert(0, GameManager.Instance.avatarMy);



        //names = GameManager.Instance.opponentsNames;
        //names.Insert(0, GameManager.Instance.myPlayerData.GetPlayerName());

        names = new List<string>();
        for (int i = 0; i < GameManager.Instance.opponentsNames.Count; i++)
        {
            if (GameManager.Instance.opponentsNames[i] != null)
            {
                names.Add(GameManager.Instance.opponentsNames[i]);
            }
        }

        names.Insert(0, GameManager.Instance.myPlayerData.GetPlayerName());


        PlayersIDs = new List<string>();
        for (int i = 0; i < GameManager.Instance.opponentsIDs.Count; i++)
        {
            if (GameManager.Instance.opponentsIDs[i] != null)
            {
                PlayersIDs.Add(GameManager.Instance.opponentsIDs[i]);
            }
        }

        PlayersIDs.Insert(0, GameManager.Instance.myPlayerData.GetPlayerId());

        
        for (int i = 0; i < PlayersIDs.Count; i++)
        {
            Debug.Log("name : " + names[i] + " : " + PlayersIDs[i] + " : " + names.Count + " : " + PlayersIDs.Count + " : " + avatars.Count);
            playerObjects.Add(new PlayerObject(names[i], PlayersIDs[i], avatars[i]));
        }

        // Bubble sort
        for (int i = 0; i < PlayersIDs.Count; i++)
        {
            for (int j = 0; j < PlayersIDs.Count - 1; j++)
            {
                if (string.Compare(playerObjects[j].id, playerObjects[j + 1].id) == 1)
                {
                    // swap ids
                    PlayerObject temp = playerObjects[j + 1];
                    playerObjects[j + 1] = playerObjects[j];
                    playerObjects[j] = temp;
                }
            }
        }
        

        ActivePlayersInRoom = PlayersIDs.Count;

        if (PlayersIDs.Count == 2)
        {
            if (SecondPlayerOnDiagonal)
            {
                Players[1].SetActive(false);
                Players[3].SetActive(false);
                ActivePlayers = new GameObject[2];
                ActivePlayers[0] = Players[0];
                ActivePlayers[1] = Players[2];

                // LUDO
                for (int i = 0; i < PlayersPawns[1].objectsArray.Length; i++)
                {
                    PlayersPawns[1].objectsArray[i].SetActive(false);
                }


                for (int i = 0; i < PlayersPawns[3].objectsArray.Length; i++)
                {
                    PlayersPawns[3].objectsArray[i].SetActive(false);
                }

                // END LUDO
            }
            else
            {

                // LUDO
                for (int i = 0; i < PlayersPawns[21].objectsArray.Length; i++)
                {
                    PlayersPawns[2].objectsArray[i].SetActive(false);
                }

                for (int i = 0; i < PlayersPawns[3].objectsArray.Length; i++)
                {
                    PlayersPawns[3].objectsArray[i].SetActive(false);
                }

                // END LUDO
                Players[2].SetActive(false);
                Players[3].SetActive(false);
                ActivePlayers = new GameObject[2];
                ActivePlayers[0] = Players[0];
                ActivePlayers[1] = Players[1];
            }
        }
        else
        {
            ActivePlayers = Players;
        }



        int startPos = 0;
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i].id.Equals(GameManager.Instance.myPlayerData.GetPlayerId()))
            {
                startPos = i;
                break;
            }
        }
        int index = 0;
        bool addedMe = false;
        myIndex = startPos;
        GameManager.Instance.myPlayerIndex = myIndex;
        for (int i = startPos; ;)
        {
            if (i == startPos && addedMe) break;

            if (PlayersIDs.Count == 2 && SecondPlayerOnDiagonal)
            {
                if (addedMe)
                {
                    playerObjects[i].timer = PlayersTimers[2];
                    playerObjects[i].ChatBubble = PlayersChatBubbles[2];
                    playerObjects[i].ChatBubbleText = PlayersChatBubblesText[2];
                    playerObjects[i].ChatbubbleImage = PlayersChatBubblesImage[2];
                    string id = playerObjects[i].id;
                    PlayersAvatarsButton[2].GetComponent<Button>().onClick.RemoveAllListeners();
                    PlayersAvatarsButton[2].GetComponent<Button>().onClick.AddListener(() => ButtonClick(id));

                    // LUDO
                    playerObjects[i].dice = PlayersDices[2];
                    playerObjects[i].pawns = PlayersPawns[2].objectsArray;

                    for (int k = 0; k < playerObjects[i].pawns.Length; k++)
                    {
                        playerObjects[i].pawns[k].GetComponent<LudoPawnController>().setPlayerIndex(i);
                    }
                    playerObjects[i].homeLockObjects = HomeLockObjects[2];

                    // END LUDO
                }
                else
                {
                    GameManager.Instance.myPlayerIndex = i;
                    playerObjects[i].timer = PlayersTimers[index];
                    playerObjects[i].ChatBubble = PlayersChatBubbles[index];
                    playerObjects[i].ChatBubbleText = PlayersChatBubblesText[index];
                    playerObjects[i].ChatbubbleImage = PlayersChatBubblesImage[index];
                    string id = playerObjects[i].id;

                    // LUDO
                    playerObjects[i].dice = PlayersDices[index];
                    playerObjects[i].pawns = PlayersPawns[index].objectsArray;

                    for (int k = 0; k < playerObjects[i].pawns.Length; k++)
                    {
                        playerObjects[i].pawns[k].GetComponent<LudoPawnController>().setPlayerIndex(i);
                    }
                    playerObjects[i].homeLockObjects = HomeLockObjects[index];
                    // END LUDO
                }
            }
            else
            {

                playerObjects[i].timer = PlayersTimers[index];
                playerObjects[i].ChatBubble = PlayersChatBubbles[index];
                playerObjects[i].ChatBubbleText = PlayersChatBubblesText[index];
                playerObjects[i].ChatbubbleImage = PlayersChatBubblesImage[index];

                // LUDO
                playerObjects[i].dice = PlayersDices[index];
                playerObjects[i].pawns = PlayersPawns[index].objectsArray;

                for (int k = 0; k < playerObjects[i].pawns.Length; k++)
                {
                    playerObjects[i].pawns[k].GetComponent<LudoPawnController>().setPlayerIndex(i);
                }
                playerObjects[i].homeLockObjects = HomeLockObjects[index];
                // END LUDO

                string id = playerObjects[i].id;
                if (index != 0)
                {
                    PlayersAvatarsButton[index].GetComponent<Button>().onClick.RemoveAllListeners();
                    PlayersAvatarsButton[index].GetComponent<Button>().onClick.AddListener(() => ButtonClick(id));
                }

            }




            playerObjects[i].AvatarObject = ActivePlayers[index];
            ActivePlayers[index].GetComponent<PlayerAvatarController>().Name.GetComponent<Text>().text = playerObjects[i].name;
            if (playerObjects[i].avatar != null)
            {
                ActivePlayers[index].GetComponent<PlayerAvatarController>().Avatar.GetComponent<Image>().sprite = playerObjects[i].avatar;
            }

            index++;

            if (i < PlayersIDs.Count - 1)
            {
                i++;
            }
            else
            {
                i = 0;
            }

            addedMe = true;
        }
        Debug.Log("GAME START IN GAME GUICONTROLLER 4");
        currentPlayerIndex = GameManager.Instance.firstPlayerInGame;
        GameManager.Instance.currentPlayer = playerObjects[currentPlayerIndex];


        // SetTurn();

        // // if (myIndex == 0)
        // // {
        // //     SetMyTurn();
        // //     playerObjects[0].dice.GetComponent<GameDiceController>().DisableDiceShadow();
        // // }
        // // else
        // // {
        // //     SetOpponentTurn();
        // //     playerObjects[currentPlayerIndex].dice.GetComponent<GameDiceController>().DisableDiceShadow();
        // // }
        

        GameManager.Instance.playerObjects = playerObjects;

        

        firstPlacePrize = GameManager.Instance.myPlayerData.GetWinnerCoin();
        secondPlacePrize = GameManager.Instance.myPlayerData.GetSecondWinnerCoin();

        firstPrizeText.GetComponent<Text>().text = firstPlacePrize + "";
        secondPrizeText.GetComponent<Text>().text = secondPlacePrize + "";

        if (secondPlacePrize == 0)
        {
            SecondPrizeObject.SetActive(false);
            firstPrizeObject.GetComponent<RectTransform>().anchoredPosition = SecondPrizeObject.GetComponent<RectTransform>().anchoredPosition;
        }


        // LUDO

        // Enable home locks

        if (GameManager.Instance.mode == MyGameMode.Quick || GameManager.Instance.mode == MyGameMode.Master)
        {
            for (int i = 0; i < GameManager.Instance.playerObjects.Count; i++)
            {
                GameManager.Instance.playerObjects[i].homeLockObjects.SetActive(true);
            }
            GameManager.Instance.needToKillOpponentToEnterHome = true;
        }
        else
        {
            GameManager.Instance.needToKillOpponentToEnterHome = false;
        }
        //GameManager.Instance.needToKillOpponentToEnterHome = false;

        // END LUDO

//         for (int i = 0; i < playerObjects.Count; i++)
//         {
//             if (playerObjects[i].id.Contains("_BOT"))
//             {
//                 GameManager.Instance.readyPlayersCount++;
//             }
//         }

        GameManager.Instance.playerObjects = playerObjects;
        //         GameManager.Instance.myPlayerData.GetGamePlayerList()[GameManager.Instance.myPlayerData.GetGamePlayerList().Length] = GameManager.Instance.myPlayerData.GetPlayerId();
        // 
        //         // Check if all players are still in room - if not deactivate
        //         for (int i = 0; i < playerObjects.Count; i++)
        //         {
        //             bool contains = false;
        //             if (!playerObjects[i].id.Contains("_BOT"))
        //             {
        //                 for (int j = 0; j < GameManager.Instance.myPlayerData.myGamePlayerList.Length; j++)
        //                 {
        //                     if (GameManager.Instance.myPlayerData.myGamePlayerList[j].Equals(playerObjects[i].id))
        //                     {
        //                         Debug.Log("Game GUI Control : " + playerObjects[i].id);
        //                         contains = true;
        //                         break;
        //                     }
        //                 }
        // 
        //                 if (!contains)
        //                 {
        //                     GameManager.Instance.readyPlayersCount++;
        //                     Debug.Log("Ready players: " + GameManager.Instance.readyPlayersCount);
        //                     setPlayerDisconnected(i);
        //                 }
        //             }
        //         }
        Debug.Log("GAME START IN GAME GUICONTROLLER 5");
        CheckPlayersIfShouldFinishGame();

        StartCoroutine(waitForPlayersToStart());
        
    }


    private void OnDestroy()
    {
        socketIO.Off("LOGIC_EVENT", OnEvent);
        socketIO.Off("DELETE_LEAVE_USER", OnDeletePlayerResult);
    }


    private IEnumerator waitForPlayersToStart()
    {
        
        yield return new WaitForSeconds(0.1f);

        Debug.Log(GameManager.Instance.readyPlayersCount + " : " + requiredToStart);
        if (GameManager.Instance.readyPlayersCount < requiredToStart)
        {
            StartCoroutine(waitForPlayersToStart());
            
        }
        else
        {
            AllPlayersReady = true;
            SetTurn();

             if (myIndex == 0)
             {
                //BotTurn();//ju1026
                SetMyTurn();
                playerObjects[0].dice.GetComponent<GameDiceController>().DisableDiceShadow();
             }
             else
             {
                 SetOpponentTurn();
                 playerObjects[currentPlayerIndex].dice.GetComponent<GameDiceController>().DisableDiceShadow();
             }
        }
    }

    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }

    public void TIPButton()
    {
        if (TIPObject.activeSelf)
        {
            TIPObject.SetActive(false);
        }
        else
        {
            TIPObject.SetActive(true);
        }
    }

    public void FacebookShare()
    {
        if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
        {

            Uri myUri = new Uri("https://play.google.com/store/apps/details?id=" + StaticStrings.AndroidPackageName);
#if UNITY_IPHONE
            myUri = new Uri("https://itunes.apple.com/us/app/apple-store/id" + StaticStrings.ITunesAppID);
#endif

            FB.ShareLink(
                myUri,
                StaticStrings.facebookShareLinkTitle,
                callback: ShareCallback
            );
        }
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            GameManager.Instance.playfabManager.addCoinsRequest(StaticStrings.rewardCoinsForShareViaFacebook);
            Debug.Log("ShareLink success!");
        }
    }

    public void StopAndFinishGame()
    {
        StopTimers();
        SetFinishGame(GameManager.Instance.myPlayerData.GetPlayerId(), true);
        ShowGameFinishWindow();
    }

    public void ShareScreenShot()
    {

#if UNITY_ANDROID
        string text = StaticStrings.ShareScreenShotText;
        text = text + " " + "https://play.google.com/store/apps/details?id=" + StaticStrings.AndroidPackageName;
        ScreenShotController.GetComponent<NativeShare>().ShareScreenshotWithText(text);
#elif UNITY_IOS
        string text = StaticStrings.ShareScreenShotText;
        text = text + " " + "https://itunes.apple.com/us/app/apple-store/id" + StaticStrings.ITunesAppID;
        ScreenShotController.GetComponent<NativeShare>().ShareScreenshotWithText(text);
#endif


    }

    public void ShowGameFinishWindow()
    {
        if (!FinishWindowActive)
        {
            FinishWindowActive = true;

            List<PlayerObject> otherPlayers = new List<PlayerObject>();
            
            for (int i = 0; i < playerObjects.Count; i++)
            {
                PlayerAvatarController controller = playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>();
                if (controller.Active && !controller.finished)
                {
                    otherPlayers.Add(playerObjects[i]);
                }
            }

			//Update apache after game finished
			string url = StaticStrings.UApacheUrl+"finishGame";
            string winnerId = playersFinished [0].id;
            
      

            GameFinishWindow.GetComponent<GameFinishWindowController>().showWindow(playersFinished, otherPlayers, firstPlacePrize, secondPlacePrize);
        }
    }

    private void ButtonClick(string id)
    {

        int index = 0;

        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i].id == id)
            {
                index = i;
                break;
            }
        }

        CurrentPlayerID = id;

        if (playerObjects[index].AvatarObject.GetComponent<PlayerAvatarController>().Active)
        {
            PlayerInfoWindow.GetComponent<PlayerInfoController>().ShowPlayerInfo(playerObjects[index].avatar, playerObjects[index].name, playerObjects[index].data);
        }

    }

    public void AddFriendButtonClick()
    {
        /*if (!CurrentPlayerID.Contains("_BOT"))
        {
            AddFriendRequest request = new AddFriendRequest()
            {
                FriendPlayFabId = CurrentPlayerID,
            };

            PlayFabClientAPI.AddFriend(request, (result) =>
            {
                PhotonNetwork.RaiseEvent((int)EnumPhoton.AddFriend, PhotonNetwork.playerName + ";" + GameManager.Instance.nameMy + ";" + CurrentPlayerID, true, null);
                addedFriendWindow.SetActive(true);
                Debug.Log("Added friend successfully");
            }, (error) =>
            {
                addedFriendWindow.SetActive(true);
                Debug.Log("Error adding friend: " + error.Error);
            }, null);
        }
        else
        {
            Debug.Log("Add Friend - It's bot!");
            addedFriendWindow.SetActive(true);
        }*/
    }
    

    public void FinishedGame()
    {
        if (GameManager.Instance.currentPlayer.id == GameManager.Instance.myPlayerData.GetPlayerId())
        {
            SetFinishGame(GameManager.Instance.currentPlayer.id, true);
        }
        else
        {
            SetFinishGame(GameManager.Instance.currentPlayer.id, false);
        }
        
    }

    private void SetFinishGame(string id, bool me)
    {
        if (!me || !iFinished)
        {
            ActivePlayersInRoom--;

            
            int index = GetPlayerPosition(id);
            
            if(index != -1)
            {
                playersFinished.Add(playerObjects[index]);
            }
           


            PlayerAvatarController controller = playerObjects[index].AvatarObject.GetComponent<PlayerAvatarController>();
            controller.Name.GetComponent<Text>().text = "";
            controller.Active = false;
            controller.finished = true;

            playerObjects[index].dice.SetActive(false);

            int position = playersFinished.Count;
            if (position == 1)
            {
                controller.Crown.SetActive(true);
            }

            if (me)
            {
                iFinished = true;
                if (ActivePlayersInRoom >= 0)
                {
                    //PhotonNetwork.RaiseEvent((int)EnumPhoton.FinishedGame, PhotonNetwork.player.NickName, true, null);
                    Dictionary<string, string> dicData = new Dictionary<string, string>();


                    dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
                    dicData.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());
                    JSONObject jsonData = new JSONObject(dicData);

                    socketIO.Emit("FINISHED_GAME_EVENT", jsonData);
                    SendFinishTurn();
                }




                if (position == 1)
                {
                    WinSound.Play();                    

                    GameManager.Instance.myPlayerData.user_data[MyPlayerData.CoinsKey] = (currentCoin + firstPlacePrize).ToString();
                    print("qq-- " + currentCoin + "   2222---" + firstPlacePrize);
                    GameManager.Instance.myPlayerData.user_data[MyPlayerData.TotalEarningsKey] = (GameManager.Instance.myPlayerData.GetTotalEarnings() + firstPlacePrize).ToString();
//                     if (GameManager.Instance.type == MyGameType.TwoPlayer)
//                     {
//                         GameManager.Instance.myPlayerData.user_data.Add(MyPlayerData.TwoPlayerWinsKey, (GameManager.Instance.myPlayerData.GetTwoPlayerWins() + 1).ToString());
//                     }
//                     else if (GameManager.Instance.type == MyGameType.FourPlayer)
//                     {
//                         GameManager.Instance.myPlayerData.user_data.Add(MyPlayerData.FourPlayerWinsKey, (GameManager.Instance.myPlayerData.GetFourPlayerWins() + 1).ToString());
//                     }
                    JSONObject jsonData = new JSONObject(GameManager.Instance.myPlayerData.user_data);

                    socketIO.Emit("UPDATE_USER_INFO", jsonData);

                    string url = StaticStrings.UApacheUrl+"updateUserDataC";

					string coinsMoney = GameManager.Instance.myPlayerData.GetCoins().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
					string totalEarnings = GameManager.Instance.myPlayerData.GetTotalEarnings().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
					int totalGamePlayed = GameManager.Instance.myPlayerData.GetPlayedGamesCount();
					string playFabId = GameManager.Instance.playfabManager.PlayFabId;
                }
                else if (position == 2)
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    print("I LOST ----------------------------------------");
                    //data.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() + secondPlacePrize).ToString());
                    GameManager.Instance.myPlayerData.user_data[MyPlayerData.CoinsKey] = currentCoin.ToString(); 
                    //GameManager.Instance.myPlayerData.user_data[MyPlayerData.CoinsKey] = (GameManager.Instance.myPlayerData.GetCoins() + secondPlacePrize).ToString();
                    //data.Add(MyPlayerData.TotalEarningsKey, (GameManager.Instance.myPlayerData.GetTotalEarnings() + secondPlacePrize).ToString());
                    JSONObject jsonData = new JSONObject(GameManager.Instance.myPlayerData.user_data);

                    socketIO.Emit("UPDATE_USER_INFO", jsonData);
                }
            }
            else if (GameManager.Instance.currentPlayer.isBot)
            {
                SendFinishTurn();
            }



            controller.setPositionSprite(position);


            CheckPlayersIfShouldFinishGame();
        }
    }

    public int GetPlayerPosition(string id)
    {
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i].id.Equals(id))
            {
                return i;
            }
        }
        return -1;
    }

    public void SendFinishTurn()
    {
        if (!FinishWindowActive && ActivePlayersInRoom > 1)
        {
            if (GameManager.Instance.currentPlayer.isBot)
            {
                BotDelay();
            }
            else
            {
                //PhotonNetwork.RaiseEvent((int)EnumPhoton.NextPlayerTurn, myIndex, true, null);

                Dictionary<string, string> dicData = new Dictionary<string, string>();

                print("myIndex ----  : " + myIndex);
                dicData.Add("index", "" + myIndex);
                dicData.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());
                JSONObject jsonData = new JSONObject(dicData);

                socketIO.Emit("FINISH_TURN_EVENT", jsonData);

                setCurrentPlayerIndex(myIndex);

                
                SetTurn();
                //SetOpponentTurn();

                GameManager.Instance.miniGame.setOpponentTurn();
            }
        }
    }

    

    public void OnEvent(SocketIOEvent evt)
    {

        Dictionary<string, string> data = new Dictionary<string, string>();

        data = evt.data.ToDictionary();

        int eventcode = int.Parse(data["event"]);
        
        if(GameManager.Instance.myPlayerData.GetLoginRoomId().Equals(data["roomId"]))
        {

            if (eventcode == (int)EnumPhoton.NextPlayerTurn)
            {                
                int content = int.Parse(data["index"]);
                print("NextPlayerTurn ---- : " + content);
                if (playerObjects[content].AvatarObject.GetComponent<PlayerAvatarController>().Active &&
                    currentPlayerIndex == content)
                {
                    if (!FinishWindowActive)
                    {
                        print("NextPlayerTurn123123");

                        setCurrentPlayerIndex(content);

                        SetTurn();
                    }
                }
            }
            else if (eventcode == (int)EnumPhoton.SendChatMessage)
            {
                for (int i = 0; i < playerObjects.Count; i++)
                {
                    if (playerObjects[i].id.Equals(data["userId"]))
                    {
                        playerObjects[i].ChatBubbleText.SetActive(true);
                        playerObjects[i].ChatbubbleImage.SetActive(false);
                        playerObjects[i].ChatBubbleText.GetComponent<Text>().text = data["index"];
                        playerObjects[i].ChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
                    }
                }
            }
            else if (eventcode == (int)EnumPhoton.SendChatEmojiMessage)
            {
                for (int i = 0; i < playerObjects.Count; i++)
                {
                    if (playerObjects[i].id.Equals(data["userId"]))
                    {
                        playerObjects[i].ChatBubbleText.SetActive(false);
                        playerObjects[i].ChatbubbleImage.SetActive(true);
                        int index = int.Parse(data["index"]);

                        if (index > emojiSprites.Length - 1)
                        {
                            index = emojiSprites.Length;
                        }
                        playerObjects[i].ChatbubbleImage.GetComponent<Image>().sprite = emojiSprites[index];
                        playerObjects[i].ChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");
                    }
                }
            }
            else if (eventcode == (int)EnumPhoton.FinishedGame)
            {
                print("Finished Game");
                string message = data["userId"];
                SetFinishGame(message, false);
            }
            //             else if (eventcode == (int)EnumPhoton.AddFriend)
            //             {
            //                 if (PlayerPrefs.GetInt(StaticStrings.FriendsRequestesKey, 0) == 0)
            //                 {
            //                     string[] data = ((string)content).Split(';');
            //                     /*if (PhotonNetwork.playerName.Equals(data[2]))*/
            //                     invitiationDialog.GetComponent<PhotonChatListener2>().showInvitationDialog(data[0], data[1], null);
            //                 }
            //                 else
            //                 {
            //                     Debug.Log("Invitations OFF");
            //                 }
            // 
            //             }

        }
    }

	public void closeMissedPnael(){
		MissedModalPanel.SetActive(false);
	}

    private void SetMyTurn()
    {
        GameManager.Instance.isMyTurn = true;

        if (GameManager.Instance.miniGame != null)
            GameManager.Instance.miniGame.setMyTurn();
		
		
		if ( StaticStrings.missedChance == 2) {
			MissedModalPanel.SetActive(true);
			textMsg.text = "Please roll dice, this is your last chance.";
		}
		if ( StaticStrings.missedChance == 3) {
			// Show game off message.
			MissedModalPanel.SetActive(false);
			MissedModalPanel.SetActive(true);
			closeBtn.gameObject.SetActive(false);
			textMsg.text = "Oop's your game has ended. Please play another game, Good Luck!";

			//ToDo
			// Make this comment live after testing...
			//LeaveGame(true);

		}
        StartTimer();
		//Debug.Log ("Common missed chance: "+StaticStrings.missedChance);
    }

    private void BotTurn()
    {
        oppoTurnSource.Play();
        //GameManager.Instance.currentPlayer = playerObjects[currentPlayerIndex];
        GameManager.Instance.isMyTurn = false;
        StartTimer();


        GameManager.Instance.miniGame.BotTurn(true);

        //Invoke("BotDelay", 2.0f);

    }

    private void SetTurn()
    {
        for (int i = 0; i < playerObjects.Count; i++)
        {
            playerObjects[i].dice.GetComponent<GameDiceController>().EnableDiceShadow();
        }

        playerObjects[currentPlayerIndex].dice.GetComponent<GameDiceController>().DisableDiceShadow();

        GameManager.Instance.currentPlayer = playerObjects[currentPlayerIndex];

        if (playerObjects[currentPlayerIndex].id == myId)
        {
            //BotTurn();//ju1026
            SetMyTurn();
        }
        else if (playerObjects[currentPlayerIndex].isBot)
        {
            BotTurn();
        }
        else
        {
            SetOpponentTurn();
        }
    }

    private void BotDelay()
    {
        if (!FinishWindowActive)
        {
            setCurrentPlayerIndex(currentPlayerIndex);
            SetTurn();
        }

    }


    private void setCurrentPlayerIndex(int current)
    {

        while (true)
        {
            current = current + 1;
            currentPlayerIndex = (current) % playerObjects.Count;
            GameManager.Instance.currentPlayer = playerObjects[currentPlayerIndex];
            if (playerObjects[currentPlayerIndex].AvatarObject.GetComponent<PlayerAvatarController>().Active)
                break;
        }

    }

    private void SetOpponentTurn()
    {
        oppoTurnSource.Play();
        GameManager.Instance.isMyTurn = false;
        /*if (playerObjects[currentPlayerIndex].id.Contains("_BOT"))
        {
            BotTurn();
        }*/

        StartTimer();
    }

    private void StartTimer()
    {
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (i == currentPlayerIndex)
            {
                playerObjects[currentPlayerIndex].timer.SetActive(true);
            }
            else
            {
                playerObjects[i].timer.SetActive(false);
            }
        }
    }

    public void StopTimers()
    {
        for (int i = 0; i < playerObjects.Count; i++)
        {
            playerObjects[i].timer.SetActive(false);
        }
    }

    public void PauseTimers()
    {
        playerObjects[currentPlayerIndex].timer.GetComponent<UpdatePlayerTimer>().Pause();
    }

    public void restartTimer()
    {
        playerObjects[currentPlayerIndex].timer.GetComponent<UpdatePlayerTimer>().restartTimer();
    }



    public void CheckPlayersIfShouldFinishGame()
    {
        if (!FinishWindowActive)
        {
            if ((ActivePlayersInRoom == 1 && !iFinished))
            {
                StopAndFinishGame();
                return;
            }

            if (ActivePlayersInRoom == 0)
            {
                StopAndFinishGame();
                return;
            }

            if (iFinished && ActivePlayersInRoom == 1 && CheckIfOtherPlayerIsBot())
            {
                AddBotToListOfWinners();
                StopAndFinishGame();
                return;
            }

            if (ActivePlayersInRoom > 1 && iFinished)
            {
                TIPButtonObject.SetActive(true);
            }
            
        }
    }
    public void AddBotToListOfWinners()
    {
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i].id.Contains("_BOT") && playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().Active)
            {
                playersFinished.Add(playerObjects[i]);
            }
        }
    }

    public bool CheckIfOtherPlayerIsBot()
    {
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i].id.Contains("_BOT") && playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().Active)
            {
                playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().finished = true;
                return true;
            }
        }
        return false;
    }

    

    public void OnDeletePlayerResult(SocketIOEvent evt)
    {
        Dictionary<string, string> recData = new Dictionary<string, string>();

        recData = evt.data.ToDictionary();
        Debug.Log("ROOM ID : " + recData["roomId"]);

        if(recData["roomId"].Equals(GameManager.Instance.myPlayerData.GetLoginRoomId()))
        {
            for (int i = 0; i < playerObjects.Count; i++)
            {
                if (playerObjects[i].id.Equals(recData["userId"]))
                {
                    Debug.Log("Disconnected player name : " + playerObjects[i].name);
                    setPlayerDisconnected(i);
                    CheckPlayersIfShouldFinishGame();
                }
            }
        }
    }



    public void setPlayerDisconnected(int i)
    {
        requiredToStart--;
        if (!FinishWindowActive)
        {
            if (!playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().finished)
                ActivePlayersInRoom--;

            if (currentPlayerIndex == i && ActivePlayersInRoom > 1)
            {

                setCurrentPlayerIndex(currentPlayerIndex);
                if (AllPlayersReady)
                    SetTurn();
            }
				
			// Send user offline to server.
			string urlGameOffline = StaticStrings.UApacheUrl+"playerDisconnect";

            playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().PlayerLeftRoom();

            // LUDO
            playerObjects[i].dice.SetActive(false);
            if (!playerObjects[i].AvatarObject.GetComponent<PlayerAvatarController>().finished)
            {
                for (int j = 0; j < playerObjects[i].pawns.Length; j++)
                {
                    // playerObjects[i].pawns[j].SetActive(false);
                    playerObjects[i].pawns[j].GetComponent<LudoPawnController>().GoToInitPosition(false);
                }
            }
            // END LUDO
        }
    }

    public void LeaveGame(bool finishWindow)
    {


        GameManager.Instance.resetAllData();
        GameManager.Instance.gameSceneStarted = false;
        Dictionary<string, string> dicData = new Dictionary<string, string>();
        dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
        dicData.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());
        JSONObject jsonData = new JSONObject(dicData);


        Debug.Log("I LEAVE GAME");
        socketIO.Emit("I_LEAVE_GAME", jsonData);
        //socketIO.Emit("FINISHED_GAME_EVENT", jsonData);


        print("POINT --- 4");
        SceneManager.LoadScene("MenuScene");
    }

    public void ShowHideChatWindow()
    {
        if (!ChatWindow.activeSelf)
        {
            ChatWindow.SetActive(true);
            ChatButton.GetComponent<Text>().text = "X";
        }
        else
        {
            ChatWindow.SetActive(false);
            ChatButton.GetComponent<Text>().text = "CHAT";
        }
    }
	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;

		if (www.error == null)
		{
			Debug.Log("OK: " + www.text);
		}
		else
		{
			Debug.Log("ERROR: " + www.error);
		}
	}
	IEnumerator ExecuteAfterTime(float time)
	{
		bool isCoroutineExecuting = false;
		if (isCoroutineExecuting)
			yield break;
		isCoroutineExecuting = true;
		yield return new WaitForSeconds(time);

		PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 1) + 1);
		SceneManager.LoadScene("MenuScene");

		GameManager.Instance.playfabManager.roomOwner = false;
		GameManager.Instance.roomOwner = false;
		GameManager.Instance.resetAllData();

		isCoroutineExecuting = false;
	}


}
