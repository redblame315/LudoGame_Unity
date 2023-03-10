using UnityEngine.UI;
using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Specialized;
using WebSocketSharp.Net;
using System;
using UnityEngine.Networking;
public class LogInWithEmail : MonoBehaviour {
    
    public GameObject phone;
    public GameObject password;
    public SocketIOComponent socketIO;
    private StaticGameVariablesController staticController;
    public GameObject invalidPhone;
    public GameObject invalidPassword;    
    public GameObject blockWindow;
    public Text blockText;
    public GameObject paswordOTP;
    public GameObject InputPhoneNum;
    public InputField PhoneNumber;
    public RegisterNewAccount RM;

    public InputField otpInput;
    public GameObject InvalidOTP;
    public GameObject ResetPassword;
    public InputField Pwd;
    public InputField ConfirmPwd;
    public GameObject InvalidCofirm;
    public GameObject SuccessConfirm;

    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").GetComponent<SocketIOComponent>();
    }

    // Use this for initialization
    void Start () {
        staticController = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>();

        socketIO.On("CHECKPHONE_RESULT", OnCheckPhoneResult);
        socketIO.On("LOGIN_RESULT", OnLoginResult);       
    }

    // Update is called once per frame
    void Update () {
		if(phone.GetComponent<InputField>().text != "")
        {
            invalidPhone.SetActive(false);            
        }
        if (password.GetComponent<InputField>().text != "")
        {
            invalidPassword.SetActive(false);
        }        
    }

    public void OnLoginBtnClick()
    {
        string logPhone = "91" + phone.GetComponent<InputField>().text;
        string logPassword = password.GetComponent<InputField>().text;
        phone.GetComponent<InputField>().text = "";
        password.GetComponent<InputField>().text = "";

        Dictionary<string, string> data = new Dictionary<string, string>();
        
        data.Add("phone", logPhone);
        data.Add("password", logPassword);

        //socketCtrl.LogIn(data);
        JSONObject sendData = new JSONObject(data);
        socketIO.Emit("USER_LOGIN", sendData);
    }

    public void OnLoginResult(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string logPassword = password.GetComponent<InputField>().text;

        data = evt.data.ToDictionary();
        if (int.Parse(data["states"]) == 1)
        {
            int blockInfo = int.Parse(data["blockinfo"]);

            if (blockInfo == 1)
            {

                if (data["avatar"].Contains("https://") == false)
                {
                    if (data["avatar"] != "")
                    {
                        print("checkeeeeee11111");
                        string[] avatar = data["avatar"].Split('/');
                        string[] avatarIndex = avatar[2].Split('.');

                        GameManager.Instance.avatarMy = staticController.avatars[int.Parse(avatarIndex[0])];
                    }
                }

                GameManager.Instance.myPlayerData.user_data = data;
                PlayerPrefs.SetString("password", data["password"]);
                PlayerPrefs.SetString("log", "login");
                PlayerPrefs.SetString("nickname", GameManager.Instance.myPlayerData.GetPlayerName());
                PlayerPrefs.SetString("id", GameManager.Instance.myPlayerData.GetPlayerId());
                PlayerPrefs.SetString("coins", "" + GameManager.Instance.myPlayerData.GetCoins());                

                if (data["avatar"].Contains("https:") == true)
                    PlayerPrefs.SetString("avatar", data["avatar"]);
                else
                    PlayerPrefs.SetString("avatar", GameManager.Instance.myPlayerData.GetAvatarIndex());

                PlayerPrefs.SetString("blockinfo", data["blockinfo"]);
                PlayerPrefs.SetString("phone", data["phone"]);
                PlayerPrefs.SetString("email", data["email"]);
                PlayerPrefs.SetString("LoggedType", "Email");
                PlayerPrefs.Save();
                Debug.Log(data["email"]);

                print("POINT --- 5");
                SceneManager.LoadScene("MenuScene");

            }
            else
            {
                invalidPassword.transform.Find("Text").GetComponent<Text>().text = "You are blocked by admin";
                invalidPassword.SetActive(true);
            }
        }
        else if(int.Parse(data["states"]) == 2)
        {
            invalidPassword.SetActive(true);
        }
        else if(int.Parse(data["states"]) == 3)
        {
            invalidPhone.SetActive(true);
        }
    }


    public void OnCheckPhoneResult(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data = evt.data.ToDictionary();

        if (int.Parse(data["states"]) == 0)
        {            
            PhoneNumber.text = "";
            InvalidMobile.SetActive(true);
            InvalidMobile.transform.Find("Text").GetComponent<Text>().text = "Incorrect Mobile Number";
            Invoke("DisappearInvalid", 2.0f);
        }
        else if (int.Parse(data["states"]) == 1)
        {
            sendSMS();

            paswordOTP.SetActive(true);
            InputPhoneNum.SetActive(false);
        }      
    }
    
    public void OnInputPhoneNum() // When click the "Forgot Passwords" Button
    {
        InputPhoneNum.gameObject.SetActive(true);
        //gameObject.SetActive(false);
    }
        
    public GameObject InvalidMobile;
    string tempPhoneNumber = "";
    public void OnSubmit()
    {
        tempPhoneNumber = "91" + PhoneNumber.text;
        print("PHone ===== " + tempPhoneNumber);

        if (tempPhoneNumber.Length == 12)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("phone", tempPhoneNumber);            
            JSONObject sendData = new JSONObject(data);
            
            socketIO.Emit("CHECK_PHONE", sendData);
        }
        else
        {
            PhoneNumber.text = "";
            InvalidMobile.transform.Find("Text").GetComponent<Text>().text = "Mobile Number Should be 10 Charactors.";
            InvalidMobile.SetActive(true);
            Invoke("DisappearInvalid", 2.0f);
        }
        
    }
    public void OnVerify()
    {
        if (otpInput.text == otpCode)
        {
            ResetPassword.SetActive(true);
            paswordOTP.SetActive(false);            
        }
        else
        {
            otpInput.text = "";
            InvalidOTP.SetActive(true);
            Invoke("DisappearInvalid", 2.0f);
        }
    }

    public void CloseInputPhone()
    {
        InputPhoneNum.SetActive(false);
        gameObject.SetActive(true);
    }
    public void CloseOTP()
    {
        paswordOTP.SetActive(false);
        InputPhoneNum.SetActive(true);
        InvalidOTP.SetActive(false);
    }
    public void CloseResetWindow()
    {
        ResetPassword.SetActive(false);
        paswordOTP.SetActive(true);
    }

    private void DisappearInvalid()
    {
        if(InvalidOTP.activeSelf)
            InvalidOTP.SetActive(false);
        if (InvalidMobile.activeSelf)
            InvalidMobile.SetActive(false);
        if (InvalidCofirm.activeSelf)
            InvalidCofirm.SetActive(false);
    }
    
    public void OnResetPassword()
    {
        if(Pwd.text == ConfirmPwd.text)
        {
            print("phone" + tempPhoneNumber);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("phone", tempPhoneNumber);
            data.Add("password", ConfirmPwd.text);

            //print("phone, password == " + PhoneNumber.text + "   " + ConfirmPwd.text);
            JSONObject sendData = new JSONObject(data);

            socketIO.Emit("RESET_PASSWORD", sendData);
            Invoke("LoginAsNewPassword", 1.0f);
        }
        else
        {
            InvalidCofirm.SetActive(true);
            Invoke("DisappearInvalid", 2.0f);
        }
    }

    void LoginAsNewPassword()
    {
        if (InvalidCofirm.activeSelf)
            InvalidCofirm.SetActive(false);
        SuccessConfirm.SetActive(true);

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("phone", tempPhoneNumber);
        data.Add("password", ConfirmPwd.text);

        print("phone, password == " + tempPhoneNumber + "   " + ConfirmPwd.text);
        JSONObject sendData = new JSONObject(data);
        socketIO.Emit("USER_LOGIN", sendData);
    }

    public Text DebugText;
    string otpCode = "";

    public void sendSMS()
    {
        int otpValue = UnityEngine.Random.Range(100000, 999999);
        otpCode = otpValue.ToString();

        string URL = "http://173.45.76.227/send.aspx?username=WALUDO&pass=India321&route=trans1&senderid=INDSMS&numbers=" + tempPhoneNumber + "&message=LUDO WALA OTP CODE IS " + otpCode;
        Debug.Log("URL : " + URL);

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("api_url", URL);
        JSONObject sendData = new JSONObject(data);

        socketIO.Emit("REQUEST_API", sendData);
    }
}
