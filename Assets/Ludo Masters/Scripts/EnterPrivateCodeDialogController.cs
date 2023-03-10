using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class EnterPrivateCodeDialogController : MonoBehaviour
{

    public GameObject inputField;
    public GameObject confirmationText;
    public GameObject joinButton;
    private Button join;
    private InputField field;
    public GameObject GameConfiguration;
    public GameObject failedDialog;
    private SocketIOComponent socketIO;


    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }



    void OnEnable()
    {
        if (field != null)
            field.text = "";
        if (confirmationText != null)
            confirmationText.SetActive(false);
        if (join != null)
            join.interactable = false;
    }

    // Use this for initialization
    void Start()
    {
        field = inputField.GetComponent<InputField>();
        join = joinButton.GetComponent<Button>();
        join.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onValueChanged()
    {

        if (field.text.Length < 8)
        {
            confirmationText.SetActive(true);
            join.interactable = false;
        }
        else
        {
            confirmationText.SetActive(false);
            join.interactable = true;
        }
    }

    public void JoinByRoomID()
    {
        print("11111--- " + GameManager.Instance.myPlayerData.GetCoins());
        //print("2222--- " + GameManager.Instance.myPlayerData.GetEntryCoin());
        //if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.myPlayerData.GetEntryCoin())
        //{
        //    return;
        //}

        GameManager.Instance.JoinedByID = true;
        GameManager.Instance.payoutCoins = 0;
        string roomID = field.text;

        if (GameManager.Instance.myPlayerData.user_data.ContainsKey("roomId"))
        {
            GameManager.Instance.myPlayerData.user_data["roomId"] = roomID;
        }
        else
        {
            GameManager.Instance.myPlayerData.user_data.Add("roomId", roomID);
        }
        Dictionary<string, string> dicData = new Dictionary<string, string>();

        dicData.Add("room_id", roomID);
        dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
        JSONObject jsonData = new JSONObject(dicData);
        Debug.Log("SEND TO SERVER PRIVATE ROOM ID :" + roomID);
        socketIO.Emit("JOIN_PRIVATE_ROOM", jsonData);
        Debug.Log("SEND TO SERVER PRIVATE ROOM ID :" + roomID);
        /*RoomInfo[] rooms = PhotonNetwork.GetRoomList();*/

        /*Debug.Log("Rooms count: " + rooms.Length);

        if (rooms.Length == 0)
        {
            Debug.Log("no rooms!");
            failedDialog.SetActive(true);
        }
        else
        {
            bool foundRoom = false;
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].Name.Equals(roomID))
                {
                    foundRoom = true;
                    if (rooms[i].CustomProperties.ContainsKey("pc"))
                    {
                        GameManager.Instance.payoutCoins = int.Parse(rooms[i].CustomProperties["pc"].ToString());

                        if (GameManager.Instance.myPlayerData.GetCoins() >= GameManager.Instance.payoutCoins)
                        {
                            PhotonNetwork.JoinRoom(roomID);
                        }
                        GameConfiguration.GetComponent<GameConfigrationController>().startGame();
                    }
                    else
                    {
                        GameManager.Instance.payoutCoins = int.MaxValue;
                        GameConfiguration.GetComponent<GameConfigrationController>().startGame();
                    }
                }
            }
            if (!foundRoom)
            {
                failedDialog.SetActive(true);
            }
        }*/

    }
}
