using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.SceneManagement;
using AssemblyCSharp;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class SocketReceiver : MonoBehaviour {


    public SocketIOComponent socketIO;
    public GameObject content;
    private Sprite[] avatarSprites;
    public GameObject blockWindow;
    public GameObject showGameCongfig;
    public GameObject[] opponentAvatar;
    public GameObject[] opponentAvatarImage;
    public GameObject[] progressBar;
    public GameObject playerMatchWindow;
    public GameObject countDown;
    public GameObject timeCountDown;
    public GameObject roomIDText;
    public GameObject myAvatar;
    public GameObject startButton;
    public GameObject failedWindow;
    public GameObject userName;
    public GameObject password;
    public GameObject emailId;
    public GameObject mobileNumber;
    public GameObject playerName1;
    public GameObject playerName2;
    public GameObject invalidInput;
    public GameObject playerInfoWindow;
    public GameObject DialogObj;

    // Use this for initialization
    //private LudoGameController ludoController;

    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
        avatarSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars;

        

        //DontDestroyOnLoad(gameObject);
    }


    void Start () {        
        InvokeRepeating("UpdateDataSocket", 1.0f, 1.0f);
        socketIO.On("GETOPPONENET_RESULT", OnGetOpponentResult);
        socketIO.On("ENTERROOM_RESULT", OnEnterroomResult);
        socketIO.On("GET_USER_INFO", OnGetUserInfo);
        socketIO.On("START_GAME_RESULT", OnGetStartGameResult);
        socketIO.On("JOIN_ROOM", OnGetJoinRoom);
        socketIO.On("PRIVATE_ROOM_RESULT", OnGetPrivateRoomResult);
        socketIO.On("DELETE_PRIVATE_USER", OnGetDeletePrivateUser);
    }

    private void OnDestroy()
    {
        socketIO.Off("GETOPPONENET_RESULT", OnGetOpponentResult);
        socketIO.Off("ENTERROOM_RESULT", OnEnterroomResult);
        socketIO.Off("GET_USER_INFO", OnGetUserInfo);
        socketIO.Off("START_GAME_RESULT", OnGetStartGameResult);
        socketIO.Off("JOIN_ROOM", OnGetJoinRoom);
        socketIO.Off("PRIVATE_ROOM_RESULT", OnGetPrivateRoomResult);
        socketIO.Off("DELETE_PRIVATE_USER", OnGetDeletePrivateUser);
    }

    public void OnGetDeletePrivateUser(SocketIOEvent evt)
    {
        Debug.Log("GET MESSAGE TO DELETE CLIENT");
        Dictionary<string, string> data = evt.data.ToDictionary();
        Debug.Log(data["roomId"] + " : " + GameManager.Instance.myPlayerData.GetLoginRoomId());
        if(data["roomId"].Equals(GameManager.Instance.myPlayerData.GetLoginRoomId()))
        {
            Debug.Log("ROOM ID : " + data["roomId"]);
            if(data.ContainsKey("result"))
            {
                Debug.Log("RESULT : " + data["result"]);
                for (int i = 0; i < 3; i++)
                {
                    opponentAvatarImage[i].SetActive(false);
                    progressBar[i].SetActive(true);
                }
                
                Dictionary<string, string> dicData = new Dictionary<string, string>();
                dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
                dicData.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());
                JSONObject jsonData = new JSONObject(dicData);
                GameManager.Instance.myPlayerData.user_data["roomId"] = "";
                for (int i = 0; i < GameManager.Instance.opponentsIDs.Count; i++)
                {
                    GameManager.Instance.opponentsIDs[i] = null;
                    GameManager.Instance.opponentsIDs[i] = null;
                    GameManager.Instance.opponentsNames[i] = null;
                }

                Debug.Log("I LEAVE GAME");

                Debug.Log("CHECK POINT  ----2");

                print("NotEnoughCheck " + NotEnoughCheck);

                if(!NotEnoughCheck)              
                    failedWindow.SetActive(true);

                Debug.Log("I LEAVE GAME");
                socketIO.Emit("I_LEAVE_GAME", jsonData);

                return;
            }
            Debug.Log("DELETE CLIENT");
            Debug.Log("USER ID : " + GameManager.Instance.opponentsIDs.Count + " : " + int.Parse(data["userId"]));
            int opponentCnt = 0;
            for (int i = 0; i< GameManager.Instance.opponentsIDs.Count; i++)
            {
                Debug.Log("USER ID : " + GameManager.Instance.opponentsIDs[i] + " : " + int.Parse(data["userId"]));
                if(GameManager.Instance.opponentsIDs[i] == null)
                {
                    opponentCnt++;
                    continue;
                }
                if(GameManager.Instance.opponentsIDs[i].Equals(data["userId"]))
                {
                    GameManager.Instance.opponentsIDs[i] = null;
                    GameManager.Instance.opponentsNames[i] = null;
                    GameManager.Instance.opponentsAvatars[i] = null;
                    progressBar[i].SetActive(true);
                    opponentAvatarImage[i].SetActive(false);
                }
            }

            if(opponentCnt == 2)
            {
                startButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void OnGetJoinRoom(SocketIOEvent evt)
    {
        Debug.Log("Team Socket Receive !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }

    public void OnGetStartGameResult(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data = evt.data.ToDictionary();

        Debug.Log("Server Room ID : " + data["roomId"] + "MY Room ID : " + GameManager.Instance.myPlayerData.GetLoginRoomId());
        if (data["roomId"].Equals(GameManager.Instance.myPlayerData.GetLoginRoomId()))
        {
            GameManager.Instance.GameScene = "GameScene";

            if (!GameManager.Instance.gameSceneStarted)
            {
                SceneManager.LoadScene(GameManager.Instance.GameScene);
                GameManager.Instance.gameSceneStarted = true;
            }
        }
    }

    bool NotEnoughCheck = false;
    public void OnGetPrivateRoomResult(SocketIOEvent evt)  //Private Room
    {
        Dictionary<string, string> dicData = new Dictionary<string, string>();
        Dictionary<string, string> tempData = evt.data.ToDictionary();

        GameManager.Instance.opponentsAvatars.Clear();
        GameManager.Instance.opponentsIDs.Clear();
        GameManager.Instance.opponentsNames.Clear();

        print("count1 --- " + GameManager.Instance.opponentsAvatars.Count + "count2--- " + GameManager.Instance.opponentsIDs.Count + "count3 ---" + GameManager.Instance.opponentsNames.Count);


        if (tempData["result"].Equals("failed"))
        {
            Debug.Log("CHECK POINT  ----1");
            playerMatchWindow.SetActive(true);
            failedWindow.SetActive(true);

            return;
        }

        if(GameManager.Instance.myPlayerData.GetCoins() < int.Parse(tempData["entryCoin"]))
        {
            NotEnoughCheck = true;
            Debug.Log("You dont have enough money to join room.");
            DialogObj.SetActive(true);
            return;
        }

        NotEnoughCheck = false;

        string roomID = tempData["roomId"];
        Debug.Log("PRIVATE ROOM CODE : " + roomID);

        


        JSONObject jsonData = evt.data["data"];
        SetRoomInfo(tempData, jsonData.Count);
        GameManager.Instance.currentPlayersCount = evt.data["data"].Count;
        GameManager.Instance.readyPlayersCount = evt.data["data"].Count;
        Debug.Log(" MY ID : " + GameManager.Instance.myPlayerData.GetLoginRoomId() + " SERVER ROOM ID : " + roomID);


       

        if (roomID.Equals(GameManager.Instance.myPlayerData.GetLoginRoomId()))
        {
            Debug.Log("ARRAY LENGTH : " + evt.data["data"]);
            Debug.Log("ARRAY LENGTH : " + evt.data["data"].Count);

            Debug.Log("I AM LOG IN 1 ");

            for (int i = 0; i < jsonData.Count; i++)
            {
                dicData = jsonData[i].ToDictionary();

                int OpponentCoin = int.Parse(dicData["coins"]);
                int GameFee = GameManager.Instance.myPlayerData.GetEntryCoin();

                print("oppCoin --- " + OpponentCoin + "GameFee --" + GameFee);


                if(OpponentCoin < GameFee)
                {
                    startButton.GetComponent<Button>().interactable = false;
                    Debug.Log("Opponent coin is not enough.");
                    return;
                }

                Debug.Log("I AM LOG IN 2");
                if (!playerMatchWindow.activeSelf /*!mainUser.Equals(GameManager.Instance.myPlayerData.GetPlayerId())*/)
                {
                    roomIDText.SetActive(false);
                    startButton.SetActive(false);
                    playerMatchWindow.SetActive(true);
                    timeCountDown.SetActive(false);
                    countDown.SetActive(false);
                    myAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
                }
                else
                {
                    Debug.Log("CheckPOINT-----4");
                    StartCoroutine(ActiveStartButton());
                }



                string id = dicData["id"];
                Debug.Log("data_id : " + id + " data_nickname : " + dicData["nickname"] + " MY ID : " + GameManager.Instance.myPlayerData.GetPlayerId());


                if (!id.Equals(GameManager.Instance.myPlayerData.GetPlayerId()))
                {
                    if (dicData["avatar"] != null)
                    {
                        string[] str1 = dicData["avatar"].Split('/');
                        if (str1[0].Contains("https"))
                        {
                            StartCoroutine(loadOpponentImage(dicData["avatar"], i));
                            //if(i == 3)
                            //{
                            //    progressBar[2].SetActive(false);
                            //    opponentAvatarImage[2].SetActive(true);
                            //    opponentAvatar[2].GetComponent<Image>().sprite = GameManager.Instance.opponentsAvatars[2];
                            //}
                            //else
                            //{
                            //    progressBar[i].SetActive(false);
                            //    opponentAvatarImage[i].SetActive(true);
                            //    opponentAvatar[i].GetComponent<Image>().sprite = GameManager.Instance.opponentsAvatars[i];
                            //}
                        }
                        else
                        {
                            string[] str2 = str1[2].Split('.');
                            if (int.Parse(str2[0]) < 0 || int.Parse(str2[0]) > avatarSprites.Length - 1)
                            {                              
                                Debug.Log("------------------------------------------------ PLAYER IMAGE 1: " + int.Parse(str2[0]));                               
                                GameManager.Instance.opponentsAvatars.Add(avatarSprites[1]);
                            }
                            else
                            {
                                if (i == 3)
                                {
                                    GameManager.Instance.opponentsAvatars.Add(avatarSprites[int.Parse(str2[0])]);
                                    Debug.Log("------------------------------------------------ PLAYER IMAGE 2: " + int.Parse(str2[0]) + " : " + GameManager.Instance.opponentsAvatars[2].name);
                                }
                                else
                                {
                                    GameManager.Instance.opponentsAvatars.Add(avatarSprites[int.Parse(str2[0])] );
                                    Debug.Log("------------------------------------------------ PLAYER IMAGE 2: " + i + " : " + int.Parse(str2[0]));
                                }
                            }
                        }
                    }
                    else
                    {
                        progressBar[i].SetActive(false);
                        GameManager.Instance.opponentsAvatars.Add(avatarSprites[0] );
                    }
                    Debug.Log("------------------------------------------------ OPPONENTS NICKNAME : " + dicData["nickname"]);
                    if (i == 3)
                    {
                        GameManager.Instance.opponentsIDs.Add(id);
                        GameManager.Instance.opponentsNames.Add(dicData["nickname"]);
                    }
                    else
                    {
                        GameManager.Instance.opponentsIDs.Add(id);
                        GameManager.Instance.opponentsNames.Add(dicData["nickname"]);
                    }
                    GameManager.Instance.currentPlayersCount++;
                    GameManager.Instance.readyPlayersCount++;
                }
            }
			
			StartCoroutine(WaitForDownloadOppImage());
		}
	}
    IEnumerator ActiveStartButton()
    {
        yield return new WaitForSeconds(6.0f);
        startButton.GetComponent<Button>().interactable = true;
    }

    IEnumerator WaitForDownloadOppImage()
    {
        yield return new WaitForSeconds(5.0f);

        for (int j = 0; j < 3; j++)
        {
            if (GameManager.Instance.opponentsAvatars[j] == null)
                continue;

            print("Number : " + j + "----" + "nickname : " + GameManager.Instance.opponentsNames[j] + "--------" +
                "ID : " + GameManager.Instance.opponentsIDs[j] + "---------" + "Avatar" + j + " : " + GameManager.Instance.opponentsAvatars[j].name);

            progressBar[j].SetActive(false);
            opponentAvatarImage[j].SetActive(true);
            opponentAvatar[j].GetComponent<Image>().sprite = GameManager.Instance.opponentsAvatars[j];
        }
    }

   
	private bool FacebookAvtarCheck = false;



    public void OnGetOpponentResult(SocketIOEvent evt) //Two or Four Players Room (Get Opponenet Data -- Avatar, Nickname, ID)
    {
		FacebookAvtarCheck = false;
        Dictionary<string, string> data = new Dictionary<string, string>();

        Debug.Log("ARRAY LENGTH : " + evt.data["data"]);
        Debug.Log("ARRAY LENGTH : " + evt.data["data"].Count);

        JSONObject jsonData = evt.data["data"];

        for (int i = 0; i < jsonData.Count; i++)
        {
            data = jsonData[i].ToDictionary();
            string id = data["id"];
            Debug.Log("data_id------- : " + id + " data_nickname-------- : " + data["nickname"] + " avatar------ : " + data["avatar"] + " MY ID : " + GameManager.Instance.myPlayerData.GetPlayerId());
            if (!id.Equals(GameManager.Instance.myPlayerData.GetPlayerId()))
            {
                if (data["avatar"] != null)
                {
                    if (id.Contains("_BOT"))
                    {                        
                        //GameManager.Instance.opponentsAvatars[i] = avatarSprites[int.Parse(data["avatar"])];
                        GameManager.Instance.opponentsAvatars.Add(avatarSprites[int.Parse(data["avatar"])] );
                        return;//kki0516
                    }
                    else
                    {
                        string[] str1 = data["avatar"].Split('/');
                        if (str1[0].Contains("https"))
                        {
                            Debug.Log("FACEBOOK AVATAR : " + data["avatar"] + " : index : " + i);
							FacebookAvtarCheck = true;
                            StartCoroutine(loadOpponentImage(data["avatar"], i));
                        }
                        else
                        {
                            string[] str2 = str1[2].Split('.');
                            if (int.Parse(str2[0]) < 0 || int.Parse(str2[0]) > avatarSprites.Length - 1)
                            {
                                Debug.Log("------------------------------------------------ PLAYER IMAGE 1: " + int.Parse(str2[0]));
                                //GameManager.Instance.opponentsAvatars[i] = avatarSprites[0];
                                GameManager.Instance.opponentsAvatars.Add(avatarSprites[0]);
                            }
                            else
                            {
                                if (i == 3)
                                {                                    
                                    GameManager.Instance.opponentsAvatars[2] = avatarSprites[int.Parse(str2[0])];                                   
                                    Debug.Log("------------------------------------------------ AVATAR: " + int.Parse(str2[0]) + " : " + GameManager.Instance.opponentsAvatars[2].name);
                                }
                                else
                                {
                                    //GameManager.Instance.opponentsAvatars[i] = avatarSprites[int.Parse(str2[0])];
                                    GameManager.Instance.opponentsAvatars.Add(avatarSprites[int.Parse(str2[0])]);
                                    Debug.Log("------------------------------------------------ AVATAR: " + i + " : " + int.Parse(str2[0]));
                                }
                            }
                        }
                    }
                }
                else
                {
                    //GameManager.Instance.opponentsAvatars[i] = avatarSprites[0];
                    GameManager.Instance.opponentsAvatars.Add(avatarSprites[0]);
                }
                Debug.Log("------------------------------------------------ OPPONENTS NICKNAME : " + data["nickname"]);
                if (i == 3)
                {
                    GameManager.Instance.opponentsIDs[2] = id;
                    GameManager.Instance.opponentsNames[2] = data["nickname"];
                    Debug.Log("GameManager.Instance.opponentsIDs[i] : " + GameManager.Instance.opponentsIDs[2] + " data_id" + id);
                }
                else
                {
                    //GameManager.Instance.opponentsIDs[i] = id;
                    //GameManager.Instance.opponentsNames[i] = data["nickname"];
                    GameManager.Instance.opponentsIDs.Add(id);
                    GameManager.Instance.opponentsNames.Add(data["nickname"]);
                    Debug.Log("GameManager.Instance.opponentsIDs[i] : " + GameManager.Instance.opponentsIDs[i] + " data_id" + id);
                }
                GameManager.Instance.currentPlayersCount++;
                GameManager.Instance.readyPlayersCount++;
            }
        }

        print("AVATAR COUNT ---   " + GameManager.Instance.opponentsAvatars.Count);

        for (int i = 0; i < GameManager.Instance.opponentsAvatars.Count; i++)
        {
            if (GameManager.Instance.opponentsAvatars[i] != null)
                print("AVATAR NAME " + i + "----    " + GameManager.Instance.opponentsAvatars[i].name);
        }


        Debug.Log("------------------------------------------------2");

        if (FacebookAvtarCheck)
            Invoke("StartGame", 5.0f);
        else
            StartGame();

    }

    private void StartGame()
    {
        GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>().StartGame();
    }

    IEnumerator loadOpponentImage(string imageURL, int i)
    {
        print("ImageURL : " + imageURL + "index : " + i);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("LOADING FACE BOOK IMAGE");
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite temp = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 32);
            GameManager.Instance.opponentsAvatars.Add(temp);
        }
    }


    private void SetRoomInfo(Dictionary<string, string> data, int memberCount)
    {
        Dictionary<string, string> roomInfo = new Dictionary<string, string>();

        roomInfo.Add("type", "" + memberCount);
        roomInfo.Add("entryCoin", data["entryCoin"]);
        roomInfo.Add("winnerCoin", data["winningCoin"]);
        int adminCoin = int.Parse(data["entryCoin"]) * memberCount - int.Parse(data["winningCoin"]);



        GameManager.Instance.JoinedByID = false;
        GameManager.Instance.GameScene = "GameScene";
        if (memberCount == 4)
        {
            GameManager.Instance.requiredPlayers = 3;
            int secondCoin = adminCoin / 3 * 2;
            roomInfo.Add("secondWinnerCoin", "" + secondCoin);
            adminCoin -= secondCoin;

        }
        else
        {
            GameManager.Instance.requiredPlayers = 1;
        }

        roomInfo.Add("adminCoin", "" + adminCoin);
        GameManager.Instance.myPlayerData.room_data = roomInfo;
    }

    
    public void ContinueGame()
    {
        if (GameManager.Instance.type != MyGameType.Private)
        {
            Invoke("StartGameWithBots", StaticStrings.WaitTimeUntilStartWithBots);
        }
    }

    public void StartGameWithBots()
    {
        GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>().LoadBots();
    }

    public void OnEnterroomResult(SocketIOEvent evt)
    {
        Dictionary<string, string> data = evt.data.ToDictionary();
        for (int i = 1; i < content.transform.childCount; i++)
        {
           Destroy(content.transform.GetChild(i).gameObject);
        }

        if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.myPlayerData.GetEntryCoin())
        {
            if(GameManager.Instance.type == MyGameType.Private)
            {
                playerMatchWindow.SetActive(true);
                if(!roomIDText.activeSelf)
                {
                    roomIDText.SetActive(true);
                }
                roomIDText.GetComponent<Text>().text = data["roomIndex"];
                GameManager.Instance.privateRoomID = roomIDText.GetComponent<Text>().text;
                timeCountDown.SetActive(false);
                countDown.SetActive(false);
                if(!startButton.activeSelf)
                {
                    print("CheckPOINT --- 4");
                    startButton.SetActive(true);
                }
                startButton.GetComponent<Button>().interactable = false;
                myAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
                for (int i = 0; i < 3; i++)
                {
                    opponentAvatarImage[i].SetActive(false);
                    progressBar[i].SetActive(true);
                }
            }
            else if (GameManager.Instance.inviteFriendActivated)
            {
                GameManager.Instance.tableNumber = GameManager.Instance.myPlayerData.GetRoomId();
                GameManager.Instance.payoutCoins = GameManager.Instance.myPlayerData.GetEntryCoin();
                GameManager.Instance.initMenuScript.backToMenuFromTableSelect();
                //GameManager.Instance.playfabManager.challengeFriend(GameManager.Instance.challengedFriendID, "" + GameManager.Instance.myPlayerData.GetEntryCoin() + ";" + GameManager.Instance.myPlayerData.GetRoomId());

            }
            else if (GameManager.Instance.offlineMode)
            {
                GameManager.Instance.payoutCoins = GameManager.Instance.myPlayerData.GetEntryCoin();
                if (!GameManager.Instance.gameSceneStarted)
                {
                    SceneManager.LoadScene(GameManager.Instance.GameScene);
                    GameManager.Instance.gameSceneStarted = true;
                }
            }
            else
            {
                GameManager.Instance.tableNumber = GameManager.Instance.myPlayerData.GetRoomId();
                GameManager.Instance.payoutCoins = GameManager.Instance.myPlayerData.GetEntryCoin();
                GameManager.Instance.facebookManager.startRandomGame();
            }

        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }



    }

    public void UpdateDataSocket()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("id", GameManager.Instance.myPlayerData.GetPlayerId());
        JSONObject jsonData = new JSONObject(data);
        socketIO.Emit("USER_INFO_UPDATE", jsonData);
    }

    public void OnGetUserInfo(SocketIOEvent evt)
    {
        //Debug.Log("Get User Info Per Second");
        GameManager.Instance.myPlayerData.user_data = evt.data.ToDictionary();
        if (int.Parse(GameManager.Instance.myPlayerData.user_data["blockinfo"]) == 0)
        {
            blockWindow.SetActive(true);
        }
    }


    public void UpdatePlayerInfo()
    {
        string nickName = userName.GetComponent<InputField>().text;
        string userPassword = password.GetComponent<InputField>().text;
        string userEmail = emailId.GetComponent<InputField>().text;
        string userMobile = mobileNumber.GetComponent<InputField>().text;

        GameManager.Instance.myphoneNum = userMobile;

        if (!Regex.IsMatch(userEmail, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"))
        {
            invalidInput.SetActive(true);
        }
        else if (userPassword.Length < 6)
        {
            invalidInput.SetActive(true);
        }
        else if (nickName.Length <= 0)
        {
            invalidInput.SetActive(true);
        }
        else if (userMobile.Length < 12)
        {
            invalidInput.SetActive(true);
        }
        else
        {
            invalidInput.SetActive(false);
            playerInfoWindow.SetActive(false);
            GameManager.Instance.myPlayerData.user_data["nickname"] = nickName;
            PlayerPrefs.SetString("phone", userMobile);
            PlayerPrefs.SetString("password", userPassword);
            PlayerPrefs.SetString("email", userEmail);
            PlayerPrefs.SetString("nickname", nickName);
            playerName1.GetComponent<Text>().text = nickName;
            playerName2.GetComponent<Text>().text = nickName;
            Dictionary<string, string> sendData = new Dictionary<string, string>();
            sendData.Add("user_name", nickName);
            sendData.Add("password", userPassword);
            sendData.Add("email", userEmail);
            sendData.Add("phone_number", userMobile);
            sendData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
            JSONObject jsonData = new JSONObject(sendData);

            socketIO.Emit("UPDATE_USER_PROFILE", jsonData);
        }
    }
}
