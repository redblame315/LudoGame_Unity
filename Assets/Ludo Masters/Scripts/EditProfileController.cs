using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class EditProfileController : MonoBehaviour
{

    public GameObject changeName;
    public GameObject gridView;
    public GameObject buttonPrefab;

    private string avatarIndex;

    public GameObject PlayerNameMain;
    public GameObject PlayerAvatarMain;
    private SocketIOComponent socketIO;
    private StaticGameVariablesController staticController;

    private List<GameObject> buttons = new List<GameObject>();
    // Use this for initialization






    private void Awake()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
    }


    void Start()
    {

        avatarIndex = GameManager.Instance.myPlayerData.GetAvatarIndex();

        staticController = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>();
        changeName.GetComponent<InputField>().text = GameManager.Instance.nameMy;

        if (GameManager.Instance.facebookAvatar != null)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.GetComponent<ProfilePictureController>().picture.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
            button.transform.SetParent(gridView.transform, false);

            GameObject border = button.GetComponent<ProfilePictureController>().frame;
            if (GameManager.Instance.myPlayerData.GetAvatarIndex().Equals("fb"))
            {
                border.GetComponent<Image>().color = Color.green;
            }

            string index = "fb";
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => ClickButton(index, border));

            buttons.Add(border);
        }



        for (int i = 0; i < staticController.avatars.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab);
            button.GetComponent<ProfilePictureController>().picture.GetComponent<Image>().sprite = staticController.avatars[i];
            button.transform.SetParent(gridView.transform, false);

            GameObject border = button.GetComponent<ProfilePictureController>().frame;
            if (GameManager.Instance.myPlayerData.GetAvatarIndex().Equals(i + ""))
            {
                border.GetComponent<Image>().color = Color.green;
            }

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

    public void Save()
    {



        GameManager.Instance.myPlayerData.user_data[MyPlayerData.AvatarIndexKey] = avatarIndex;
        GameManager.Instance.myPlayerData.user_data[MyPlayerData.PlayerName] = changeName.GetComponent<InputField>().text;

        JSONObject jsonData = new JSONObject(GameManager.Instance.myPlayerData.user_data);

        socketIO.Emit("UPDATE_USER_INFO", jsonData);



        PlayerNameMain.GetComponent<Text>().text = changeName.GetComponent<InputField>().text;
        GameManager.Instance.nameMy = changeName.GetComponent<InputField>().text;


        if (avatarIndex.Equals("fb"))
        {
            PlayerAvatarMain.GetComponent<Image>().sprite = GameManager.Instance.facebookAvatar;
            GameManager.Instance.avatarMy = GameManager.Instance.facebookAvatar;
        }
        else
        {
            PlayerAvatarMain.GetComponent<Image>().sprite = staticController.avatars[int.Parse(avatarIndex)];
            GameManager.Instance.avatarMy = staticController.avatars[int.Parse(avatarIndex)];
        }



        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
