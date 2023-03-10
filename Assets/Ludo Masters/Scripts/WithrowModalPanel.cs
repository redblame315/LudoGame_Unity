using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using AssemblyCSharp;
using System.Globalization;
using System.Collections.Generic;
using SocketIO;

public class WithrowModalPanel : MonoBehaviour
{

    //public Button cancelButton;
    public GameObject modalPanelObject;
    public GameObject modalPanelObjectAdvance;
    public Dropdown myDropdown;
	public InputField amountF;
	public InputField nameF;
	public InputField mobileF;
	public Text textMsg;
	string mobile;
	string name;
	string amount;
	string finalAmount;
	long numVal;
	int numValN;
	bool numValB = false;
	bool mobValB = false;
	bool nameValB = false;
    private SocketIOComponent socketIO;
	public string dropValue = "google";

    private static WithrowModalPanel modalPanel;





    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }

    public static WithrowModalPanel Instance()
    {
        if (!modalPanel)
        {
            //modalPanel = FindObjectOfType(typeof(WithrowModalPanel)) as WithrowModalPanel;
            modalPanel = FindObjectOfType<WithrowModalPanel>();
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }

    private void Start()
    {
        // Just to be sure it is allways only added once
        // I have the habit to remove before adding a listener
        // This is valid even if the listener was not added yet

        myDropdown.onValueChanged.RemoveListener(HandleValueChanged);
        myDropdown.onValueChanged.AddListener(HandleValueChanged);
		textMsg.text = "";
    }

    private void OnDestroy()
    {
        // To avoid errors also remove listeners as soon as they
        // are not needed anymore
        // Otherwise in the case this object is destroyed but the dropdown is not
        // it would still try to call your listener -> Exception
        myDropdown.onValueChanged.RemoveListener(HandleValueChanged);
    }

    private void HandleValueChanged(int newValue)
    {
		mobile = mobileF.text;
		name = nameF.text;
		amount = amountF.text;
        switch (myDropdown.value)
        {
            case 0:
                Debug.Log("Basic panel!");
                //modalPanelObject.SetActive(true);
                //modalPanelObjectAdvance.SetActive(false);
				Debug.Log("Basic panel!" + mobile+"---"+name+"---"+amount);
				dropValue = "google";
                break;

            case 1:
                Debug.Log("Advance panel!");
                //modalPanelObjectAdvance.SetActive(true);
                //modalPanelObject.SetActive(false);
				Debug.Log("Basic panel!" + mobile+"---"+name+"---"+amount);
				dropValue = "paytm";
                break;
        }
    }
	public void sendMoney(){
		string url = StaticStrings.UApacheUrl+"withdrawRequest";

		int fCoinsMoney = GameManager.Instance.myPlayerData.GetCoins();
		Debug.Log ("Final coins" + fCoinsMoney);
		//Debug.Log ("Final coins string" + GameManager.Instance.myPlayerData.GetCoins().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' '));

		string mobile = mobileF.text.ToString();
		string name = nameF.text.ToString();
		string amount = amountF.text.ToString();
		finalAmount = amount;
		int finalReqAmount = int.Parse (amount);

		string playFabId = GameManager.Instance.playfabManager.PlayFabId;
		Debug.Log("Basic panel!" + mobile+"---"+name+"---"+amount);

		Debug.Log (long.TryParse(mobile, out numVal));
		Debug.Log (mobile.Length);

		if(long.TryParse(mobile, out numVal) && mobile.Length == 10)
		{
			mobValB = true;
			Debug.Log ("Here");
		}
		if(int.TryParse(amount, out numValN) && amount.Length > 2 && finalReqAmount <= fCoinsMoney)
		{
			numValB = true;
		}
		if(name.Length > 2){
			nameValB = true;
		}
		if (numValB && mobValB && nameValB) {
			numValB = false;
			mobValB = false;
			nameValB = false;

			WWWForm form = new WWWForm ();
			form.AddField ("playFabId", playFabId);
			form.AddField ("payType", dropValue);
			form.AddField ("amount", amount);
			form.AddField ("mobile", mobile);
			form.AddField ("name", name);
			WWW www = new WWW (url, form);

			StartCoroutine (WaitForRequest (www));

			//inputfieldname.Select();
			mobileF.text = "";
			nameF.text = "";
			amountF.text = "";
			textMsg.text = "Your payment withraw request send successfull, \n" +
				"Please check status in payment history section.";

		} else {
			if (!numValB) {
				textMsg.text = "Please enter valid amount, and minimum withdraw amout is 99.";
			}else if(!nameValB){
				textMsg.text = "";
				textMsg.text = "Please enter valid name.";
			}else {
				textMsg.text = "";
				textMsg.text = "Please enter valid mobile number.";
			}
		}
	}

	IEnumerator WaitForRequest(WWW www)  
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!-: " + www.text);
			int myInt = int.Parse (finalAmount);
			Dictionary<string, string> data = new Dictionary<string, string>();
            GameManager.Instance.myPlayerData.user_data[MyPlayerData.CoinsKey] = (GameManager.Instance.myPlayerData.GetCoins() - myInt).ToString();
            //GameManager.Instance.myPlayerData.user_data.Add(MyPlayerData.CoinsKey, (GameManager.Instance.myPlayerData.GetCoins() - myInt).ToString());
            JSONObject jsonData = new JSONObject(GameManager.Instance.myPlayerData.user_data);

            socketIO.Emit("UPDATE_USER_INFO", jsonData);


            string url = StaticStrings.UApacheUrl+"updateUserDataC";

			string coinsMoney = GameManager.Instance.myPlayerData.GetCoins().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
			string totalEarnings = GameManager.Instance.myPlayerData.GetTotalEarnings().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
			int totalGamePlayed = GameManager.Instance.myPlayerData.GetPlayedGamesCount();
			string playFabId = GameManager.Instance.playfabManager.PlayFabId;

			WWWForm form1 = new WWWForm();

			form1.AddField("totalCoins", coinsMoney);
			form1.AddField("earning", totalEarnings);
			form1.AddField("totalGamePlayed", totalGamePlayed);
			form1.AddField("playFabId", playFabId);

			Debug.Log("url: " + url);

			WWW wwwF = new WWW(url, form1);

			StartCoroutine(WaitForRequest2(wwwF));
		}
		else
		{
			Debug.Log("WWW Error: " + www.error);
		}
	}
	IEnumerator WaitForRequest2(WWW www)
	{
		yield return www;

		if (www.error == null)
		{
			Debug.Log("OK: " + www.text);
		}
		else
		{
			Debug.Log("ERROR: " + www.error);
		}
	}
}


