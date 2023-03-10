using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class GameConfigrationController : MonoBehaviour
{

    public GameObject TitleText;
    public GameObject bidText;
    public GameObject MinusButton;
    public GameObject PlusButton;
    public GameObject[] Toggles;
    private int currentBidIndex = 0;
    public GameObject content;
    public GameObject twoPlayerTitile;
    public GameObject foruPlayerTitle;
    public GameObject privateTitle;




    private MyGameMode[] modes = new MyGameMode[] { MyGameMode.Classic, MyGameMode.Quick, MyGameMode.Master };
    public GameObject privateRoomJoin;



    // Use this for initialization
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {

    }


    void OnEnable()
    {
        for (int i = 0; i < Toggles.Length; i++)
        {
            int index = i;
            Toggles[i].GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                {
                    ChangeGameMode(value, modes[index]);
                }
            );
        }

        currentBidIndex = 0;
        UpdateBid(true);

        Toggles[0].GetComponent<Toggle>().isOn = true;
        GameManager.Instance.mode = MyGameMode.Classic;

        switch (GameManager.Instance.type)
        {
            case MyGameType.TwoPlayer:
                twoPlayerTitile.SetActive(true);
                foruPlayerTitle.SetActive(false);
                privateTitle.SetActive(false);
                break;
            case MyGameType.FourPlayer:
                foruPlayerTitle.SetActive(true);
                twoPlayerTitile.SetActive(false);
                privateTitle.SetActive(false);
                break;
            case MyGameType.Private:                
                privateTitle.SetActive(true);
                privateRoomJoin.SetActive(true);
                foruPlayerTitle.SetActive(false);
                twoPlayerTitile.SetActive(false);
                break;
        }

    }

    void OnDisable()
    {
        for (int i = 0; i < Toggles.Length; i++)
        {
            int index = i;
            Toggles[i].GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        }

        privateRoomJoin.SetActive(false);
        currentBidIndex = 0;
        UpdateBid(false);
        Toggles[0].GetComponent<Toggle>().isOn = true;
        Toggles[1].GetComponent<Toggle>().isOn = false;
        Toggles[2].GetComponent<Toggle>().isOn = false;
    }

    public void setCreatedProvateRoom()
    {
        GameManager.Instance.JoinedByID = false;
    }

    public void startGame()
    {
        Debug.Log(GameManager.Instance.myPlayerData.GetCoins());
        if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.payoutCoins)
        {
            if (GameManager.Instance.type != MyGameType.Private)
            {
                Debug.Log("start random game");
                GameManager.Instance.facebookManager.startRandomGame();
            }
            else
            {
                if (GameManager.Instance.JoinedByID)
                {
                    Debug.Log("Joined by id!");
                    GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                }
                else
                {
                    Debug.Log("Joined and created");
                    GameManager.Instance.playfabManager.CreatePrivateRoom();
                    GameManager.Instance.matchPlayerObject.GetComponent<SetMyData>().MatchPlayer();
                }

            }
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }
    }

    private void ChangeGameMode(bool isActive, MyGameMode mode)
    {
        if (isActive)
        {
            GameManager.Instance.mode = mode;
        }
    }



    public void IncreaseBid()
    {
        if (currentBidIndex < StaticStrings.bidValues.Length - 1)
        {
            currentBidIndex++;
            UpdateBid(true);
        }
    }

    public void DecreaseBid()
    {
        if (currentBidIndex > 0)
        {
            currentBidIndex--;
            UpdateBid(true);
        }
    }

    private void UpdateBid(bool changeBidInGM)
    {
        bidText.GetComponent<Text>().text = StaticStrings.bidValuesStrings[currentBidIndex];

        if (changeBidInGM)
            GameManager.Instance.payoutCoins = StaticStrings.bidValues[currentBidIndex];

        if (currentBidIndex == 0) MinusButton.GetComponent<Button>().interactable = false;
        else MinusButton.GetComponent<Button>().interactable = true;

        if (currentBidIndex == StaticStrings.bidValues.Length - 1) PlusButton.GetComponent<Button>().interactable = false;
        else PlusButton.GetComponent<Button>().interactable = true;
    }

    public void HideThisScreen()
    {
        for(int i = 1; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        gameObject.SetActive(false);
    }
   
   
}
