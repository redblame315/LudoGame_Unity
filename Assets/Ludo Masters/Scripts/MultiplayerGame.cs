using UnityEngine;
using System.Collections;
using PlayFab;
//using PlayFab.ClientModels;


public class MultiplayerGame /*: Photon.PunBehaviour*/ {

//	private PhotonView myPhotonView;


//	void Awake() {
////		PhotonNetwork.OnEventCall += this.OnEvent;
//		PlayFabSettings.TitleId = "F8C3"; //your title id goes here.
//	}
//
//
//	// handle events:
//	private void OnEvent(byte eventcode, object content, int senderid)
//	{
//		if (eventcode == 0)
//		{
//			Debug.Log ("AA");
//			PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?
//			Debug.Log((string)content);
//
////			byte[] selected = (byte[])content;
////			foreach (byte unitId in selected)
////			{
////				// do something
////			}
//		}
//	}

//	void OnLoginSuccess(LoginResult result)
//	{
//		StartCoroutine(GetUserStats());
//		this.playfabId = result.PlayFabId;
//		GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest();
//		request.PhotonApplicationId = photonComponent.AppId.Trim();
//
//		// get an authentication ticket to pass on to Photon
//		PlayFabClientAPI.GetPhotonAuthenticationToken(request, OnPhotonAuthenticationSuccess, OnPlayFabError);
//	}

	// Use this for initialization
//	public void Start()
//	{
//		PhotonNetwork.ConnectUsingSettings("0.1");
//		PhotonNetwork.sendRate = 80;
//		PhotonNetwork.sendRateOnSerialize = 80;
//	}

//	public override void OnJoinedLobby()
//	{
//		Debug.Log("OnJoinedLobby");
////		PhotonNetwork.JoinRandomRoom();
//	}
//
//	public override void OnConnectedToMaster()
//	{
//		// when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
//		//PhotonNetwork.JoinRandomRoom();
//		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2 };
//		PhotonNetwork.JoinOrCreateRoom("debugRoom", roomOptions, TypedLobby.Default);
//
//	}
//
//	public void OnPhotonRandomJoinFailed()
//	{
//		PhotonNetwork.CreateRoom(null);
//	}
//
//
//	public override void OnJoinedRoom()
//	{
//		Debug.Log ("OnJoinedRoom");
//
//	}
//
//	public void OnGUI()
//	{
//		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
//	}


}
