using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using AssemblyCSharp;
using System;
using LitJson;

public class PaymentPanel : MonoBehaviour {

	public GameObject modalPanelObject;

	private static PaymentPanel modalPanel;

	public static PaymentPanel Instance()
	{
		if (!modalPanel)
		{
			modalPanel = FindObjectOfType(typeof(PaymentPanel)) as PaymentPanel;
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
	public void urlBtnFn(){
		string playFabId = GameManager.Instance.playfabManager.PlayFabId;
		Application.OpenURL("http://3.6.89.148/ludoearn");
	}
	public void refreshBtn(){
		string url = StaticStrings.UApacheUrl+"get_user_updated_coins";
		string playFabId = GameManager.Instance.playfabManager.PlayFabId;
		WWWForm form = new WWWForm();
		form.AddField ("playFabId", playFabId);
		WWW www = new WWW(url, form);
		StartCoroutine(WaitForRequestt(www));
	}
	IEnumerator WaitForRequestt(WWW www)
	{
		yield return www;

		if (www.error == null)
		{
			Processjson(www.text);
			Debug.Log(www.text);

		}
		else
		{
			Debug.Log("ERROR: " + www.error);
		}
	}
	private void Processjson(string jsonString)
	{
		Debug.Log ("Server Data:- " + jsonString);

		try{
			JsonData jsonvale = JsonMapper.ToObject(jsonString);
			for (int i = 0; i < jsonvale.Count; i++) {
				Debug.Log(jsonvale[i]);
				string amountM = jsonvale [i] ["amount"].ToString ();
				string payId = jsonvale [i] ["pay_id"].ToString ();
				string pFlag = jsonvale [i] ["flag"].ToString ();
				if(pFlag == "1" && !String.IsNullOrEmpty(amountM) && !String.IsNullOrEmpty(payId)){
					
				}
			} 
		}catch(Exception e){
			Debug.Log ("Json error:");
			//Debug.LogException (e, this);
		}
	}
}
