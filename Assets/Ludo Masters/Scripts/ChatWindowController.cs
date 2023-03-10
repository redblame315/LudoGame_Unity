using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;



public class ChatWindowController : MonoBehaviour
{

    public GameObject gridView;
    public GameObject horizontalEmojiView;
    public GameObject ChatMessageButtonPrefab;
    public GameObject ChatEmojiButtonPrefab;
    public GameObject ChatButton;
    public GameObject chatWindow;
    public GameObject myChatBubble;
    public GameObject myChatBubbleText;
    public GameObject myChatBubbleImage;
    [HideInInspector]
    public Sprite[] emojiSprites;
    private int emojiPerPack;
    private int packsCount = 6;
    // Use this for initialization


    private SocketIOComponent socketIO;



    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }



    void Start()
    {

        emojiSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emoji;
        emojiPerPack = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emojiPerPack;
        packsCount = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().packsCount;

        // Add text messages
        for (int i = 0; i < StaticStrings.chatMessages.Length; i++)
        {
            GameObject button = Instantiate(ChatMessageButtonPrefab);
            button.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.chatMessages[i];
            button.transform.parent = gridView.transform;
            button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            string index = StaticStrings.chatMessages[i];
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => SendMessageEvent(index));
        }

        for (int j = 0; j < packsCount; j++)
        {

            if (j == 0 || (GameManager.Instance.myPlayerData.GetEmoji() != null && GameManager.Instance.myPlayerData.GetEmoji().Contains("'" + (j - 1) + "'")))
            {
                // Add emoji message
                for (int i = 0; i < emojiPerPack; i++)
                {
                    GameObject button = Instantiate(ChatEmojiButtonPrefab);
                    button.transform.GetComponent<Image>().sprite = emojiSprites[j * emojiPerPack + i];
                    button.transform.parent = horizontalEmojiView.transform;
                    button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    int index = j * emojiPerPack + i;
                    button.GetComponent<Button>().onClick.RemoveAllListeners();
                    button.GetComponent<Button>().onClick.AddListener(() => SendMessageEventEmoji(index));
                }
            }
        }



        for (int i = 0; i < StaticStrings.chatMessagesExtended.Length; i++)
        {
            if (GameManager.Instance.myPlayerData.GetChats() != null && GameManager.Instance.myPlayerData.GetChats().Contains("'" + i + "'"))
            {
                for (int j = 0; j < StaticStrings.chatMessagesExtended[i].Length; j++)
                {
                    GameObject button = Instantiate(ChatMessageButtonPrefab);
                    button.transform.GetChild(0).GetComponent<Text>().text = StaticStrings.chatMessagesExtended[i][j];
                    button.transform.parent = gridView.transform;
                    button.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    string index = StaticStrings.chatMessagesExtended[i][j];
                    button.GetComponent<Button>().onClick.RemoveAllListeners();
                    button.GetComponent<Button>().onClick.AddListener(() => SendMessageEvent(index));
                }
            }

        }
    }

    public void SendMessageEvent(string index)
    {
        Debug.Log("Button Clicked " + index);
        if (!GameManager.Instance.offlineMode)
        {
            Dictionary<string, string> dicData = new Dictionary<string, string>();

            dicData.Add("index", index);
            dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
            dicData.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());
            JSONObject jsonData = new JSONObject(dicData);

            socketIO.Emit("CHAT_SEND_EVENT", jsonData);
        }
        //PhotonNetwork.RaiseEvent((int)EnumPhoton.SendChatMessage, index + ";" + PhotonNetwork.playerName, true, null);

        chatWindow.SetActive(false);

        ChatButton.GetComponent<Text>().text = "CHAT";
        myChatBubbleImage.SetActive(false);
        myChatBubbleText.SetActive(true);
        myChatBubbleText.GetComponent<Text>().text = index;
        myChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");


        //ChatMessagesObject.GetComponent<Animator>().Play("hideMessageDialog");
        //messageDialogVisible = false;

        // if (isGameScene)
        // {
        //     myMessageBubble.SetActive(true);
        //     myMessageBubble.transform.GetChild(0).GetComponent<Text>().text = index;
        //     if (isGameScene)
        //     {
        //         CancelInvoke("hideMyMessageBubble");
        //         Invoke("hideMyMessageBubble", 6.0f);
        //     }
        // }

    }

    public void SendMessageEventEmoji(int index)
    {
        Debug.Log("Button Clicked " + index);

        if (!GameManager.Instance.offlineMode)
        {
            //PhotonNetwork.RaiseEvent((int)EnumPhoton.SendChatEmojiMessage, index + ";" + PhotonNetwork.playerName, true, null);
            Dictionary<string, string> dicData = new Dictionary<string, string>();

            dicData.Add("index", "" + index);
            dicData.Add("user_id", GameManager.Instance.myPlayerData.GetPlayerId());
            dicData.Add("room_id", GameManager.Instance.myPlayerData.GetLoginRoomId());
            JSONObject jsonData = new JSONObject(dicData);

            socketIO.Emit("CHAT_EMOJI_SEND_EVENT", jsonData);
        }

        chatWindow.SetActive(false);

        ChatButton.GetComponent<Text>().text = "CHAT";
        myChatBubbleImage.SetActive(true);
        myChatBubbleText.SetActive(false);
        myChatBubbleImage.GetComponent<Image>().sprite = emojiSprites[index];
        myChatBubble.GetComponent<Animator>().Play("MessageBubbleAnimation");


        //ChatMessagesObject.GetComponent<Animator>().Play("hideMessageDialog");
        //messageDialogVisible = false;

        // if (isGameScene)
        // {
        //     myMessageBubble.SetActive(true);
        //     myMessageBubble.transform.GetChild(0).GetComponent<Text>().text = index;
        //     if (isGameScene)
        //     {
        //         CancelInvoke("hideMyMessageBubble");
        //         Invoke("hideMyMessageBubble", 6.0f);
        //     }
        // }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
