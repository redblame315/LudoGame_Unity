using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class GameDiceController : MonoBehaviour
{

    public Sprite[] diceValueSprites;
    public GameObject arrowObject;
    public GameObject diceValueObject;
    public GameObject diceAnim;

    // Use this for initialization
    public bool isMyDice = false;
    public GameObject LudoController;
    public LudoGameController controller;
    public int player = 1;
    private Button button;
    private int steps = 0;
    public GameObject notInteractable;
    private SocketIOComponent socketIO;

    


    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }

    void Start()
    {
        button = GetComponent<Button>();
        controller = LudoController.GetComponent<LudoGameController>();

        button.interactable = false;
    }

    public void SetDiceValue()
    {
        diceValueObject.GetComponent<Image>().sprite = diceValueSprites[steps - 1];
        diceValueObject.SetActive(true);
        diceAnim.SetActive(false);
        controller.gUIController.restartTimer();
        if (isMyDice)
        {
            controller.HighlightPawnsToMove(player, steps);
        }
        if (GameManager.Instance.currentPlayer.isBot)
        {
            controller.HighlightPawnsToMove(player, steps);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnableShot()
    {
        if (GameManager.Instance.currentPlayer.isBot)
        {
            GameManager.Instance.miniGame.BotTurn(false);
            notInteractable.SetActive(false);
        }
        else
        {
            if (PlayerPrefs.GetInt(StaticStrings.VibrationsKey, 0) == 0)
            {
#if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
#endif
            }
            else
            {
            }
            controller.gUIController.myTurnSource.Play();
            notInteractable.SetActive(false);
            button.interactable = true;
            arrowObject.SetActive(true);
        }
    }

    public void DisableShot()
    {
        notInteractable.SetActive(true);
        button.interactable = false;
        arrowObject.SetActive(false);
    }

    public void EnableDiceShadow()
    {
        notInteractable.SetActive(true);
    }

    public void DisableDiceShadow()
    {
        notInteractable.SetActive(false);
    }
    int aa = 0;
    int bb = 0;
    public void RollDice()
    {
        Dictionary<string, string> sendData = new Dictionary<string, string>();

        
        if (isMyDice)
        {
            controller.nextShotPossible = false;
            controller.gUIController.PauseTimers();
            button.interactable = false;
            arrowObject.SetActive(false);
            // if (aa % 2 == 0) steps = 6;
            // else steps = 2;
            // aa++;
            steps = Random.Range(1, 7);
        
            RollDiceStart(steps);
            string data = steps + ";" + controller.gUIController.GetCurrentPlayerIndex();
            sendData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
            sendData.Add("currentPlayer", "" + controller.gUIController.GetCurrentPlayerIndex());
            sendData.Add("value", "" + steps);
            sendData.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());
            JSONObject jsonSendData = new JSONObject(sendData);

            socketIO.Emit("ROLL_DICE_EVENT", jsonSendData);
            /*PhotonNetwork.RaiseEvent((int)EnumGame.DiceRoll, data, true, null);*/
            
        }
    }



    public void RollDiceBot(int value)
    {

        controller.nextShotPossible = false;
        controller.gUIController.PauseTimers();

        
        // if (bb % 2 == 0) steps = 6;
        // else steps = 2;
        // bb++;

        steps = value;

        RollDiceStart(steps);


    }

    public void RollDiceStart(int steps)
    {
        GetComponent<AudioSource>().Play();
        this.steps = steps;
        diceValueObject.SetActive(false);
        diceAnim.SetActive(true);
        diceAnim.GetComponent<Animator>().Play("RollDiceAnimation");
    }
}
