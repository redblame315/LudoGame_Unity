using UnityEngine;
using System.Collections;
using PlayFab;
using System;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using AssemblyCSharp;
using System.Globalization;
using UnityEngine.Networking;
using SocketIO;


public class PlayFabManager : MonoBehaviour
{

    private Sprite[] avatarSprites;

    public string PlayFabId;
    public string authToken;
    public bool multiGame = true;
    public bool roomOwner = false;
    private FacebookManager fbManager;
    public GameObject fbButton;
    private FacebookFriendsMenu facebookFriendsMenu;
    private bool alreadyGotFriends = false;
    public GameObject menuCanvas;
    public GameObject MatchPlayersCanvas;
    public GameObject splashCanvas;
    public bool opponentReady = false;
    public bool imReady = false;
    public GameObject playerAvatar;
    public GameObject playerName;
    public GameObject backButtonMatchPlayers;


    public GameObject loginEmail;
    public GameObject loginPassword;
    public GameObject loginInvalidEmailorPassword;
    public GameObject loginCanvas;


    public GameObject regiterEmail;
    public GameObject registerPassword;
    public GameObject registerNickname;
    public GameObject registerInvalidInput;
    public GameObject registerCanvas;

    public GameObject resetPasswordEmail;
    public GameObject resetPasswordInformationText;

    public SocketIOComponent socketIO;


    public bool isInLobby = false;
    public bool isInMaster = false;

    void Awake()
    {
        //Debug.Log("Playfab awake");
        //PlayerPrefs.DeleteAll();
        DontDestroyOnLoad(socketIO.transform.gameObject);
        DontDestroyOnLoad(transform.gameObject);
    }
    
    public void destroy()
    {
    }

    // Use this for initialization
    void Start()
    {
/*        Debug.Log("Playfab start");*/
        GameManager.Instance.playfabManager = this;
        fbManager = GameObject.Find("FacebookManager").GetComponent<FacebookManager>();
        facebookFriendsMenu = GameManager.Instance.facebookFriendsMenu;
        avatarSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars;
    }

    void Update()
    {
        /*if (chatClient != null) { chatClient.Service(); }*/
    }



    // handle events:
//     private void OnEvent(byte eventcode, object content, int senderid)
//     {
// 
//         Debug.Log("Received event: " + (int)eventcode + " Sender ID: " + senderid);
// 
//         if (eventcode == (int)EnumPhoton.BeginPrivateGame)
//         {
//             //StartGame();
//             LoadGameScene();
//         }
//         else if (eventcode == (int)EnumPhoton.StartWithBots/* && senderid != PhotonNetwork.player.ID*/)
//         {
//             LoadBots();
//         }
//         else if (eventcode == (int)EnumPhoton.StartGame)
//         {
//             LoadGameScene();
//         }
//         else if (eventcode == (int)EnumPhoton.ReadyToPlay)
//         {
//             GameManager.Instance.readyPlayersCount++;
//             //LoadGameScene();
//         }
// 
//     }

//     public void LoadGameWithDelay()
//     {
//         LoadGameScene();
//     }
    
    

    public void StartGame()
    {
        Debug.Log("Start Game Play fab manager");
        GameManager.Instance.botDiceValues = new List<int>();
        GameManager.Instance.botDelays = new List<float>();
        string[] d1 = new string[2];
        d1[0] = "2,3,4,5,6,3,6,3,1,4,2,2,6,1,3,1,5,6,2,5,6,3,4,2,3,4,1,5,6,3,2,1,4,2,3,2,1,5,4,4,3,2,6,5,4,3,6,2,1,4,5,6,3,3,4,2,2,3,2,1,4,5,6,3,2,1,3,4,2,4,1,5";
        d1[1] = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1";


        string[] diceValues = d1[0].Split(',');
        for (int i = 0; i < diceValues.Length; i++)
        {
            GameManager.Instance.botDiceValues.Add(int.Parse(diceValues[i]));
        }

        string[] delays = d1[1].Split(',');
        for (int i = 0; i < delays.Length; i++)
        {
            GameManager.Instance.botDelays.Add(float.Parse(delays[i]));
        }

        waitAndStartGame();
    }

    private void waitAndStartGame()
    {
        Debug.Log("Ready : " + GameManager.Instance.readyPlayers + " Require : " + GameManager.Instance.requiredPlayers);

        startGameScene();
        GameManager.Instance.readyPlayers = 0;
        opponentReady = false;
        imReady = false;
    }

    public void startGameScene()
    {
        Debug.Log("Start Game Scene");
        
        LoadGameScene();
    }


    public void LoadGameScene()
    {
        Dictionary<string, string> startData = new Dictionary<string, string>();

        startData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
        startData.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());

        Debug.Log("Admin_Coin ---- " + GameManager.Instance.myPlayerData.GetAdminCoin());
        startData.Add("admin_coin", GameManager.Instance.myPlayerData.GetAdminCoin());
        JSONObject jStartData = new JSONObject(startData);
        socketIO.Emit("GAME_START", jStartData);

    }



    public void WaitForNewPlayer()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        Debug.Log("room_id"+GameManager.Instance.myPlayerData.GetLoginRoomId());
        data.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());

        JSONObject jsonData = new JSONObject(data);

/*        print("Send to server for get opponent");*/
        socketIO.Emit("GET_OPPONENT", jsonData);
        
    }
    
//     
//     public void StartGameWithBots()
//     {
//         LoadBots();
//     }

    public void LoadBots()
    {
/*        Debug.Log("Close room - add bots");*/
        
        AddBots();

    }

    public void AddBots()
    {
        // Add Bots here

        Debug.Log("Add Bots with delay : " + GameManager.Instance.requiredPlayers);



        for (int i = 0; i < GameManager.Instance.requiredPlayers - 1; i++)
        {
            
            if (GameManager.Instance.opponentsIDs[i] == null)
            {
                Debug.Log("AddBots : " + i + " : " + GameManager.Instance.opponentsIDs[i]);
                AddBot(i);
            }
        }
    }


    public void AddBot(int i)
    {
        GameManager.Instance.opponentsAvatars[i] = avatarSprites[UnityEngine.Random.Range(0, avatarSprites.Length - 1)];
        GameManager.Instance.opponentsIDs[i] = "_BOT" + i;
        GameManager.Instance.opponentsNames[i] = "Guest" + UnityEngine.Random.Range(100000, 999999);
        Debug.Log("Name: " + GameManager.Instance.opponentsNames[i]);

        if(GameManager.Instance.controlAvatars == null)
        {
            Debug.Log(GameManager.Instance.linesLength + " NULL!!!!!!!!!!!!!!!!!!!!!!!!");
            
        }
        GameManager.Instance.controlAvatars.PlayerJoined(i, "_BOT" + i);
    }

    public void resetPassword()
    {
        resetPasswordInformationText.SetActive(false);
    }

    public void setInitNewAccountData(bool fb)
    {
        Dictionary<string, string> data = MyPlayerData.InitialUserData(fb);
        GameManager.Instance.myPlayerData.user_data = data;
        JSONObject jsonData = new JSONObject(GameManager.Instance.myPlayerData.user_data);

        socketIO.Emit("UPDATE_USER_INFO", jsonData);
    }


    public void updateBoughtChats(int index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
//         data.Add(MyPlayerData.ChatsKey, GameManager.Instance.myPlayerData.GetChats() + ";'" + index + "'");
// 
//         GameManager.Instance.myPlayerData.UpdateUserData(data);


    }

    public void UpdateBoughtEmojis(int index)
    {
//        Dictionary<string, string> data = new Dictionary<string, string>();
//         data.Add(MyPlayerData.EmojiKey, GameManager.Instance.myPlayerData.GetEmoji() + ";'" + index + "'");
// 
// 
//         GameManager.Instance.myPlayerData.UpdateUserData(data);
    }

    public void addCoinsRequest(int count)
    {
        GameManager.Instance.myPlayerData.user_data[MyPlayerData.CoinsKey] = "" + (GameManager.Instance.myPlayerData.GetCoins() + count);
        JSONObject jsonData = new JSONObject(GameManager.Instance.myPlayerData.user_data);

        socketIO.Emit("UPDATE_USER_INFO", jsonData);
    }

    public void getPlayerDataRequest()
    {
        Debug.Log("Get player data request!!");
    }


    private IEnumerator loadSceneMenu()
    {
        yield return new WaitForSeconds(0.1f);

        if (isInMaster && isInLobby)
        {
            print("POINT --- 7");
            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            StartCoroutine(loadSceneMenu());
        }

    }



    /*public void LinkFacebookAccount()
    {
        LinkFacebookAccountRequest request = new LinkFacebookAccountRequest()
        {
            AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString,
            ForceLink = true
        };

        PlayFabClientAPI.LinkFacebookAccount(request, (result) =>
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("LoggedType", "Facebook");
            data.Add("FacebookID", Facebook.Unity.AccessToken.CurrentAccessToken.UserId);
            data.Add("PlayerAvatarUrl", GameManager.Instance.avatarMyUrl);
            data.Add(MyPlayerData.PlayerName, GameManager.Instance.nameMy);
            data.Add(MyPlayerData.AvatarIndexKey, "fb");
            data.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() + StaticStrings.CoinsForLinkToFacebook).ToString());
            GameManager.Instance.myAvatarGameObject.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
            GameManager.Instance.myNameGameObject.GetComponent<Text>().text = GameManager.Instance.nameMy;
            GameManager.Instance.myPlayerData.UpdateUserData(data);

            GameManager.Instance.FacebookLinkButton.SetActive(false);
        },
        (error) =>
        {
            Debug.Log("Error linking facebook account: " + error.ErrorMessage + "\n" + error.ErrorDetails);
            GameManager.Instance.connectionLost.showDialog();
        });



    }*/

    public void LoginWithFacebook()
    {
        /*LoginWithFacebookRequest request = new LoginWithFacebookRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString
        };

        PlayFabClientAPI.LoginWithFacebook(request, (result) =>
        {
            PlayFabId = result.PlayFabId;
            Debug.Log("Got PlayFabID: " + PlayFabId);



            if (result.NewlyCreated)
            {
                Debug.Log("(new account)");
                setInitNewAccountData(true);
                Dictionary<string, string> data1 = new Dictionary<string, string>();
                data1.Add(MyPlayerData.AvatarIndexKey, "fb");
                GameManager.Instance.myPlayerData.UpdateUserData(data1);
            }
            else
            {
                CheckIfFirstTitleLogin(PlayFabId, true);
                Debug.Log("(existing account)");
            }


            UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
            {
                DisplayName = GameManager.Instance.playfabManager.PlayFabId
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
            {
                Debug.Log("Title Display name updated successfully");
            }, (error) =>
            {
                Debug.Log("Title Display name updated error: " + error.Error);

            }, null);


            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("LoggedType", "Facebook");
            data.Add("FacebookID", Facebook.Unity.AccessToken.CurrentAccessToken.UserId);
            if (result.NewlyCreated)
                data.Add("PlayerName", GameManager.Instance.nameMy);
            else
            {
                GetUserDataRequest getdatarequest = new GetUserDataRequest()
                {
                    PlayFabId = result.PlayFabId,

                };

                PlayFabClientAPI.GetUserData(getdatarequest, (result2) =>
                {

                    Dictionary<string, UserDataRecord> data2 = result2.Data;

                    if (data2.ContainsKey("PlayerName"))
                    {
                        GameManager.Instance.nameMy = data2["PlayerName"].Value;
                    }
                    else
                    {
                        Dictionary<string, string> data5 = new Dictionary<string, string>();
                        data5.Add("PlayerName", GameManager.Instance.nameMy);
                        data5.Add(MyPlayerData.AvatarIndexKey, "fb");
                        GameManager.Instance.myPlayerData.UpdateUserData(data5);
                    }
                }, (error) =>
                {
                    Debug.Log("Data updated error " + error.ErrorMessage);
                }, null);
            }
            data.Add("PlayerAvatarUrl", GameManager.Instance.avatarMyUrl);

            GameManager.Instance.myPlayerData.UpdateUserData(data);


            GetPhotonToken();

        },
            (error) =>
            {
                Debug.Log("Error logging in player with custom ID: " + error.ErrorMessage + "\n" + error.ErrorDetails);
                GameManager.Instance.connectionLost.showDialog();
            });*/
    }
    
    private string androidUnique()
    {
        AndroidJavaClass androidUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityPlayerActivity = androidUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject unityPlayerResolver = unityPlayerActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass androidSettingsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
        return androidSettingsSecure.CallStatic<string>("getString", unityPlayerResolver, "android_id");
    }

    public void LoginWithEmailAccount()
    {


        loginInvalidEmailorPassword.SetActive(false);



        string email = "";
        string password = "";
        if (PlayerPrefs.HasKey("email_account"))
        {
            email = PlayerPrefs.GetString("email_account");
            password = PlayerPrefs.GetString("password");
        }
        else
        {
            email = loginEmail.GetComponent<Text>().text;
            password = loginPassword.GetComponent<Text>().text;

        }
    }

    public void Login()
    {
        string customId = "";
        if (PlayerPrefs.HasKey("unique_identifier"))
        {
            customId = PlayerPrefs.GetString("unique_identifier");
        }
        else
        {
            customId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("unique_identifier", customId);
        }
        

        Debug.Log("UNIQUE IDENTIFIER: " + customId);
    }

    public void GetPlayfabFriends()
    {
        if (alreadyGotFriends)
        {
            Debug.Log("show firneds FFFF");
            if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
            {
                fbManager.getFacebookInvitableFriends();
            }
            else
            {

                facebookFriendsMenu.showFriends(null, null, null);
            }
        }
        else
        {
            Debug.Log("IND");
        }


    }


    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Playfab Error: " + error.ErrorMessage);
    }

   
    public void connectToChat()
    {
    }
    


    public void OnConnected()
    {
        Debug.Log("Photon Chat connected!!!");
    }

    
    public void showMenu()
    {
        menuCanvas.gameObject.SetActive(true);

        playerName.GetComponent<Text>().text = GameManager.Instance.nameMy;

        if (GameManager.Instance.avatarMy != null)
            playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;

        splashCanvas.SetActive(false);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to CHAT - set online status!");
    }


    public void challengeFriend(string id, string message)
    {
        GameManager.Instance.invitationID = id;
        Debug.Log("Send invitation to: " + id);
    }
    
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (!sender.Equals(this.PlayFabId))
        {
            if (message.ToString().Contains("INVITE_TO_PLAY_PRIVATE"))
            {
                GameManager.Instance.invitationID = sender;

                string[] messageSplit = message.ToString().Split(';');
                string whoInvite = messageSplit[1];
                string payout = messageSplit[2];
                string roomID = messageSplit[3];
                GameManager.Instance.payoutCoins = int.Parse(payout);
                GameManager.Instance.invitationDialog.GetComponent<PhotonChatListener>().showInvitationDialog(0, whoInvite, payout, roomID, 0);
            }
        }

        if ((GameManager.Instance.invitationID.Length == 0 || !GameManager.Instance.invitationID.Equals(sender)))
        {

        }
        else
        {
            GameManager.Instance.invitationID = "";
        }
    }


    public void switchUser()
    {
        GameManager.Instance.playfabManager.destroy();
        GameManager.Instance.facebookManager.destroy();
        GameManager.Instance.connectionLost.destroy();
        GameManager.Instance.avatarMy = null;
        GameManager.Instance.logged = false;
        GameManager.Instance.resetAllData();
        SceneManager.LoadScene("LoginSplash");
    }

    public void OnDisconnected()
    {
        Debug.Log("Chat disconnected - Reconnect");
        connectToChat();
    }



    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {

    }

    public void OnUnsubscribed(string[] channels)
    {

    }


    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("STATUS UPDATE CHAT!");
        Debug.Log("Status change for: " + user + " to: " + status);

        bool foundFriend = false;
        for (int i = 0; i < GameManager.Instance.friendsStatuses.Count; i++)
        {
            string[] friend = GameManager.Instance.friendsStatuses[i];
            if (friend[0].Equals(user))
            {
                GameManager.Instance.friendsStatuses[i][1] = "" + status;
                foundFriend = true;
                break;
            }
        }

        if (!foundFriend)
        {
            GameManager.Instance.friendsStatuses.Add(new string[] { user, "" + status });
        }

        if (GameManager.Instance.facebookFriendsMenu != null)
            GameManager.Instance.facebookFriendsMenu.updateFriendStatus(status, user);
    }
    

    public void JoinRoomAndStartGame()
    {
        //StartCoroutine("WaitForNewPlayer");
        Invoke("WaitForNewPlayer", 10.0f);
        
    }

    public string generateBotMoves()
    {
        // Generate BOT moves
        string BotMoves = "";
        int BotCount = 100;
        // Generate dice values
        for (int i = 0; i < BotCount; i++)
        {
            BotMoves += (UnityEngine.Random.Range(1, 7)).ToString();
            if (i < BotCount - 1)
            {
                BotMoves += ",";
            }
        }

        BotMoves += ";";

        // Generate delays
        float minValue = GameManager.Instance.playerTime / 10;
        if (minValue < 1.5f) minValue = 1.5f;
        for (int i = 0; i < BotCount; i++)
        {
            BotMoves += (UnityEngine.Random.Range(minValue, GameManager.Instance.playerTime / 8)).ToString();
            if (i < BotCount - 1)
            {
                BotMoves += ",";
            }
        }
        return BotMoves;
    }

    public void extractBotMoves(string data)
    {
        GameManager.Instance.botDiceValues = new List<int>();
        GameManager.Instance.botDelays = new List<float>();
        string[] d1 = data.Split(';');


        string[] diceValues = d1[0].Split(',');
        for (int i = 0; i < diceValues.Length; i++)
        {
            GameManager.Instance.botDiceValues.Add(int.Parse(diceValues[i]));
        }

        string[] delays = d1[1].Split(',');
        for (int i = 0; i < delays.Length; i++)
        {
            GameManager.Instance.botDelays.Add(float.Parse(delays[i]));
        }
    }

    public void OnLeftLobby()
    {
        isInLobby = false;
        isInMaster = false;
    }

    
    public void CreatePrivateRoom()
    {
        GameManager.Instance.JoinedByID = false;

        string roomName = "";
        for (int i = 0; i < 8; i++)
        {
            roomName = roomName + UnityEngine.Random.Range(0, 10);
        }
    }

    public int GetFirstFreeSlot()
    {
        int index = 0;
        for (int i = 0; i < GameManager.Instance.opponentsIDs.Count; i++)
        {
            if (GameManager.Instance.opponentsIDs[i] == null)
            {
                index = i;
                break;
            }
        }
        return index;
    }

  
    private void GetPlayerDataRequest(string playerID)
    {

    }
    

    private void getOpponentData(int index, bool fbAvatar, int avatarIndex, string id)
    {
            Debug.Log("GET OPPONENT DATA: " + avatarIndex);
            GameManager.Instance.opponentsAvatars[index] = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars[avatarIndex];
            GameManager.Instance.controlAvatars.PlayerJoined(index, id);

    }

    public IEnumerator loadImageOpponent(string url, int index, string id)
    {
        WWW www = new WWW(url);

        yield return www;

        GameManager.Instance.opponentsAvatars[index] = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
        GameManager.Instance.controlAvatars.PlayerJoined(index, id);
    }
}
