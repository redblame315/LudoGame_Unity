using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SocketIO;

public class GameRoom : MonoBehaviour {

    public int tableNumber;
    public int tableType;
    public int fee;
    public int winnerCoin;
    public Text entryCoin;
    public Text onlineUser;
    public Text roomName;
    public Text winningCoin;
    public SocketIOComponent socketIO;
    public GameObject twoPlayerTitle;
    public GameObject FourPlayerTitle;
    public GameObject privatePlayerTitle;


    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }
    

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponent<Button>().onClick.AddListener(startGame);
    }
    




    public void startGame()
    {
        Dictionary<string, string> roomInfo = new Dictionary<string, string>();

        print("winnercoin ---" + winnerCoin);

        roomInfo.Add("id", "" + tableNumber);
        roomInfo.Add("type", "" + tableType);
        roomInfo.Add("entryCoin", "" + fee);
        roomInfo.Add("winnerCoin", "" + winnerCoin);
        int adminCoin = fee * tableType - winnerCoin;

        

        GameManager.Instance.JoinedByID = false;
        GameManager.Instance.GameScene = "GameScene";
        if(GameManager.Instance.type == MyGameType.FourPlayer)
        {
            GameManager.Instance.requiredPlayers = 3;
            int secondCoin = (fee * tableType - winnerCoin) / 3 * 2;
            roomInfo.Add("secondWinnerCoin" , "" + secondCoin);
            adminCoin -= secondCoin;
            
        }
        else
        {
            GameManager.Instance.requiredPlayers = 1;
        }

        roomInfo.Add("adminCoin", "" + adminCoin);
        GameManager.Instance.myPlayerData.room_data = roomInfo;
        Dictionary<string, string> data = new Dictionary<string, string>();

        data.Add("room_id", "" + tableNumber);
        data.Add("nickname",  GameManager.Instance.myPlayerData.GetPlayerId());
        data.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
        data.Add("member", "" + GameManager.Instance.myPlayerData.GetRoomType());
        JSONObject jsonData = new JSONObject(data);

        socketIO.Emit("ENTER_ROOM", jsonData);
        

    }
}
