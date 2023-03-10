using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SocketIO;


public class LudoGameController :/* PunBehaviour, */MonoBehaviour, IMiniGame
{

    public GameObject[] dice;
    public GameObject GameGui;
    public GameGUIController gUIController;
    public GameObject[] Pawns1;
    public GameObject[] Pawns2;
    public GameObject[] Pawns3;
    public GameObject[] Pawns4;

    public GameObject gameBoard;
    public GameObject gameBoardScaler;

    [HideInInspector]
    public int steps = 5;

    public bool nextShotPossible;
    private int SixStepsCount = 0;
    public int finishedPawns = 0;
    private int botCounter = 0;
    private List<GameObject> botPawns;
    private SocketIOComponent socketIO;




    public void HighlightPawnsToMove(int player, int steps)
    {

        botPawns = new List<GameObject>();

        gUIController.restartTimer();


        GameObject[] pawns = GameManager.Instance.currentPlayer.pawns;

        this.steps = steps;

        if (steps == 6)
        {
            nextShotPossible = true;
            SixStepsCount++;
            if (SixStepsCount == 3)
            {
                nextShotPossible = false;
                if (GameGui != null)
                {
                    //gUIController.SendFinishTurn();
                    /*Invoke("sendFinishTurnWithDelay", 1.0f);*/
                    sendFinishTurnWithDelay();
                }

                return;
            }
        }
        else
        {
            SixStepsCount = 0;
            nextShotPossible = false;
        }

        bool movePossible = false;

        int possiblePawns = 0;
        GameObject lastPawn = null;
        for (int i = 0; i < pawns.Length; i++)
        {
            bool possible = pawns[i].GetComponent<LudoPawnController>().CheckIfCanMove(steps);
            if (possible)
            {
                lastPawn = pawns[i];
                movePossible = true;
                possiblePawns++;
                botPawns.Add(pawns[i]);
            }
        }



        if (possiblePawns == 1)
        {
            if (GameManager.Instance.currentPlayer.isBot)
            {
                StartCoroutine(movePawn(lastPawn, false));
            }
            else
            {
                lastPawn.GetComponent<LudoPawnController>().MakeMove();
                StartCoroutine(MovePawnWithDelay(lastPawn));
            }

        }
        else
        {
            if (possiblePawns == 2 && lastPawn.GetComponent<LudoPawnController>().pawnInJoint != null)
            {
                if (GameManager.Instance.currentPlayer.isBot)
                {
                    if (!lastPawn.GetComponent<LudoPawnController>().mainInJoint)
                    {
                        StartCoroutine(movePawn(lastPawn, false));
                    }
                    else
                    {
                        StartCoroutine(movePawn(lastPawn.GetComponent<LudoPawnController>().pawnInJoint, false));
                    }
                
                }
                else
                {
                    if (!lastPawn.GetComponent<LudoPawnController>().mainInJoint)
                    {
                        print("AAAAAAAAAABBBBBBBBBBB Call");
                        lastPawn.GetComponent<LudoPawnController>().MakeMove();
                    }
                    else
                    {
                        lastPawn.GetComponent<LudoPawnController>().pawnInJoint.GetComponent<LudoPawnController>().MakeMove();
                    }
                    //lastPawn.GetComponent<LudoPawnController>().MakeMove();
                }
            }
            else
            {
                if (possiblePawns > 0 && GameManager.Instance.currentPlayer.isBot)
                {
                    int bestScoreIndex = 0;
                    int bestScore = int.MinValue;
                    // Make bot move
                    for (int i = 0; i < botPawns.Count; i++)
                    {
                        int score = botPawns[i].GetComponent<LudoPawnController>().GetMoveScore(steps);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestScoreIndex = i;
                        }
                    }
                
                    StartCoroutine(movePawn(botPawns[bestScoreIndex], true));
                }
            }
        }

        if (!movePossible)
        {
            if (GameGui != null)
            {
                gUIController.PauseTimers();
                Invoke("sendFinishTurnWithDelay", 1.0f);
            }
        }
    }

    private IEnumerator MovePawnWithDelay(GameObject lastPawn)
    {
        yield return new WaitForSeconds(1.0f);
        print("MovePawnWithDelayyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy");
        //lastPawn.GetComponent<LudoPawnController>().MakeMove();
    }

    public void sendFinishTurnWithDelay()
    {
        gUIController.SendFinishTurn();
    }

    public void Unhighlight()
    {
        for (int i = 0; i < Pawns1.Length; i++)
        {
            Pawns1[i].GetComponent<LudoPawnController>().Highlight(false);
        }

        for (int i = 0; i < Pawns2.Length; i++)
        {
            Pawns2[i].GetComponent<LudoPawnController>().Highlight(false);
        }

        for (int i = 0; i < Pawns3.Length; i++)
        {
            Pawns3[i].GetComponent<LudoPawnController>().Highlight(false);
        }

        for (int i = 0; i < Pawns4.Length; i++)
        {
            Pawns4[i].GetComponent<LudoPawnController>().Highlight(false);
        }

    }

    void IMiniGame.BotTurn(bool first)
    {

        if (first)
        {
            SixStepsCount = 0;
        }
        Invoke("RollDiceWithDelay", 0.5f/*GameManager.Instance.botDelays[(botCounter + 1) % GameManager.Instance.botDelays.Count]*/);
        botCounter++;
        //throw new System.NotImplementedException();
    }


    public IEnumerator movePawn(GameObject pawn, bool delay)
    {
        if (delay)
        {
            yield return new WaitForSeconds(GameManager.Instance.botDelays[(botCounter + 1) % GameManager.Instance.botDelays.Count]);
            botCounter++;
        }
        pawn.GetComponent<LudoPawnController>().MakeMovePC();
    }

    public void RollDiceWithDelay()
    {
        GameManager.Instance.currentPlayer.dice.GetComponent<GameDiceController>().RollDiceBot(GameManager.Instance.botDiceValues[(botCounter + 1) % GameManager.Instance.botDelays.Count]);
    }


    void IMiniGame.CheckShot()
    {
        throw new System.NotImplementedException();
    }

    void IMiniGame.setMyTurn()
    {
        SixStepsCount = 0;
        GameManager.Instance.diceShot = false;
        dice[0].GetComponent<GameDiceController>().EnableShot();
    }

    void IMiniGame.setOpponentTurn()
    {
        SixStepsCount = 0;
        GameManager.Instance.diceShot = false;
        dice[0].GetComponent<GameDiceController>().DisableShot();
        Unhighlight();
    }



    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        GameManager.Instance.miniGame = this;
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
        /*PhotonNetwork.OnEventCall += this.OnEvent;*/
    }

    // Use this for initialization
    void Start()
    {
        // Scale gameboard
        socketIO.On("GAME_EVENT", OnEvent);

        float scalerWidth = gameBoardScaler.GetComponent<RectTransform>().rect.size.x;
        float boardWidth = gameBoard.GetComponent<RectTransform>().rect.size.x;

        //gameBoard.GetComponent<RectTransform>().localScale = new Vector2(scalerWidth / boardWidth, scalerWidth / boardWidth);

        gUIController = GameGui.GetComponent<GameGUIController>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        /*PhotonNetwork.OnEventCall -= this.OnEvent;*/
        socketIO.Off("GAME_EVENT", OnEvent);
    }

    
    public void OnEvent(SocketIOEvent evt)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data = evt.data.ToDictionary();

        int eventcode = int.Parse(data["event"]);
        
        if (GameManager.Instance.myPlayerData.GetLoginRoomId().Equals(data["roomId"]))
        {
            if (eventcode == (int)EnumGame.DiceRoll)
            {
                gUIController.PauseTimers();
                steps = int.Parse(data["value"]);
                int pl = int.Parse(data["curPlayerIndex"]);

                GameManager.Instance.playerObjects[pl].dice.GetComponent<GameDiceController>().RollDiceStart(steps);
            }
            else if (eventcode == (int)EnumGame.PawnMove)
            {
                int index = int.Parse(data["index"]);
                int pl = int.Parse(data["curPlayerIndex"]);
                steps = int.Parse(data["value"]);
                Debug.Log("MOVE VALUES ::::::::::::::::::::::::::::::::::::::::: " + steps);
                GameManager.Instance.playerObjects[pl].pawns[index].GetComponent<LudoPawnController>().MakeMovePC();
            }
            else if (eventcode == (int)EnumGame.PawnRemove)
            {
                int index = int.Parse(data["index"]);
                int playerIndex = int.Parse(data["curPlayerIndex"]);
                Debug.Log("index = " + index + "   " +  "curPlayerIndex = " + playerIndex);

                //sGameManager.Instance.playerObjects[playerIndex].pawns[index].GetComponent<LudoPawnController>().GoToInitPosition(false);
            }
        }

    }
}
