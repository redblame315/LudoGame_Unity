using System.Collections;
using System.Collections.Generic;
using PlayFab;
//using PlayFab.ClientModels;
using AssemblyCSharp;
using System;
using UnityEngine;

public class ExitGame : MonoBehaviour {

    private bool isQuiting;
    public GameObject quitMessageGameObject;

	//public Dictionary<string, UserDataRecord> data;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isQuiting == true)
        {
            if(Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Escaped pressed times 02");
				//string url = StaticStrings.ApacheUrl;
				string url = StaticStrings.ApacheUrl+"user_offline";
				//string url = "http://192.168.1.223:81/projects/ludo_master/mob/user_offline";
				string userName = GameManager.Instance.myPlayerData.GetPlayerName ();
				string appId = SystemInfo.deviceUniqueIdentifier;
				string playFabIdd = GameManager.Instance.playfabManager.PlayFabId;

				WWWForm form = new WWWForm();
				form.AddField("playerName", userName);
				form.AddField("appId", appId);
				form.AddField ("playFabId", playFabIdd);
				WWW www = new WWW(url, form);
				StartCoroutine(WaitForRequest(www));

            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Escaped pressed times 01");
            quitMessageGameObject.SetActive(true);
            isQuiting = true;

            StartCoroutine(QuitingTimer());
        }
    }
	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
		}
		else
		{
			Debug.Log("WWW Error: " + www.error);
		}

		Application.Quit();
	}

    IEnumerator QuitingTimer()
    {
        yield return new WaitForSeconds(2);
        isQuiting = false;
        Debug.Log("No response");
        quitMessageGameObject.SetActive(false);
    }
}
