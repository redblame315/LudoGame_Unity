using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
//using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInfoController : MonoBehaviour
{

    public GameObject window;

    public GameObject avatar;
    public GameObject playername;

    public Sprite avatarSprite;

    public GameObject TotalEarningsValue;
    public GameObject CurrentMoneyValue;
    public GameObject GamesWonValue;
    public GameObject WinRateValue;
    public GameObject TwoPlayerWinsValue;
    public GameObject FourPlayerWinsValue;
    public GameObject FourPlayerWinsText;
    public GameObject GamesPlayedValue;
	public GameObject GameLoseValue;

    private int index;
    public Sprite defaultAvatar;

    public GameObject addFriendButton;
    public GameObject editProfileButton;
    public GameObject EditButton;
    public GameObject userNickName;
    public GameObject userPassword;
    public GameObject userEmailID;
    public GameObject userMobile;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (!StaticStrings.isFourPlayerModeEnabled)
        {
            FourPlayerWinsValue.SetActive(false);
            FourPlayerWinsText.SetActive(false);
        }

        if (GameManager.Instance.avatarMy != null)
            avatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;

        defaultAvatar = avatar.GetComponent<Image>().sprite;
    }

    public void ShowPlayerInfo(int index)
    {
        

        if (index == 0)
        {
            this.index = index;
            window.SetActive(true);
            Debug.Log("Show Player Info Function call : " + index);
            FillData(GameManager.Instance.avatarMy, GameManager.Instance.myPlayerData.GetPlayerName(), GameManager.Instance.myPlayerData);
            addFriendButton.SetActive(false);
            Debug.Log("User Phone :::::::" + PlayerPrefs.GetString("phone"));
            userNickName.GetComponent<InputField>().text = GameManager.Instance.myPlayerData.GetPlayerName();
            if(PlayerPrefs.HasKey("password"))
            {
                Debug.Log("User Password :::::::" +  PlayerPrefs.GetString("password"));
                userPassword.GetComponent<InputField>().text = PlayerPrefs.GetString("password");
            }
            if(PlayerPrefs.HasKey("phone"))
            {
                userMobile.GetComponent<InputField>().text = PlayerPrefs.GetString("phone");
            }
            if (PlayerPrefs.HasKey("email"))
            {
                userEmailID.GetComponent<InputField>().text = PlayerPrefs.GetString("email");
            }
            //editProfileButton.SetActive(true);
        }
        else
        {
            /*addFriendButton.SetActive(true);
            editProfileButton.SetActive(false);
            Debug.Log("Player info " + index);

            FillData(GameManager.Instance.playerObjects[index].avatar, GameManager.Instance.playerObjects[index].name, GameManager.Instance.playerObjects[index].data);*/
            return;
        }
    }

    public void ShowPlayerInfo(Sprite avatarSprite, string name, MyPlayerData data)
    {
        editProfileButton.SetActive(false);
        addFriendButton.SetActive(true);

        window.SetActive(true);

        FillData(avatarSprite, name, data);
    }



    public void FillData(Sprite avatarSprite, string name, MyPlayerData data)
    {

        if (avatarSprite == null)
        {
            avatar.GetComponent<Image>().sprite = defaultAvatar;
        }
        else
        {
            avatar.GetComponent<Image>().sprite = avatarSprite;
        }

        playername.GetComponent<Text>().text = name;
        TotalEarningsValue.GetComponent<Text>().text = data.GetTotalEarnings().ToString();
        GamesPlayedValue.GetComponent<Text>().text = data.GetPlayedGamesCount().ToString();
        CurrentMoneyValue.GetComponent<Text>().text = data.GetCoins().ToString();
        //GamesWonValue.GetComponent<Text>().text = (data.GetTwoPlayerWins() + data.GetFourPlayerWins()).ToString();
		//GameLoseValue.GetComponent<Text>().text = (data.GetPlayedGamesCount() - data.GetTwoPlayerWins() + data.GetFourPlayerWins()).ToString();

        float gamesWon = (data.GetTwoPlayerWins() + data.GetFourPlayerWins());
        Debug.Log("WON: " + gamesWon);
        Debug.Log("played: " + data.GetPlayedGamesCount());
        if (data.GetPlayedGamesCount() != 0 && gamesWon != 0)
        {
            WinRateValue.GetComponent<Text>().text = Mathf.RoundToInt((gamesWon / data.GetPlayedGamesCount() * 100)).ToString() + "%";
        }
        else
        {
            WinRateValue.GetComponent<Text>().text = "0%";
        }
        TwoPlayerWinsValue.GetComponent<Text>().text = data.GetTwoPlayerWins().ToString();
        FourPlayerWinsValue.GetComponent<Text>().text = data.GetFourPlayerWins().ToString();

    }
}
