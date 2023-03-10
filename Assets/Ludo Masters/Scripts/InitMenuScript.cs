using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Facebook.Unity;
using SocketIO;
//using ExitGames.Client.Photon.Chat;
using UnityEngine.SceneManagement;
//using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using UnityEngine.Networking;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Advertisements;
#endif
using AssemblyCSharp;


public class InitMenuScript : MonoBehaviour
{
    public GameObject rateWindow;
    public GameObject FacebookLinkReward;
    public GameObject rewardDialogText;
    public GameObject FacebookLinkButton;
    public GameObject playerName;
    public GameObject playerName2;
    public GameObject videoRewardText;
    public GameObject playerAvatar;
    public GameObject playerAvatar2;
    public GameObject fbFriendsMenu;
    public GameObject matchPlayer;
    public GameObject backButtonMatchPlayers;
    public GameObject MatchPlayersCanvas;
    public GameObject menuCanvas;
    public GameObject tablesCanvas;
    public GameObject gameTitle;
    public GameObject changeDialog;
    public GameObject inputNewName;
    public GameObject tooShortText;
    public GameObject coinsText;
    public GameObject coinsText2;
    public GameObject coinsTextShop;
    public GameObject coinsTab;
    public GameObject TheMillButton;
    public GameObject dialog;
    // Use this for initialization
    public GameObject GameConfigurationScreen;
    public GameObject FourPlayerMenuButton;
    private SocketIOComponent socketIO;
    private List<Dictionary<string, string>> roomData;

    public GameObject gameroom;
    public RectTransform Content;
    public GameObject facebookImage;
    public GameObject[] ratingStar;

    public GameObject MessageRefer;
    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
        GameManager.Instance.myphoneNum = PlayerPrefs.GetString("phone");
        print("GameManager.Instance.myphoneNum ==" + GameManager.Instance.myphoneNum);

        PlayerInfoUpdate.transform.Find("Window").Find("InputNameField").GetComponent<InputField>().text = PlayerPrefs.GetString("nickname");
        PlayerInfoUpdate.transform.Find("Window").Find("InputPasswordField").GetComponent<InputField>().text = PlayerPrefs.GetString("password");
        PlayerInfoUpdate.transform.Find("Window").Find("InputEmailField").GetComponent<InputField>().text = PlayerPrefs.GetString("email");
        PlayerInfoUpdate.transform.Find("Window").Find("InputMobileField").GetComponent<InputField>().text = PlayerPrefs.GetString("phone");


        if (PlayerPrefs.GetString("phone").Length < 12 || PlayerPrefs.GetString("password").Length == 0 || PlayerPrefs.GetString("email").Length == 0)
        {
            FBMsg.SetActive(true);
            return;
        }

        Dictionary<string, string> Referdata = new Dictionary<string, string>();
        print("refCode == " + GameManager.Instance.myReferCode);
        print("myPhone == " + GameManager.Instance.myphoneNum);
        Referdata.Add("referCode", GameManager.Instance.myReferCode);
        Referdata.Add("phone", GameManager.Instance.myphoneNum);
        JSONObject sendRefData = new JSONObject(Referdata);

        print("123123");
        socketIO.Emit("GET_REFERCOINS");
        socketIO.Emit("USER_REFERCHECK", sendRefData);        
    }

    public Text referCoinText;
    public void OnReferResult(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data = evt.data.ToDictionary();
        print("Log OnReferResult ---- " + int.Parse(data["states"]));
        if (int.Parse(data["states"]) == 1)
        {
            print("checkReferCode");
            GameManager.Instance.referCodeCheck = true;
            referCoinText.text = customRefercoin;
            MessageRefer.SetActive(true);
            PlayerPrefs.SetString("coins", customRefercoin);
            PlayerPrefs.Save();
            GameManager.Instance.myReferCode = "";
        }
    }

    string customRefercoin = "";
    public void OnReferCoins(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data = evt.data.ToDictionary();
        customRefercoin = data["freecoins"];
        GameManager.Instance.ReferCoins = customRefercoin;
        print("Refercoin = " + customRefercoin);

    }

    void Start()
    {
        socketIO.On("ROOMLIST_RESULT", OnRoomlistResult);
        socketIO.On("REFERCOINS_CHECK", OnReferCoins);
        socketIO.On("REFER_RESULT", OnReferResult);
        
        

        roomData = new List<Dictionary<string, string>>();

        if (PlayerPrefs.GetInt(StaticStrings.SoundsKey, 0) == 0)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }


        FacebookLinkReward.GetComponent<Text>().text = "+ " + StaticStrings.CoinsForLinkToFacebook;

        if (!StaticStrings.isFourPlayerModeEnabled)
        {
            FourPlayerMenuButton.SetActive(false);
        }

        GameManager.Instance.FacebookLinkButton = FacebookLinkButton;

        GameManager.Instance.dialog = dialog;
        videoRewardText.GetComponent<Text>().text = "+" + StaticStrings.rewardForVideoAd;
        GameManager.Instance.tablesCanvas = tablesCanvas;
        GameManager.Instance.facebookFriendsMenu = fbFriendsMenu.GetComponent<FacebookFriendsMenu>(); ;
        GameManager.Instance.matchPlayerObject = matchPlayer;
        GameManager.Instance.backButtonMatchPlayers = backButtonMatchPlayers;
        playerName.GetComponent<Text>().text = GameManager.Instance.myPlayerData.GetPlayerName();//GameManager.Instance.nameMy;
        playerName2.GetComponent<Text>().text = GameManager.Instance.myPlayerData.GetPlayerName();
        GameManager.Instance.MatchPlayersCanvas = MatchPlayersCanvas;

        if (PlayerPrefs.GetString("LoggedType").Equals("Facebook"))
        {
            FacebookLinkButton.SetActive(false);
        }

        playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
        playerAvatar2.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;


        print("avatar : " + PlayerPrefs.GetString("avatar"));
        if (PlayerPrefs.GetString("avatar").Contains("https:") /*GameManager.Instance.avatarMy == null && PlayerPrefs.GetString("LoggedType").Equals("Facebook")*/)
        {
            StartCoroutine(loadMyProfileImage(PlayerPrefs.GetString("avatar")));
        }





        GameManager.Instance.myAvatarGameObject = playerAvatar;
        GameManager.Instance.myNameGameObject = playerName;

        GameManager.Instance.coinsTextMenu = coinsText;
        GameManager.Instance.coinsTextShop = coinsTextShop;
        GameManager.Instance.initMenuScript = this;

        if (StaticStrings.hideCoinsTabInShop)
        {
            coinsTab.SetActive(false);
        }

#if UNITY_WEBGL
        coinsTab.SetActive(false);
#endif
        rewardDialogText.GetComponent<Text>().text = "1 Video = " + StaticStrings.rewardForVideoAd + " Coins";
        //coinsText.GetComponent<Text>().text = GameManager.Instance.myPlayerData.GetCoins() + "";
        coinsText.GetComponent<Text>().text = PlayerPrefs.GetString("coins");
        coinsText2.GetComponent<Text>().text = PlayerPrefs.GetString("coins");

        /*        Debug.Log("Load ad menu");*/
        //AdsManager.Instance.adsScript.ShowAd(AdLocation.GameStart);

        if (PlayerPrefs.GetInt("GamesPlayed", 1) % 8 == 0 && PlayerPrefs.GetInt("GameRated", 0) == 0)
        {
            rateWindow.SetActive(true);
            PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed", 1) + 1);
        }

    }

    IEnumerator loadMyProfileImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("LOADING PROFILE IMAGE");
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite temp = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 32);
            GameManager.Instance.avatarMy = temp;
            GameManager.Instance.facebookAvatar = temp;

            playerAvatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
            playerAvatar2.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;
        }
    }

    public void QuitApp()
    {
        PlayerPrefs.SetInt("GameRated", 5);
#if UNITY_ANDROID
        //Application.OpenURL("market://details?id=" + StaticStrings.AndroidPackageName);
#elif UNITY_IPHONE
        //Application.OpenURL("itms-apps://itunes.apple.com/app/id" + StaticStrings.ITunesAppID);
#endif
        //Application.Quit();
    }


    

    public void LinkToFacebook()
    {
        GameManager.Instance.facebookManager.FBLinkAccount();
    }

    public void ShowGameConfiguration(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.Instance.type = MyGameType.TwoPlayer;
                break;
            case 1:
                GameManager.Instance.type = MyGameType.FourPlayer;
                break;
            case 2:
                GameManager.Instance.type = MyGameType.Private;
                break;
        }
        roomData = new List<Dictionary<string, string>>();
        socketIO.Emit("GET_ROOMLIST");      
        

        //AdsManager.Instance.adsScript.ShowAd(AdLocation.GamePropertiesWindow);
    }

    public void OnRoomlistResult(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data = evt.data.ToDictionary();
        int gameType = 0;

        switch (GameManager.Instance.type)
        {
            case MyGameType.FourPlayer:
                gameType = 4;
                break;
            case MyGameType.TwoPlayer:
                gameType = 2;
                break;
            case MyGameType.Private:
                gameType = 1;
                break;
        }

        if (int.Parse(data["end"]) == 0)
        {
            roomData.Add(data);
        }
        else
        {
            GameConfigurationScreen.SetActive(true);
            for (int i = 0; i < roomData.Count; i++)
            {
                if (int.Parse(roomData[i]["id"]) == 0)
                {
                    gameroom.GetComponent<GameRoom>().onlineUser.text = "ONLINE USER " + roomData[i]["member"];
                    gameroom.GetComponent<GameRoom>().roomName.text = "DEFAULT ROOM";
                    int winnerCoin = 36;
                    gameroom.GetComponent<GameRoom>().winningCoin.text = "" + winnerCoin;
                    gameroom.GetComponent<GameRoom>().tableType = gameType;
                    gameroom.GetComponent<GameRoom>().winnerCoin = winnerCoin;
                    continue;
                }
                if (int.Parse(roomData[i]["matchtype"]) == gameType)
                {
                    GameObject obj = Instantiate(gameroom, Content);
                    obj.GetComponent<GameRoom>().tableNumber = int.Parse(roomData[i]["id"]);
                    obj.GetComponent<GameRoom>().fee = int.Parse(roomData[i]["coin"]);
                    obj.GetComponent<GameRoom>().entryCoin.text = "" + roomData[i]["coin"];
                    obj.GetComponent<GameRoom>().onlineUser.text = "ONLINE USER " + roomData[i]["member"];
                    obj.GetComponent<GameRoom>().roomName.text = roomData[i]["roomname"];
                    obj.GetComponent<GameRoom>().winningCoin.text = "" + roomData[i]["winning"];
                    obj.GetComponent<GameRoom>().winnerCoin = int.Parse(roomData[i]["winning"]);
                    obj.GetComponent<GameRoom>().tableType = gameType;

                    switch (GameManager.Instance.type)
                    {
                        case MyGameType.FourPlayer:
                            obj.GetComponent<GameRoom>().FourPlayerTitle.SetActive(true);
                            obj.GetComponent<GameRoom>().twoPlayerTitle.SetActive(false);
                            obj.GetComponent<GameRoom>().privatePlayerTitle.SetActive(false);
                            break;
                        case MyGameType.TwoPlayer:
                            obj.GetComponent<GameRoom>().twoPlayerTitle.SetActive(true);
                            obj.GetComponent<GameRoom>().FourPlayerTitle.SetActive(false);
                            obj.GetComponent<GameRoom>().privatePlayerTitle.SetActive(false);
                            break;
                        case MyGameType.Private:
                            obj.GetComponent<GameRoom>().privatePlayerTitle.SetActive(true);
                            obj.GetComponent<GameRoom>().twoPlayerTitle.SetActive(false);
                            obj.GetComponent<GameRoom>().FourPlayerTitle.SetActive(false);
                            break;
                    }
                }
                
            }
            
        }
        
    }


    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("TestScreenshot.png");
    }


    // Update is called once per frame
    void Update()
    {
    }

    public void showAdStore()
    {
      //  AdsManager.Instance.adsScript.ShowAd(AdLocation.StoreWindow);
    }

    public void backToMenuFromTableSelect()
    {
        GameManager.Instance.offlineMode = false;
        tablesCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        gameTitle.SetActive(true);
    }

    public void showSelectTableScene(bool challengeFriend)
    {
        if (!challengeFriend)
            GameManager.Instance.inviteFriendActivated = false;

       // AdsManager.Instance.adsScript.ShowAd(AdLocation.GameStart);
        if (GameManager.Instance.offlineMode)
        {
            TheMillButton.SetActive(false);
        }
        else
        {
            TheMillButton.SetActive(true);
        }
        menuCanvas.SetActive(false);
        tablesCanvas.SetActive(true);
        gameTitle.SetActive(false);
    }

    public void playOffline()
    {
        //GameManager.Instance.tableNumber = 0;
        GameManager.Instance.offlineMode = true;
        GameManager.Instance.roomOwner = true;
        showSelectTableScene(false);
        //SceneManager.LoadScene(GameManager.Instance.GameScene);
    }

    public void switchUser()
    {
        if (FB.IsLoggedIn)
            FB.LogOut();

        GameManager.Instance.playfabManager.destroy();
        GameManager.Instance.facebookManager.destroy();
        GameManager.Instance.connectionLost.destroy();
        GameManager.Instance.avatarMy = null;
        /*PhotonNetwork.Disconnect();*/

        PlayerPrefs.DeleteAll();
        GameManager.Instance.resetAllData();
        LocalNotification.ClearNotifications();
        //GameManager.Instance.myPlayerData.GetCoins() = 0;
        SceneManager.LoadScene("LoginSplash");
    }

    public void showChangeDialog()
    {
        changeDialog.SetActive(true);
    }

    public void changeUserName()
    {
        
        string newName = inputNewName.GetComponent<Text>().text;
        if (newName.Equals(StaticStrings.addCoinsHackString))
        {
            GameManager.Instance.playfabManager.addCoinsRequest(1000000);
            changeDialog.SetActive(false);
        }
        else
        {
            if (newName.Length > 0)
            {
                /*UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest()
                {
                    //DisplayName = newName
                    DisplayName = GameManager.Instance.playfabManager.PlayFabId
                };

                PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (response) =>
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("PlayerName", newName);
                    UpdateUserDataRequest userDataRequest = new UpdateUserDataRequest()
                    {
                        Data = data,
                        Permission = UserDataPermission.Public
                    };

                    PlayFabClientAPI.UpdateUserData(userDataRequest, (result1) =>
                    {
                        Debug.Log("Data updated successfull ");
                        Debug.Log("Title Display name updated successfully");
                        PlayerPrefs.SetString("GuestPlayerName", newName);
                        PlayerPrefs.Save();
                        GameManager.Instance.nameMy = newName;
                        playerName.GetComponent<Text>().text = newName;
                    }, (error1) =>
                    {
                        Debug.Log("Data updated error " + error1.ErrorMessage);
                    }, null);

                }, (error) =>
                {
                    Debug.Log("Title Display name updated error: " + error.Error);

                }, null);*/

                changeDialog.SetActive(false);
            }
            else
            {
                tooShortText.SetActive(true);
            }
        }



    }

    public void cancelChooseRoom()
    {
        roomData = new List<Dictionary<string, string>>();
    }

    public void startQuickGame()
    {
        GameManager.Instance.facebookManager.startRandomGame();
    }

    public void startQuickGameTableNumer(int tableNumer, int fee)
    {
        GameManager.Instance.payoutCoins = fee;
        GameManager.Instance.tableNumber = tableNumer;
        GameManager.Instance.facebookManager.startRandomGame();
    }

    public void showFacebookFriends()
    {

       // AdsManager.Instance.adsScript.ShowAd(AdLocation.FacebookFriends);
        GameManager.Instance.playfabManager.GetPlayfabFriends();
    }

    public void setTableNumber()
    {
        GameManager.Instance.tableNumber = Int32.Parse(GameObject.Find("TextTableNumber").GetComponent<Text>().text);
    }

    public GameObject FBMsg;
    public GameObject PlayerInfoUpdate;
    public void OnfacebookOk()
    {
        FBMsg.SetActive(false);
        PlayerInfoUpdate.SetActive(true);
    }


    public void shareApp()
    {
        //Application.OpenURL("https://play.google.com/store/apps/details?id=com.ludo.league");
    }

    public void ratingGame(int rating)
    {
        for(int i = 0; i < rating; i++)
        {
            ratingStar[i].SetActive(true);
        }

        for(int i = 4; i > rating - 1; i--)
        {
            ratingStar[i].SetActive(false);
        }
    }


    public void OnCloseReferMsg()
    {
        GameManager.Instance.referCodeCheck = false;
        MessageRefer.SetActive(false);
    }


    //    public void ShowRewardedAd()
    //    {
    //#if UNITY_ANDROID || UNITY_IOS
    //        if (Advertisement.IsReady("rewardedVideo"))
    //        {
    //            var options = new ShowOptions { resultCallback = HandleShowResult };
    //            Advertisement.Show("rewardedVideo", options);
    //        }
    //#endif
    //    }


    //#if UNITY_ANDROID || UNITY_IOS
    //    private void HandleShowResult(ShowResult result)
    //    {
    //        switch (result)
    //        {
    //            case ShowResult.Finished:
    //                Debug.Log("The ad was successfully shown.");
    //                GameManager.Instance.playfabManager.addCoinsRequest(StaticStrings.rewardForVideoAd);
    //                //
    //                // YOUR CODE TO REWARD THE GAMER
    //                // Give coins etc.
    //                break;
    //            case ShowResult.Skipped:
    //                Debug.Log("The ad was skipped before reaching the end.");
    //                break;
    //            case ShowResult.Failed:
    //                Debug.LogError("The ad failed to be shown.");
    //                break;
    //        }
    //    }
    //#endif

}
