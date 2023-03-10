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

public class RegisterNewAccount : MonoBehaviour {


    public GameObject nickName;
    public GameObject email;
    public GameObject password;
    public GameObject passwordConf;
    public GameObject phoneNumber;    
    public GameObject gridView;
    public GameObject buttonPrefab;
    public GameObject TermsAndCondition;

    public GameObject invalidName;
    public GameObject invalidAvatar;
    public GameObject invalidPassword;
    public GameObject invalidPasswordConf;
    public GameObject invalidEmail;
    public GameObject invalidPhone;
    public GameObject invalidTerms;
    public GameObject turnOn;
    //private SocketController socketCtrl;

    bool checkTerm = false;

    private string avatarIndex = "0";

    public SocketIOComponent socketIO;

    private StaticGameVariablesController staticController;

    private List<GameObject> buttons = new List<GameObject>();

    

    // Use this for initialization
    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").GetComponent<SocketIOComponent>();
    }


    void Start()
    {
        print("qweqweqweqweqwe");
        socketIO.On("REGISTER_RESULT", OnRegisterResult);
       

        //avatarIndex = GameManager.Instance.myPlayerData.GetAvatarIndex();

        //GameObject go = GameObject.Find("SocketController");
        //socketCtrl = go.GetComponent<SocketController>();

        staticController = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>();
        //nickName.GetComponent<InputField>().text = GameManager.Instance.nameMy;
// 
//         if (GameManager.Instance.facebookAvatar != null)
//         {
//             GameObject button = Instantiate(buttonPrefab);
//             button.GetComponent<ProfilePictureController>().picture.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
//             button.transform.SetParent(gridView.transform, false);
// 
//             GameObject border = button.GetComponent<ProfilePictureController>().frame;
//             if (GameManager.Instance.myPlayerData.GetAvatarIndex().Equals("fb"))
//             {
//                 border.GetComponent<Image>().color = Color.green;
//             }
// 
//             string index = "fb";
//             button.GetComponent<Button>().onClick.RemoveAllListeners();
//             button.GetComponent<Button>().onClick.AddListener(() => ClickButton(index, border));
// 
//             buttons.Add(border);
// 
// 
//             
//         }

        for (int i = 0; i < staticController.avatars.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.GetComponent<ProfilePictureController>().picture.GetComponent<Image>().sprite = staticController.avatars[i];
            button.transform.SetParent(gridView.transform, false);

            GameObject border = button.GetComponent<ProfilePictureController>().frame;
//             if (GameManager.Instance.myPlayerData.GetAvatarIndex().Equals(i + ""))
//             {
//                 border.GetComponent<Image>().color = Color.green;
//             }

            string index = "" + i;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => ClickButton(index, border));

            buttons.Add(border);
        }
    }

    public void ClickButton(string avatarIndex, GameObject border)
    {
        this.avatarIndex = avatarIndex;

        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].GetComponent<Image>().color = Color.white;

        }
        border.GetComponent<Image>().color = Color.green;
    }

    string regName = "";
    string regEmail = "";
    string regPassword = "";
    string regPasswordConf = "";
    string regPhone = "";
    string refCode = "";
    public void OnRegister()
    {
        regName = nickName.GetComponent<InputField>().text;
        regEmail = email.GetComponent<InputField>().text;
        regPassword = password.GetComponent<InputField>().text;
        regPasswordConf = passwordConf.GetComponent<InputField>().text;
        regPhone = "91" + phoneNumber.GetComponent<InputField>().text;
        refCode = referInput.GetComponent<InputField>().text;

        if (!Regex.IsMatch(regEmail, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"))
        {            
            regEmail = "";
            invalidEmail.SetActive(true);
        }
        else if (regPhone.Length < 12)
        {
            regPhone = "";
            invalidPhone.SetActive(true);            
        }
        else if(regPassword.Length < 6)
        {
            regPassword = "";
            invalidPassword.SetActive(true);            
        }        
        else if(regPassword != regPasswordConf)
        {
            regPasswordConf = "";
            invalidPasswordConf.SetActive(true);
        }
        else if(regName.Length <= 0)
        {
            regName = "";
            invalidName.SetActive(true);
        }
        else if(!avatarIndex.Equals("fb") && int.Parse(avatarIndex) < 0)
        {
            invalidAvatar.SetActive(true);
        }       
        else if(!checkTerm)
        {
            invalidTerms.SetActive(true);
        }
        else
        {
            OTPwindow.SetActive(true);
           
            sendSMS();
        }
    }

    public GameObject otpInput;
    public GameObject referInput;

    public void OnVerify()
    {
        
        if(otpInput.GetComponent<InputField>().text == otpCode)
        {
            GameManager.Instance.myphoneNum = regPhone;
            GameManager.Instance.myReferCode = refCode;
            print(" GameManager.Instance.myphoneNum == " + GameManager.Instance.myphoneNum);
            print("GameManager.Instance.myReferCode == " + GameManager.Instance.myReferCode);

            referInput.GetComponent<InputField>().text = "";
            nickName.GetComponent<InputField>().text = "";
            email.GetComponent<InputField>().text = "";
            password.GetComponent<InputField>().text = "";
            phoneNumber.GetComponent<InputField>().text = "";
            GameManager.Instance.nameMy = nickName.GetComponent<InputField>().text;

            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("email", regEmail);
            data.Add("password", regPassword);
            data.Add("nickname", regName);
            data.Add("phone", regPhone);
            data.Add("avatar", avatarIndex);
            data.Add("money", "0");

            PlayerPrefs.SetString("nickname", regName);
            PlayerPrefs.SetString("password", regPassword);
            PlayerPrefs.SetString("phone", regPhone);
            PlayerPrefs.SetString("email", regEmail);
            PlayerPrefs.SetString("coins", "0");

            //socketCtrl.Register(data);
            JSONObject sendData = new JSONObject(data);

            print("123123123123");
            socketIO.Emit("USER_REGISTER", sendData);

            if (avatarIndex.Equals("fb"))
            {
                GameManager.Instance.avatarMy = GameManager.Instance.facebookAvatar;
            }
            else
            {
                GameManager.Instance.avatarMy = staticController.avatars[int.Parse(avatarIndex)];
            }
        }
    }

    public GameObject RepeatPhoneCheck;
    public void OnRegisterResult(SocketIOEvent evt)
    {
        Debug.Log(evt.name + evt.data);

        GameManager.Instance.myPlayerData.user_data = evt.data.ToDictionary();
       
        if((GameManager.Instance.myPlayerData.user_data["states"]) == "1")
        {
            RepeatPhoneCheck.SetActive(true);
            return;
        }

        PlayerPrefs.SetString("log", "login");
        PlayerPrefs.SetString("nickname", GameManager.Instance.myPlayerData.GetPlayerName());
        PlayerPrefs.SetString("id", GameManager.Instance.myPlayerData.GetPlayerId());
        PlayerPrefs.SetString("avatar", GameManager.Instance.myPlayerData.GetAvatarIndex());
        PlayerPrefs.SetString("blockinfo", GameManager.Instance.myPlayerData.user_data["blockinfo"]);
        PlayerPrefs.SetString("LoggedType", "Email");
        PlayerPrefs.Save();

        OTPwindow.SetActive(false);

        //regName = "";
        //regPhone = "";
        //regEmail = "";
        //regPassword = "";

        print("POINT --- 8");
        SceneManager.LoadScene("MenuScene");
    }

    


    public GameObject OTPwindow;
    public void OnCloseOTP()
    {
        otpInput.GetComponent<InputField>().text = "";
        OTPwindow.SetActive(false);
        gameObject.SetActive(true);
        RepeatPhoneCheck.SetActive(false);
    }

    string otpCode = "";
    public Text DebugText;    
    public void sendSMS()
    {
        int otpValue = UnityEngine.Random.Range(100000, 999999);
        otpCode = otpValue.ToString();

        string URL = "http://173.45.76.227/send.aspx?username=WALUDO&pass=India321&route=trans1&senderid=INDSMS&numbers=" + regPhone + "&message=LUDO WALA OTP CODE IS " + otpCode;
        Debug.Log("URL : " + URL);
    
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("api_url", URL);
        JSONObject sendData = new JSONObject(data);

        socketIO.Emit("REQUEST_API", sendData);
    }


    public void AgreeTermsCondition()
    {
        if (turnOn.activeSelf)
        {
            checkTerm = false;
            turnOn.SetActive(false);
            invalidTerms.SetActive(false);
        }            
        else
        {           
            TermsAndCondition.SetActive(true);
            checkTerm = true;
            turnOn.SetActive(true);
        }   
    }

    public void OnOKButton()
    {
        TermsAndCondition.SetActive(false);
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Regex.IsMatch(email.GetComponent<InputField>().text, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"))
        {
            invalidEmail.SetActive(false);            
        }

        if (phoneNumber.GetComponent<InputField>().text != "" && 
            phoneNumber.GetComponent<InputField>().text.Length == 10)
        {
            invalidPhone.SetActive(false);
        }            
        
        if (password.GetComponent<InputField>().text != "" &&
            password.GetComponent<InputField>().text.Length >= 6)
        {
            invalidPassword.SetActive(false);
        }
        if(password.GetComponent<InputField>().text == passwordConf.GetComponent<InputField>().text)
        {
            invalidPasswordConf.SetActive(false);
        }
        if (nickName.GetComponent<InputField>().text != "")
        {
            invalidName.SetActive(false);
        }
        if (checkTerm)
        {
            invalidTerms.SetActive(false);
        }   
    }
}
