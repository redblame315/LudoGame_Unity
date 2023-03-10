using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine.UI;
//using PlayFab.ClientModels;
using PlayFab;
//using ExitGames.Client.Photon.Chat;
using AssemblyCSharp;

public class FacebookFriendsMenu : MonoBehaviour
{

    public GameObject list;
    public GameObject friendPrefab;
    public GameObject friendPrefab2;
    public GameObject friendsMenu;
    public GameObject mainMenu;
    public InputField filterInputField;
    private PlayFabManager playFabManager;
    public GameObject confirmDialog;
    public GameObject confirmDialogText;
    public GameObject confirmDialogButton;

    private List<GameObject> friendsObjects = new List<GameObject>();
    private Sprite[] playersAvatars;
    // Use this for initialization
    void Start()
    {

        //		showFriendsTest ();
        playFabManager = GameManager.Instance.playfabManager;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateName(int i, string text, string id)
    {
        Debug.Log(i + " -- " + friendsObjects.Count);
        if (friendsObjects != null && friendsObjects.Count > 0 && i <= friendsObjects.Count - 1 && friendsObjects[i] != null)
        {
            friendsObjects[i].SetActive(true);
            friendsObjects[i].transform.Find("FriendName").GetComponent<Text>().text = text;
            // friendsObjects[i].transform.Find("FriendAvatar").gameObject.SetActive(true);
            // getFriendImageUrl(id, friendsObjects[i].transform.Find("FriendAvatar").GetComponent<Image>(), friendsObjects[i].transform.Find("FriendAvatar"));
        }

    }

    public void addPlayFabFriends(List<string> playfabIDs, List<string> playfabFBName, List<string> playfabFBID)
    {
        playersAvatars = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().avatars;

        friendsObjects = new List<GameObject>();


        friendsMenu.gameObject.SetActive(true);
        //mainMenu.gameObject.SetActive(false);



        for (int i = 0; i < playfabIDs.Count; i++)
        {


            GameObject friend = Instantiate(friendPrefab2, Vector3.zero, Quaternion.identity) as GameObject;
            string name = playfabFBName[i];
            if (playfabFBName[i].Length > 13)
            {
                name = playfabFBName[i].Substring(0, 12) + "...";
            }
            friend.transform.Find("FriendName").GetComponent<Text>().text = name;
            //friend.transform.Find("FriendName").GetComponent<Text>().text = playfabFBName[i];

            string friendName = playfabFBName[i];

            string friendID = playfabIDs[i];



            friend.GetComponent<PlayFabFriendScript>().playfabID = friendID;
            Debug.Log("ADD LISTENER");
            friend.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.RemoveAllListeners();
            friend.transform.Find("DeleteFriend").GetComponent<Button>().onClick.RemoveAllListeners();
            friend.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.AddListener(() => ChallengeFriend(friendID));
            friend.transform.Find("DeleteFriend").GetComponent<Button>().onClick.AddListener(() => RemoveFriend(friendID, friendName, friend));
            //
            //			friend.GetComponent<MonoBehaviour> ().StartCoroutine (
            //			if(!playfabFBID[i].Equals ("NoID"))
            //				loadImageFBID (playfabFBID [i], friend.transform.Find ("FriendAvatar").GetComponent <Image> ());
            //			);

            getFriendImageUrl(friendID, friend.transform.Find("Avatar/FriendAvatar").GetComponent<Image>(), friend.transform.Find("Avatar/FriendAvatar").gameObject);

            friend.transform.parent = list.transform;
            friend.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

            friendsObjects.Add(friend);

            if (playfabFBName[i].Length < 1)
            {
                friendsObjects[i].SetActive(false);
            }

        }
    }


    public void updateFriendStatus(int status, string id)
    {
        foreach (GameObject friend in friendsObjects)
        {
            if (friend.GetComponent<PlayFabFriendScript>().playfabID.Equals(id))
            {
                /*if (status == ChatUserStatus.Online)
                {*/
                    friend.GetComponent<PlayFabFriendScript>().statusIndicatorText.GetComponent<Text>().text = "Online";
                    friend.GetComponent<PlayFabFriendScript>().statusIndicator.GetComponent<Image>().color = Color.green;
                /*}
                else if (status == ChatUserStatus.Offline)
                {
                    friend.GetComponent<PlayFabFriendScript>().statusIndicatorText.GetComponent<Text>().text = "Offline";
                    friend.GetComponent<PlayFabFriendScript>().statusIndicator.GetComponent<Image>().color = Color.red;
                }*/
            }
        }
    }

    public void getFriendImageUrl(string id, Image image, GameObject imobject)
    {

        //GetUserDataRequest getdatarequest = new GetUserDataRequest()
        //{
        //    PlayFabId = id,
        //};
        //
        //PlayFabClientAPI.GetUserData(getdatarequest, (result) =>
        //{
        //
        //    Dictionary<string, UserDataRecord> data = result.Data;
        //    imobject.SetActive(true);
        //    if (data[MyPlayerData.AvatarIndexKey].Value.Equals("fb"))
        //    {
        //        if (data.ContainsKey("PlayerAvatarUrl"))
        //        {
        //            // image.GetComponent<MonoBehaviour>().StartCoroutine(loadImage(data["PlayerAvatarUrl"].Value, image));
        //            filterInputField.GetComponent<MonoBehaviour>().StartCoroutine(loadImage(data["PlayerAvatarUrl"].Value, image));
        //        }
        //    }
        //    else
        //    {
        //        if (playersAvatars == null) Debug.Log("NULLLLL");
        //        image.sprite = playersAvatars[int.Parse(data[MyPlayerData.AvatarIndexKey].Value)];
        //    }
        //
        //
        //}, (error) =>
        //{
        //    Debug.Log("Data updated error " + error.ErrorMessage);
        //}, null);
    }

    public void showFriends()
    {
        //		foreach (GameObject o in fbFriendsObjects) {
        //
        //		}
        //		friendsMenu.gameObject.SetActive (true);
        //		mainMenu.gameObject.SetActive (false);
        //		GameObject.Find ("FriendsMask").GetComponent <ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }

    public void showFriends(List<string> friendsNames, List<string> friendsIDs, List<string> friendsAvatars)
    {


        friendsMenu.gameObject.SetActive(true);
        //mainMenu.gameObject.SetActive(false);



        if (friendsNames != null)
        {
            for (int i = 0; i < friendsNames.Count; i++)
            {


                GameObject friend = Instantiate(friendPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                string name = friendsNames[i];
                if (friendsNames[i].Length > 13)
                {
                    name = friendsNames[i].Substring(0, 12) + "...";
                }
                // friend.transform.Find("FriendName").GetComponent<Text>().text = friendsNames[i];
                friend.transform.Find("FriendName").GetComponent<Text>().text = name;

                string friendID = friendsIDs[i];






                friend.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.RemoveAllListeners();
                friend.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.AddListener(() => InviteFriend(friendID));



                friend.GetComponent<MonoBehaviour>().StartCoroutine(loadImage(friendsAvatars[i], friend.transform.Find("Avatar/FriendAvatar").GetComponent<Image>()));


                friend.transform.parent = list.transform;
                friend.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

                friendsObjects.Add(friend);
                Debug.Log("KUPA");
                for (int j = 0; j < GameManager.Instance.friendsStatuses.Count; j++)
                {

                    string[] friend1 = GameManager.Instance.friendsStatuses[j];
                    Debug.Log(friendID + "  " + friend1[0]);
                    if (friend1[0].Equals(friendID))
                    {
                        Debug.Log("Found FRIEND");
                        /*if (friend1[1].Equals("" + ChatUserStatus.Online))
                            GameManager.Instance.facebookFriendsMenu.updateFriendStatus(ChatUserStatus.Online, friendID);*/
                        break;
                    }
                }

            }
        }


        // GameObject.Find("FriendsMask").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        // GameObject.Find("FriendsMask").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

    }

    public void AddFacebookFriend(string friendsNames, string friendsIDs, string friendsAvatars)
    {


        //		friendsMenu.gameObject.SetActive (true);
        //		mainMenu.gameObject.SetActive (false);

        if (friendsNames != null)
        {



            GameObject friend = Instantiate(friendPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            friend.transform.parent = list.transform;

            string name = friendsNames;
            if (friendsNames.Length > 13)
            {
                name = friendsNames.Substring(0, 12) + "...";
            }
            friend.transform.Find("FriendName").GetComponent<Text>().text = name;

            //friend.transform.Find("FriendName").GetComponent<Text>().text = friendsNames;

            string friendID = friendsIDs;
            string friendName = friendsNames;
            friend.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.RemoveAllListeners();
            friend.transform.Find("InviteFriendButton").GetComponent<Button>().onClick.AddListener(() => InviteFriend(friendID));





            friend.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

            friendsObjects.Add(friend);
            friend.GetComponent<MonoBehaviour>().StartCoroutine(loadImage(friendsAvatars, friend.transform.Find("Avatar/FriendAvatar").GetComponent<Image>()));


        }
        // GameObject.Find("FriendsMask").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        //GameObject.Find("FriendsMask").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

    }

    public void RemoveFriend(string id, string name, GameObject friend)
    {
        Debug.Log("click");
        confirmDialog.SetActive(true);
        confirmDialogText.GetComponent<Text>().text = "Remove " + friend.transform.Find("FriendName").GetComponent<Text>().text + " from your friends?";
        string friendID = id;
        confirmDialogButton.GetComponent<Button>().onClick.RemoveAllListeners();
        confirmDialogButton.GetComponent<Button>().onClick.AddListener(() => removeFriendRequest(friendID, friend));
    }

    public void removeFriendRequest(string id, GameObject friend)
    {
        Debug.Log("REMOVE CLICK");

        //RemoveFriendRequest request = new RemoveFriendRequest()
        //{
        //    FriendPlayFabId = id
        //};
        //
        //PlayFabClientAPI.RemoveFriend(request, (result) =>
        //{
        //    Debug.Log("Removed friend successfully");
        //    friend.SetActive(false);
        //}, (error) =>
        //{
        //    Debug.Log("Error removing friend: " + error.Error);
        //}, null);
    }

    public void hideFriends()
    {
        filterInputField.text = "";

        foreach (GameObject o in friendsObjects)
        {
            Destroy(o);
        }

        friendsMenu.gameObject.SetActive(false);
        //mainMenu.gameObject.SetActive(true);
    }


    public void FilterFriends()
    {
        string search = filterInputField.text;
        for (int i = 0; i < friendsObjects.Count; i++)
        {
            if (friendsObjects[i].transform.Find("FriendName").GetComponent<Text>().text.Length > 0)
                friendsObjects[i].SetActive(true);
            if (!friendsObjects[i].transform.Find("FriendName").GetComponent<Text>().text.ToLower().Contains(search.ToLower()))
            {
                friendsObjects[i].SetActive(false);

            }
        }
        // GameObject.Find("FriendsMask").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        //GameObject.Find("FriendsMask").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
        //Debug.Log (filterInputField.text);
    }

    public void InviteFriend(string i)
    {
        Debug.Log("" + i);
        List<string> to = new List<string>();
        to.Add(i);
        FB.AppRequest(
            StaticStrings.facebookInviteMessage, to, null, null, null, null, null,
            delegate (IAppRequestResult result)
            {
                Debug.Log("RESULT: " + "Cancelled - " + result.Cancelled);
                if (!result.Cancelled && (result.Error == null || (result.Error != null && result.Error.Equals(""))))
                {
                    GameManager.Instance.playfabManager.addCoinsRequest(StaticStrings.rewardCoinsForFriendInvite);
                }
                Debug.Log("REQUEST RESULT: " + result.RawResult);
            }
        );

    }

    public void ChallengeFriend(string id)
    {
        Debug.Log("Challenge friend: " + id);
        GameManager.Instance.facebookFriendsMenu.hideFriends();
        GameManager.Instance.playfabManager.challengeFriend(id, GameManager.Instance.payoutCoins + ";" + GameManager.Instance.privateRoomID);
    }

    public void loadImageFBID(string userID, Image image)
    {

        FB.API("/" + userID + "/picture?type=square&height=92&width=92", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result)
        {
            if (result.Texture != null)
            {
                // use texture
                image.sprite = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32);

                //				playFabManager.LoginWithFacebook ();
            }
        });
    }

    public IEnumerator loadImage(string url, Image image)
    {
        // Load avatar image

        // Start a download of the given URL
        WWW www = new WWW(url);

        // Wait for download to complete
        yield return www;


        image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);

    }

    //	public void showFriends() { 
    //		mainMenu.gameObject.SetActive (false);
    //		friendsMenu.gameObject.SetActive (true);
    //
    //		for (int i=0; i<50; i++) {
    //			GameObject friend = Instantiate(friendPrefab, Vector3.zero ,Quaternion.identity) as GameObject;
    //			friend.transform.Find ("FriendName").GetComponent <Text>().text = "Name";
    //
    //			friend.transform.parent = list.transform;
    //			friend.GetComponent <RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
    //
    //		}
    //
    //	}
}
