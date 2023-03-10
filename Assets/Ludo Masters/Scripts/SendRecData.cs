using System.Collections;
using System.Collections.Generic;
using PlayFab;
/*using PlayFab.ClientModels;*/
using AssemblyCSharp;
using System;
using UnityEngine;
using System.Globalization;
using LitJson;
 
 public class SendRecData : MonoBehaviour
{

    void Start()
    {

        //string url = "http://varchartech.com/ludodata/ludoserver.php";
		//string url =  "http://192.168.1.223:81/projects/ludo_master/mob/add_user";
		//Debug.Log("Player coins here---"+GameManager.Instance.myPlayerData.GetCoins());
		//Debug.Log("Player total earning here---"+GameManager.Instance.myPlayerData.GetTotalEarnings());
		//Debug.Log("Player total played games here---"+GameManager.Instance.myPlayerData.GetPlayedGamesCount());
		//Debug.Log("Player avatarindex here---"+GameManager.Instance.myPlayerData.GetAvatarIndex());
/*		Debug.Log("Player name here---"+GameManager.Instance.myPlayerData.GetPlayerName());*/
		///Debug.Log("Player data here---"+GameManager.Instance.myPlayerData.data);

		//string url = StaticStrings.ApacheUrl+"add_user";
        //
		//string userName = GameManager.Instance.myPlayerData.GetPlayerName();
		//string userId = GameManager.Instance.myPlayerData.GetPlayerName();
		//string appId = SystemInfo.deviceUniqueIdentifier;
		//string fbId = GameManager.Instance.myPlayerData.GetFbId();
        //
		//string coins = GameManager.Instance.myPlayerData.GetCoins().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
		//string earnings = GameManager.Instance.myPlayerData.GetTotalEarnings().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
		//int totalGamePlayed = GameManager.Instance.myPlayerData.GetPlayedGamesCount();
		//string avatarIndex = GameManager.Instance.myPlayerData.GetAvatarIndex();
		//string playFabId = GameManager.Instance.playfabManager.PlayFabId;
        //
        //
        //
		//Debug.Log("Server file called." + playFabId);
        //
        //WWWForm form = new WWWForm();
		//form.AddField("playerName", userName);
		//form.AddField("playerId", userId);
		//form.AddField("appId", appId);
		//form.AddField("fbId", fbId);
		//form.AddField("totalCoins", coins);
		//form.AddField("earning", earnings);
		//form.AddField("totalGamePlayed", totalGamePlayed);
		//form.AddField("avatarIndex", avatarIndex);
		//form.AddField ("playFabId", playFabId);
        //
		//Debug.Log("url: " + url);
        //
        //WWW www = new WWW(url, form);
        //
        //StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

         // check for errors
        if (www.error == null)
        {
           Debug.Log("WWW Ok!-: " + www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

	public void getGameHistory(){
		string url = "http://192.168.1.222/projects/codeigniter/ci/index.php/basicdata/game2p";
		WWW www = new WWW(url);

		StartCoroutine(WaitForRequestt(www));
	}

	IEnumerator WaitForRequestt(WWW www)
	{
		yield return www;

		if (www.error == null)
		{
			Processjson(www.text);
		}
		else
		{
			Debug.Log("ERROR: " + www.error);
		}
	}

	private void Processjson(string jsonString)
	{
		//Debug.Log ("Server Data:- " + jsonString);
		try{
			JsonData jsonvale = JsonMapper.ToObject(jsonString);
			for(int i = 0; i<jsonvale.Count; i++)
			{
				Debug.Log (i + "--" + jsonvale[i] + " -- " + jsonvale[i].Count);
				for (int j = 0; j < jsonvale[i].Count; j++) {
					//Debug.Log ("Second loop:"+ j + ": " + jsonvale[i][j]);

				}
				Debug.Log ("Second loop:"+ "" + ": " + jsonvale[i]["game_type"]);
				Debug.Log ("Second loop:"+ "" + ": " + jsonvale[i]["game_money"]);
			} 
		}catch(Exception e){
			Debug.Log ("Json error:");
			Debug.Log ("Ecxeption: " + e);
			//Debug.LogException (e, this);
		}

		//parseJSON parsejson;
		//parsejson = new parseJSON();
	}
}
