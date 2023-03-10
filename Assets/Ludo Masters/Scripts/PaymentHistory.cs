using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;
using LitJson;
using AssemblyCSharp;

public class PaymentHistory : MonoBehaviour {

	public GameObject modalPanelObject;

	private static PaymentHistory modalPanel;

	public static PaymentHistory Instance()
	{
		if (!modalPanel)
		{
			modalPanel = FindObjectOfType(typeof(PaymentHistory)) as PaymentHistory;
			if (!modalPanel)
				Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
		}

		return modalPanel;
	}
	public void OpenPanel()
	{
		modalPanelObject.SetActive(true);
	}
	public void ClosePanel()
	{
		modalPanelObject.SetActive(false);
	}

	public Font MyriadWebPro;
	void Start () {

		string url = StaticStrings.UApacheUrl+"myWithdrawData";
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
			myText.text = "Withdraw  Win  09/05/2019"+i+ "  50 classic";

		}
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
			GameObject newGO = new GameObject("myHistoryE");
			newGO.transform.SetParent(this.transform);
			Text myText = newGO.AddComponent<Text>();
			myText.font = MyriadWebPro;

			myText.rectTransform.sizeDelta = new Vector2(420,50);
			myText.rectTransform.localScale = new Vector3(1, 1, 1);
			myText.resizeTextMaxSize = 40;
			myText.resizeTextMinSize = 14;
			myText.resizeTextForBestFit = true;
			myText.color = new Color(254, 152, 101);
			myText.text = "You dont have any history.\n Please add money from add money section.";
		}
	}

	private void Processjson(string jsonString)
	{
		
		try{
			JsonData jsonvale = JsonMapper.ToObject(jsonString);
			for (int i = 0; i < jsonvale.Count; i++) {

				string amountM = jsonvale [i] ["amount"].ToString ();
				string status = jsonvale [i] ["status"].ToString ();
				string date = jsonvale [i] ["date"].ToString ();

				string finalStatus = (status=="0") ? "Pending" : "Success";

				GameObject newGO = new GameObject ("myHistoryE" + i);
				newGO.transform.SetParent (this.transform);
				Text myText = newGO.AddComponent<Text> ();
				myText.font = MyriadWebPro;

				myText.rectTransform.sizeDelta = new Vector2 (430, 33);
				myText.rectTransform.localScale = new Vector3 (1, 1, 1);
				myText.fontSize = 23;
				myText.resizeTextMaxSize = 40;
				myText.resizeTextMinSize = 14;
				//myText.resizeTextForBestFit = true;
				myText.color = new Color (254, 152, 101);
				myText.text = " Withdraw" +"  " +amountM+"  "+finalStatus+"  " + date;
			} 
		}catch(Exception e){
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

