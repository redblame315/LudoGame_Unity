using UnityEngine;
using System.Collections;
/*using ExitGames.Client.Photon.Chat;*/
/*using ExitGames.Client.Photon;*/
using UnityEngine.UI;
//using PlayFab.ClientModels;
using PlayFab;

public class PhotonChatListener2 : MonoBehaviour
{

    private Animator animator;
    public Text text;
    private string senderID;
    private string roomName;
    // "invited"
    // "accepted"
    public string type;
    public GameObject okButton;
    public GameObject rejectButton;
    public GameObject acceptButton;
    public GameObject matchPlayersCanvas;
    public GameObject friendsCanvas;
    public GameObject menuCanvas;
    public GameObject gameTitle;
    public GameObject addedFriendWindow;

    private string friendID;
    // Use this for initialization
    void Start()
    {
        GameManager.Instance.invitationDialog = this.gameObject;
        animator = GetComponent<Animator>();

    }

    //type
    // 0 - invite received
    // 1 - invite rejected
    // 2 - invite accepted
    // 3 - start game
    public void showInvitationDialog(string name, string id, string room)
    {

        friendID = name;

        rejectButton.SetActive(true);
        acceptButton.SetActive(true);


        this.type = "invited";
        senderID = id;
        roomName = room;

        text.text = id + " want to add you to Friends";
        animator.Play("AddFriendAnimation");

    }



    public void accept()
    {
        //AddFriendRequest request = new AddFriendRequest()
        //{
        //    FriendPlayFabId = friendID
        //};
        //
        //PlayFabClientAPI.AddFriend(request, (result) =>
        //{
        //    addedFriendWindow.SetActive(true);
        //    Debug.Log("Added friend successfully");
        //
        //}, (error) =>
        //{
        //    addedFriendWindow.SetActive(true);
        //    Debug.Log("Error adding friend: " + error.Error);
        //}, null);

        animator.Play("InvitationDialogHide");
    }

    public void hideDialog(string a)
    {

        // 		if (type.Equals ("invited")) {
        // 			if (a.Equals ("accepted")) {
        // 				GameManager.Instance.chatClient.SendPrivateMessage (senderID, "INVITE_ACCEPT;" + roomName + ";" + GameManager.Instance.nameMy);
        // 				RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2 };


        // 				PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        // //				menuCanvas.SetActive (false);
        // //				gameTitle.SetActive (false);


        // 				matchPlayersCanvas.GetComponent <SetMyData> ().MatchPlayer ();
        // 				matchPlayersCanvas.GetComponent <SetMyData> ().setBackButton (false);
        // //				friendsCanvas.SetActive (false);
        // //				menuCanvas.SetActive (false);
        // //				gameTitle.SetActive (false);

        // 			} else if (a.Equals ("rejected")) {
        // 				GameManager.Instance.chatClient.SendPrivateMessage (senderID, "INVITE_REJECT;" + roomName + ";" + GameManager.Instance.nameMy);
        // 			}
        // 		} else if (type.Equals ("accepted")) {
        // 			if (a.Equals ("accepted")) {
        // 				GameManager.Instance.chatClient.SendPrivateMessage (senderID, "INVITE_START;" + roomName + ";" + GameManager.Instance.nameMy);
        // 				matchPlayersCanvas.GetComponent <SetMyData> ().MatchPlayer ();
        // 				matchPlayersCanvas.GetComponent <SetMyData> ().setBackButton (false);
        // //				friendsCanvas.SetActive (false);
        // //				menuCanvas.SetActive (false);
        // //				gameTitle.SetActive (false);
        // 				PhotonNetwork.JoinRoom (roomName);

        // 			} else if (a.Equals ("rejected")) {
        // 				GameManager.Instance.chatClient.SendPrivateMessage (senderID, "INVITE_STOP;" + roomName + ";" + GameManager.Instance.nameMy);

        // 			}
        // 		}
        //Debug.Log ("Dialog: " + a);
        animator.Play("InvitationDialogHide");
    }

}
