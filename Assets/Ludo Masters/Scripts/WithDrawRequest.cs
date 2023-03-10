using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;


public class WithDrawRequest : MonoBehaviour {
    
    public GameObject coinAmount;
    public GameObject paytmPhone;
    public GameObject invalidInput;
    public GameObject invalidPhone;
    public SocketIOComponent gameSocketIO;
    public GameObject withDrawWindow;
    public GameObject verifycodeInputField;
    public GameObject verifycodeWindow;
    public GameObject alertWindow;
    public GameObject MessageWindow;
    private int coin;
    private string phoneNum;

    private void Awake()
    {
        gameSocketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CheckCoin()
    {
        coin = int.Parse(coinAmount.GetComponent<InputField>().text);
        phoneNum = paytmPhone.GetComponent<InputField>().text;
        if (coin > GameManager.Instance.myPlayerData.GetCoins() || coin < 100)
        {
            invalidInput.SetActive(true);
            coinAmount.GetComponent<InputField>().text = "";
            Invoke("Disappear" , 1.5f);
            return;
        }

        if(phoneNum.Length < 10)
        {
            invalidPhone.SetActive(true);
            paytmPhone.GetComponent<InputField>().text = "";
            Invoke("Disappear", 1.5f);
            return;
        }

        

        Invoke("AppearMessage", 0.5f);
        phoneNum = "91" + phoneNum;
        WithDrawCoin();
    }

    void Disappear()
    {
        if(invalidInput.activeSelf)
            invalidInput.SetActive(false);
        if (invalidPhone.activeSelf)
            invalidPhone.SetActive(false);
    }

    void AppearMessage()
    {
        MessageWindow.SetActive(true);
    }

    public void OnClickOk()
    {
        MessageWindow.SetActive(false);
        gameObject.SetActive(false);
    }

    public void WithDrawCoin()
    {
        //alertWindow.SetActive(false);
        withDrawWindow.SetActive(false);
        coinAmount.GetComponent<InputField>().text = "";
        paytmPhone.GetComponent<InputField>().text = "";
        invalidInput.SetActive(false);
        invalidPhone.SetActive(false);

        Dictionary<string, string> dicData = new Dictionary<string, string>();

        dicData.Add("user_name", GameManager.Instance.myPlayerData.GetPlayerName());
        dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
        dicData.Add("coins", "" + coin);
        dicData.Add("paytmPhone", phoneNum);
        Debug.Log("PHONE NUMBER : " + PlayerPrefs.GetString("phone"));
        if (PlayerPrefs.HasKey("phone"))
        {
            dicData.Add("phone", PlayerPrefs.GetString("phone"));
        }
        else
        {
            dicData.Add("phone", "0");
        }
        Debug.Log("PHONE NUMBER : " + dicData["phone"]);


        JSONObject jsonData = new JSONObject(dicData);

        Debug.Log("Send Request : " + coin);
        gameSocketIO.Emit("WITHDRAW_REQUEST", jsonData);
    }

    public void SendVerifyCode()
    {
        string verficode = verifycodeInputField.GetComponent<InputField>().text;


        verifycodeWindow.SetActive(false);

        Dictionary<string, string> dicData = new Dictionary<string, string>();

        dicData.Add("user_name", GameManager.Instance.myPlayerData.GetPlayerName());
        dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
        dicData.Add("verficode",  verficode);

        JSONObject jsonData = new JSONObject(dicData);

        gameSocketIO.Emit("VERIFY_CODE", jsonData);
    }
    
}
