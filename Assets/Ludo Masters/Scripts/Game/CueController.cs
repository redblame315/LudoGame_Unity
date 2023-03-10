using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class CueController : MonoBehaviour
{


    [HideInInspector]
    public bool isServer;

    public GameObject youWonMessage;
    private bool canShowControllers = true;
    public GameObject prizeText;
    private AudioSource[] audioSources;
    public GameObject audioController;
    public GameObject invitiationDialog;
    public GameObject chatButton;
    public GameControllerScript gameControllerScript;


    void Start()
    {

        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();

        if (GameManager.Instance.offlineMode)
        {
            chatButton.SetActive(false);
        }


        if (!GameManager.Instance.offlineMode)
            GameManager.Instance.playfabManager.addCoinsRequest(-GameManager.Instance.payoutCoins);


        GameManager.Instance.audioSources = audioController.GetComponents<AudioSource>();
        audioSources = GetComponents<AudioSource>();

        GameManager.Instance.iWon = false;
        GameManager.Instance.iLost = false;
        GameManager.Instance.iDraw = false;


        setPrizeText();


        GameManager.Instance.cueController = this;

        isServer = false;

        if (GameManager.Instance.roomOwner)
        {
            isServer = true;
        }

    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            /*PhotonNetwork.RaiseEvent(151, 1, true, null);
            PhotonNetwork.SendOutgoingCommands();*/
            Debug.Log("Application pause");
        }
        else
        {
            /*PhotonNetwork.RaiseEvent(152, 1, true, null);
            PhotonNetwork.SendOutgoingCommands();*/
            Debug.Log("Application resume");
        }
    }



    private void setPrizeText()
    {
        int prizeCoins = GameManager.Instance.payoutCoins * 2;

        if (prizeCoins >= 1000)
        {
            if (prizeCoins >= 1000000)
            {
                if (prizeCoins % 1000000.0f == 0)
                {
                    prizeText.GetComponent<Text>().text = (prizeCoins / 1000000.0f).ToString("0") + "M";

                }
                else
                {
                    prizeText.GetComponent<Text>().text = (prizeCoins / 1000000.0f).ToString("0.0") + "M";

                }

            }
            else
            {
                if (prizeCoins % 1000.0f == 0)
                {
                    prizeText.GetComponent<Text>().text = (prizeCoins / 1000.0f).ToString("0") + "k";
                }
                else
                {
                    prizeText.GetComponent<Text>().text = (prizeCoins / 1000.0f).ToString("0.0") + "k";
                }

            }
        }
        else
        {
            prizeText.GetComponent<Text>().text = prizeCoins + "";
        }

        if (GameManager.Instance.offlineMode)
        {
            prizeText.GetComponent<Text>().text = "Practice";
        }
    }




    void Awake()
    {
        /*PhotonNetwork.OnEventCall += this.OnEvent;*/
    }

    public void removeOnEventCall()
    {
        /*PhotonNetwork.OnEventCall -= this.OnEvent;*/
    }


    void Update()
    {

    }

    void FixedUpdate()
    {

    }


    void OnDestroy()
    {
        /*PhotonNetwork.OnEventCall -= this.OnEvent;*/
    }

    // Multiplayer data received
    private void OnEvent(byte eventcode, object content, int senderid)
    {

        // if (!isServer && eventcode == 0)
        // {

        // }
        // else if (eventcode == 19)
        // { // Opponent Won!
        //     HideAllControllers();
        //     GameManager.Instance.audioSources[3].Play();
        //     youWonMessage.SetActive(true);
        //     youWonMessage.GetComponent<YouWinMessageChangeSprite>().changeSprite();
        //     youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
        //     GameManager.Instance.iWon = false;
        // }
        // else if (eventcode == 20)
        // { // You won!
        //     HideAllControllers();
        //     GameManager.Instance.audioSources[3].Play();
        //     youWonMessage.SetActive(true);
        //     youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
        //     GameManager.Instance.iWon = true;
        // }
        // else if (eventcode == 21)
        // { // You draw!
        //     HideAllControllers();
        //     GameManager.Instance.audioSources[3].Play();
        //     youWonMessage.SetActive(true);
        //     youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
        //     GameManager.Instance.iDraw = true;
        // }
        // else if (eventcode == 192)
        // { // Invitiation received
        //     invitiationDialog.GetComponent<PhotonChatListener2>().showInvitationDialog(null, null, null);
        // }
        // else if (eventcode == 151)
        // { // Opponent paused game
        //     // if (isServer)
        //     //     ShotPowerIndicator.anim.Play("ShotPowerAnimation");
        //     GameManager.Instance.opponentActive = false;
        //     GameManager.Instance.stopTimer = true;
        //     GameManager.Instance.gameControllerScript.showMessage(StaticStrings.waitingForOpponent + " " + StaticStrings.photonDisconnectTimeout);
        // }
        // else if (eventcode == 152)
        // { // Opponent resumed game
        //     // if (canShowControllers && isServer && !shotMyTurnDone)
        //     //     ShotPowerIndicator.anim.Play("MakeVisible");
        //     GameManager.Instance.opponentActive = true;

        //     // if ((isServer && !shotMyTurnDone) || !isServer)
        //     GameManager.Instance.stopTimer = false;
        //     // GameManager.Instance.gameControllerScript.hideBubble();

        // }
        // else if (eventcode == 9)
        // { // My turn - show cue and lines

        //     setMyTurn();
        // }

    }

    public void setOpponentTurn()
    {
        isServer = false;
        gameControllerScript.resetTimers(2, true);
        GameManager.Instance.miniGame.setOpponentTurn();
    }

    public void setMyTurn()
    {
        GameManager.Instance.myTurnDone = false;
        isServer = true;
        gameControllerScript.resetTimers(1, true);
        GameManager.Instance.miniGame.setMyTurn();
    }

    public void checkShot()
    {
        if (GameManager.Instance.iWon)
        {
            IWon();
        }
        else if (GameManager.Instance.iLost)
        {
            ILost();
        }
    }


    public void IWon()
    {
        GameManager.Instance.iWon = true;
        HideAllControllers();
        GameManager.Instance.audioSources[3].Play();
        youWonMessage.SetActive(true);
        youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
        /*if (!GameManager.Instance.offlineMode)
            PhotonNetwork.RaiseEvent(19, null, true, null);*/
    }

    public void Draw()
    {
        GameManager.Instance.iDraw = true;
        HideAllControllers();
        GameManager.Instance.audioSources[3].Play();
        youWonMessage.SetActive(true);
        youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
        /*if (!GameManager.Instance.offlineMode)
            PhotonNetwork.RaiseEvent(21, null, true, null);*/
    }

    public void ILost()
    {
        GameManager.Instance.iWon = false;
        HideAllControllers();
        GameManager.Instance.audioSources[3].Play();
        youWonMessage.SetActive(true);
        youWonMessage.GetComponent<YouWinMessageChangeSprite>().changeSprite();
        youWonMessage.GetComponent<Animator>().Play("YouWinMessageAnimation");
        /*if (!GameManager.Instance.offlineMode)
            PhotonNetwork.RaiseEvent(20, null, true, null);*/
    }

    public void setTurnOffline(bool showTurnMessage)
    {

    }





    private void ShowAllControllers()
    {
        if (canShowControllers)
        {
            Debug.Log("Showing controllers");
        }
    }

    public void HideAllControllers()
    {

    }

    public void stopTimer()
    {
        GameManager.Instance.stopTimer = true;
    }

}
