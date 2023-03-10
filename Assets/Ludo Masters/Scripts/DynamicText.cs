using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Globalization;
using System;
using AssemblyCSharp;

public class DynamicText : MonoBehaviour {

	// Use this for initialization
	public Font MyriadWebPro;
	void Start () {

		string url = StaticStrings.UApacheUrl+"getGameHData";
		string playFabId = GameManager.Instance.playfabManager.PlayFabId;

		WWWForm form = new WWWForm();
		form.AddField ("playFabId", playFabId);

		WWW www = new WWW(url, form);

		StartCoroutine(WaitForRequestt(www));

		for (int i = 20; i < 10; i++) {
			GameObject newGO = new GameObject("myTextGO"+i);
			newGO.transform.SetParent(this.transform);
			Text myText = newGO.AddComponent<Text>();
			myText.font = MyriadWebPro;

			myText.rectTransform.sizeDelta = new Vector2(272,20);
			myText.rectTransform.localScale = new Vector3(1, 1, 1);
			myText.resizeTextMaxSize = 40;
			myText.resizeTextMinSize = 14;
			myText.resizeTextForBestFit = true;
			myText.color = new Color(254, 152, 101);
			myText.text = "2p  Win  09/05/2019"+i+ "  50 classic";

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addText(){
		
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
			GameObject newGO = new GameObject("myHistoryE");
			newGO.transform.SetParent(this.transform);
			Text myText = newGO.AddComponent<Text>();
			myText.font = MyriadWebPro;

			myText.rectTransform.sizeDelta = new Vector2(272,40);
			myText.rectTransform.localScale = new Vector3(1, 1, 1);
			myText.resizeTextMaxSize = 40;
			myText.resizeTextMinSize = 14;
			myText.resizeTextForBestFit = true;
			myText.color = new Color(254, 152, 101);
			myText.text = "You dont have any history.\n Please play  games online and enjoy.";
		}
	}

	private void Processjson(string jsonString)
	{
		Debug.Log ("Server Data:- " + jsonString);

		try{
			JsonData jsonvale = JsonMapper.ToObject(jsonString);
			for (int i = 0; i < jsonvale.Count; i++) {

				string type = jsonvale [i] ["game_type"].ToString ();
				string money = jsonvale [i] ["game_money"].ToString ();
				string date = jsonvale [i] ["date"].ToString ();
				string winStatus = jsonvale [i] ["result"].ToString ();

				string winText = (winStatus == "0") ? "Lose" : "Won ";

				GameObject newGO = new GameObject ("myHistory" + i);
				newGO.transform.SetParent (this.transform);
				Text myText = newGO.AddComponent<Text> ();
				myText.font = MyriadWebPro;

				myText.rectTransform.sizeDelta = new Vector2 (272, 20);
				myText.rectTransform.localScale = new Vector3 (1, 1, 1);
				myText.resizeTextMaxSize = 40;
				myText.resizeTextMinSize = 14;
				myText.resizeTextForBestFit = true;
				myText.color = new Color (254, 152, 101);
				myText.text = " "+ type + "  "+winText+" "+date+"  " + money;
			} 
		}catch(Exception e){
			Debug.Log ("Json error:");
			//Debug.LogException (e, this);

			GameObject newGO = new GameObject ("myHistoryE");
			newGO.transform.SetParent (this.transform);
			Text myText = newGO.AddComponent<Text> ();
			myText.font = MyriadWebPro;

			myText.rectTransform.sizeDelta = new Vector2 (272, 40);
			myText.rectTransform.localScale = new Vector3 (1, 1, 1);
			myText.resizeTextMaxSize = 40;
			myText.resizeTextMinSize = 14;
			myText.resizeTextForBestFit = true;
			myText.color = new Color (254, 152, 101);
			myText.text = "You dont have any history.\n Please play games online and enjoy.";
		}
	}
}
