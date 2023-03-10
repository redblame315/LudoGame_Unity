using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook;
using Facebook.MiniJSON;
using UnityEngine.UI;
using AssemblyCSharp;
using SocketIO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class FacebookManager : MonoBehaviour
{
    public GameObject facebookLoginButton;
    public GameObject guestLoginButton;
    private PlayFabManager playFabManager;
    public string name;
    public Sprite sprite;
    private GameObject FbLoginButton;
    private bool LoggedIn = false;
    private FacebookFriendsMenu facebookFriendsMenu;
    private bool alreadyGotFriends = false;
    public GameObject splashCanvas;
    public GameObject loginCanvas;
    public GameObject fbButton;
    public GameObject matchPlayersCanvas;
    public GameObject menuCanvas;
    public GameObject gameTitle;
    public GameObject idLoginDialog;
    public GameObject idRegisterDialog;
    public GameObject forgetPasswordDialog;


    public InputField loginEmail;
    public InputField loginPassword;
    public GameObject loginInvalidEmailorPassword;

    public InputField regiterEmail;
    public InputField registerPassword;
    public InputField registerNickname;
    public GameObject registerInvalidInput;

    public InputField resetPasswordEmail;
    public GameObject resetPasswordInformationText;
    public GameObject selectBtn;
    public SocketIOComponent socketIO;

    private StaticGameVariablesController staticController;

    public GameObject blockWindow;
    public Text blockText;






    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        /*        Debug.Log("FBManager awake");*/
        PlayerPrefs.DeleteAll();
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
        GameManager.Instance.facebookManager = this;
        DontDestroyOnLoad(transform.gameObject);

        staticController = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>();
        playFabManager = GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>();
        
        if (!GameManager.Instance.logged)
        {

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                		if (!FB.IsInitialized) {
                			// Initialize the Facebook SDK
                			FB.Init(InitCallback, OnHideUnity);
                		} else {
                			// Already initialized, signal an app activation App Event
                			FB.ActivateApp();
                            initSession();
                		}
#else
            initSession();
#endif

            //if (!FB.IsInitialized)
            //{
            //    // Initialize the Facebook SDK
            //    FB.Init(InitCallback, OnHideUnity);
            //}
            //else
            //{
            //    // Already initialized, signal an app activation App Event
            //    FB.ActivateApp();
            //    initSession();
            //}

            GameManager.Instance.logged = true;
        }
    }


    public void OnClickPlayerPrefDeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    void Start()
    {
        /*        Debug.Log("FBManager start");*/
        GameManager.Instance.facebookManager = this;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        FbLoginButton = GameObject.Find("FbLoginButton");

        facebookFriendsMenu = GameManager.Instance.facebookFriendsMenu; //.GetComponent <FacebookFriendsMenu> ();

        //socketIO.On("EMAIL_LOGIN_RESULT", OnEmailLoginResult);
        socketIO.On("FACEBOOK_REGISTER_RESULT", OnFaceBookLoginResult);
    }

    public void OnEmailLoginResult(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data = evt.data.ToDictionary();

        Debug.Log("EMAIL LOGIN RESULT ARRIVE : " + data);

        int result = int.Parse(data["result"]);

        if (result == 0)
        {
            blockText.text = data["message"];
            PlayerPrefs.DeleteAll();
            blockWindow.SetActive(true);
        }
        else
        {
            print("POINT --- 1");
            SceneManager.LoadScene("MenuScene");
        }
    }

    public void OnFaceBookLoginResult(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data = evt.data.ToDictionary();
        Debug.Log("FACEBOOK LOGIN RESULT ARRIVE : ");

        GameManager.Instance.myPlayerData.user_data = data;
        if (!PlayerPrefs.HasKey("id"))
        {
            PlayerPrefs.SetString("log", "login");
            PlayerPrefs.SetString("nickname", GameManager.Instance.myPlayerData.GetPlayerName());
            PlayerPrefs.SetString("id", GameManager.Instance.myPlayerData.GetPlayerId());
            PlayerPrefs.SetString("coins", data["coins"]);
            PlayerPrefs.SetString("avatar", data["avatar"]);
            PlayerPrefs.SetString("email", data["email"]);
            PlayerPrefs.SetString("blockinfo", data["blockinfo"]);
            PlayerPrefs.SetString("phone", data["phone"]);
            PlayerPrefs.SetString("password", data["password"]);
            PlayerPrefs.Save();
        }
        print("POINT ---2");
        SceneManager.LoadScene("MenuScene");

    }


    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            Debug.Log("INITIAL BACK");
            FB.ActivateApp();
            initSession();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void startRandomGame()
    {
        GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
        GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().setBackButton(true);
/*        Debug.Log("JoinRoom and start game");*/
        playFabManager.JoinRoomAndStartGame();
    }


    public void FBLogin()
    {
        var perms = new List<string>() { "public_profile", "email"/*, "user_friends" */};
        FB.LogInWithReadPermissions(perms, AuthCallback);
//         if (!LoggedIn)
//         {
//             var perms = new List<string>() { "public_profile", "email"/*, "user_friends" */};
//             FB.LogInWithReadPermissions(perms, AuthCallback);
//         }
//         else
//         {
//             playFabManager.JoinRoomAndStartGame();
//         }

    }

    public void FBLinkAccount()
    {
        GameManager.Instance.LinkFbAccount = true;
        FBLogin();
    }

    public void FBLoginWithoutLink()
    {
        GameManager.Instance.LinkFbAccount = false;
        FBLogin();
    }
    

    public void GuestLogin()
    {
        if (!LoggedIn)
        {
            playFabManager.Login();
        }
    }

    public void showRegisterDialog()
    {
        idLoginDialog.SetActive(false);
        idRegisterDialog.SetActive(true);
    }

    public void CloseLoginDialog()
    {
        //loginInvalidEmailorPassword.SetActive(false);
        loginEmail.text = "";
        loginPassword.text = "";
        //loginCanvas.SetActive(true);
        //idLoginDialog.SetActive(false);
    }

    public void CloseRegisterDialog()
    {
        regiterEmail.text = "";
        registerPassword.text = "";
        registerNickname.text = "";
        //registerInvalidInput.SetActive(false);
        //loginCanvas.SetActive(true);
        //idRegisterDialog.SetActive(false);
    }

    public void CloseForgetPasswordDialog()
    {
        resetPasswordEmail.text = "";
        resetPasswordInformationText.SetActive(false);
        forgetPasswordDialog.SetActive(false);
        loginCanvas.SetActive(true);
    }

    public void showForgetPasswordDialog()
    {
        forgetPasswordDialog.SetActive(true);
        idLoginDialog.SetActive(false);
    }

    public void IDLoginButtonPressed()
    {
        loginCanvas.SetActive(false);
        idLoginDialog.SetActive(true);
    }

    public void IDLogin()
    {
        if (!LoggedIn)
        {
            var perms = new List<string>() { "public_profile", "email"/*, "user_friends" */};
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID

            GameManager.Instance.facebookIDMy = aToken.UserId;
            // Print current access token's granted permissions
            Debug.Log(aToken.ToJson());
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }

            //			getFacebookInvitableFriends ();
            //			getFacebookFriends ();
            //			callApiToGetName ();
            //			getMyProfilePicture (GameManager.Instance.facebookIDMy);
            //
            //
            //			LoggedIn = true;
            //
            //			GameObject.Find ("FbLoginButtonText").GetComponent <Text>().text = "Play";

            PlayerPrefs.SetString("LoggedType", "Facebook");
            PlayerPrefs.Save();

            if (!GameManager.Instance.LinkFbAccount)
            {
                selectBtn.SetActive(false);
                splashCanvas.SetActive(true);

            }
            Debug.Log("AUTHCALLBACK");
            initSession();
        }
        else
        {
            //             facebookLoginButton.GetComponent<Button>().interactable = true;
            //             guestLoginButton.GetComponent<Button>().interactable = true;
            selectBtn.SetActive(true);
            Debug.Log("User cancelled login");
        }
    }

    private void initSession()
    {
        /*        Debug.Log("FbManager init session");*/
        if (!PlayerPrefs.HasKey("LoggedType"))
        {
            return;
        }
        
        string logType = PlayerPrefs.GetString("LoggedType");
        Debug.Log("LOGGED TYPE : " + logType);
        if (logType.Equals("Facebook"))
        {
            GameManager.Instance.facebookIDMy = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
            callApiToGetName();
            //callApiToID();
            getMyProfilePicture(GameManager.Instance.facebookIDMy);
            callAPIToGetEmail();

            Invoke("sendFaceBookInfo", 5.0f);

        }
        else
        {
            Invoke("StartMenuScene", 5.0f);
        }

    }
    
    private void StartMenuScene()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("nickname", PlayerPrefs.GetString("nickname"));
        data.Add("coins", PlayerPrefs.GetString("coins"));
        data.Add("avatar", PlayerPrefs.GetString("avatar"));
        data.Add("blockinfo", PlayerPrefs.GetString("blockinfo"));
        data.Add("id", PlayerPrefs.GetString("id"));

        print("AUTO LOGIN PLAYER ID : " + PlayerPrefs.GetString("id"));
        if (data["avatar"].Contains("https:") == false)
            GameManager.Instance.avatarMy = staticController.avatars[int.Parse(data["avatar"])];

        GameManager.Instance.myPlayerData.user_data = data;
        JSONObject jsonEmailData = new JSONObject(data);
        socketIO.Emit("EMAIL_LOG_IN", jsonEmailData);

        print("POINT --- 3");
        SceneManager.LoadScene("MenuScene");
    }

    private void sendFaceBookInfo()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data.Add("nickname", GameManager.Instance.nameMy);
        data.Add("avatar", GameManager.Instance.avatarMyUrl);
        data.Add("email", GameManager.Instance.emailMy);
        data.Add("fb_id", GameManager.Instance.facebookIDMy);

        Debug.Log("FACE BOOK LOGIN INFO !!!!!!!!");
        Debug.Log("FACEBOOK USER INFORMATION : " + data);
        JSONObject jsonData = new JSONObject(data);
        LoggedIn = true;

        socketIO.Emit("FACEBOOK_LOG_IN", jsonData);
    }

    private void callApiToGetName()
    {
        FB.API("me?fields=name", Facebook.Unity.HttpMethod.GET, APICallbackName);
    }

    //private void callApiToID()
    //{
    //    FB.API("me?fields=id", Facebook.Unity.HttpMethod.GET, APICallbackID);
    //}

    //void APICallbackID(IResult response)
    //{
    //    GameManager.Instance.myID = response.ResultDictionary["id"].ToString();

    //    Debug.Log("My id " + GameManager.Instance.myID);
    //}

    void APICallbackName(IResult response)
    {
        GameManager.Instance.nameMy = response.ResultDictionary["name"].ToString();
        
        Debug.Log("My name " + GameManager.Instance.nameMy);
    }


    private void callAPIToGetEmail()
    {
        FB.API("me?fields=email", Facebook.Unity.HttpMethod.GET, APICallBackEmail);
    }

    void APICallBackEmail(IResult response)
    {
        GameManager.Instance.emailMy = response.ResultDictionary["email"].ToString();

        Debug.Log("My email " + GameManager.Instance.emailMy);
    }

    private void getMyFacebookID()
    {
        string temp = GameManager.Instance.avatarMyUrl.Remove(GameManager.Instance.avatarMyUrl.IndexOf("https:"), GameManager.Instance.avatarMyUrl.IndexOf("id=") + 3);
        temp = temp.Remove(temp.IndexOf("&height"));
        print("MY FACEBOOK ID :  " + temp);
        GameManager.Instance.facebookIDMy = temp;
    }


    public void getMyProfilePicture(string userID)
    {

        //FB.API("/" + userID + "/picture?type=square&height=92&width=92", Facebook.Unity.HttpMethod.GET, delegate(IGraphResult result) {
        FB.API("/me?fields=picture.width(200).height(200)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.Error == null)
            {

                // use texture

                Dictionary<string, object> reqResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

                if (reqResult == null) Debug.Log("JEST NULL"); else Debug.Log("nie null");


                GameManager.Instance.avatarMyUrl = ((reqResult["picture"] as Dictionary<string, object>)["data"] as Dictionary<string, object>)["url"] as string;
                Debug.Log("My avatar " + GameManager.Instance.avatarMyUrl);
                StartCoroutine(loadImageMy(GameManager.Instance.avatarMyUrl));

                getMyFacebookID();
            }
            else
            {
                Debug.Log("Error retreiving image: " + result.Error);
            }
        });
    }

    IEnumerator loadImageMy(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
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
            GameManager.Instance.avatarMy = temp;
            GameManager.Instance.facebookAvatar = temp;
        }
    }

    public void getOpponentProfilePicture(string userID)
    {

        FB.API("/" + userID + "/picture?type=square&height=92&width=92", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.Texture != null)
            {
                // use texture

                GameManager.Instance.avatarMy = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32);

                playFabManager.LoginWithFacebook();
            }
        });
    }





    public void getFacebookInvitableFriends()
    {
        if (alreadyGotFriends)
        {
            facebookFriendsMenu.showFriends();
        }
        else
        {
            //&fields=picture.width(100).height(100)
            FB.API("/me/invitable_friends?limit=5000&fields=id,name,picture.width(100).height(100)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
            {

                if (result.Error == null)
                {
                    List<string> friendsNames = new List<string>();
                    List<string> friendsIDs = new List<string>();
                    List<string> friendsAvatars = new List<string>();
                    //Grab all requests in the form of a dictionary. 

                    Dictionary<string, object> reqResult = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
                    //Grab 'data' and put it in a list of objects. 


                    List<object> newObj = reqResult["data"] as List<object>;
                    //For every item in newObj is a separate request, so iterate on each of them separately. 


                    Debug.Log("Friends Count: " + newObj.Count);
                    for (int xx = 0; xx < newObj.Count; xx++)
                    {
                        Dictionary<string, object> reqConvert = newObj[xx] as Dictionary<string, object>;

                        string name = reqConvert["name"] as string;
                        string id = reqConvert["id"] as string;

                        //friendsNames.Add (name);
                        //friendsIDs.Add (id);

                        Dictionary<string, object> avatarDict = reqConvert["picture"] as Dictionary<string, object>;
                        avatarDict = avatarDict["data"] as Dictionary<string, object>;

                        string avatarUrl = avatarDict["url"] as string;
                        //friendsAvatars.Add (avatarUrl);
                        //Debug.Log("URL: " + avatarUrl);
                        GameManager.Instance.facebookFriendsMenu.AddFacebookFriend(name, id, avatarUrl);
                    }

                    //					alreadyGotFriends = true;
                    //					facebookFriendsMenu.showFriends (friendsNames, friendsIDs, friendsAvatars);



                }
                else
                {
                    Debug.Log("Something went wrong. " + result.Error + "  " + result.ToString());
                }

            });

        }
    }


    public void destroy()
    {
        if (this.gameObject != null)
            DestroyImmediate(this.gameObject);
    }

    public void showLoadingCanvas()
    {
        loginCanvas.SetActive(false);
        splashCanvas.SetActive(true);
    }



}
