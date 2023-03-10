using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;
using AssemblyCSharp;

public class ChatShopController : MonoBehaviour
{


    public GameObject priceText;
    public GameObject chatName;
    public GameObject button;
    public GameObject buttonText;
    private int price;
    private int index;
    public GameObject[] bubbles;

    // Use this for initialization
    void Start()
    {


    }

    public void fillData(int i)
    {
        this.index = i;
        string[] messages = StaticStrings.chatMessagesExtended[i];
        int price = StaticStrings.chatPrices[i];
        string name = StaticStrings.chatNames[i];
        this.price = price;
        priceText.GetComponent<Text>().text = price.ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
        chatName.GetComponent<Text>().text = name;

        for (int j = 0; j < messages.Length; j++)
        {
            bubbles[j].transform.GetChild(0).GetComponent<Text>().text = messages[j];
            bubbles[j].SetActive(true);
        }

        for (int j = 5; j >= messages.Length; j--)
        {
            bubbles[j].SetActive(false);
        }



        if (GameManager.Instance.myPlayerData.GetChats() != null &&
            GameManager.Instance.myPlayerData.GetChats().Length > 0 && GameManager.Instance.myPlayerData.GetChats().Contains("'" + i + "'"))
        {
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buyChat()
    {
        if (GameManager.Instance.myPlayerData.GetCoins() >= this.price)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(-this.price);
            GameManager.Instance.playfabManager.updateBoughtChats(this.index);
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }
    }

    public void buyEmoji()
    {
        if (GameManager.Instance.myPlayerData.GetCoins() >= this.price)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(-this.price);
            GameManager.Instance.playfabManager.UpdateBoughtEmojis(this.index);
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
        else
        {
            GameManager.Instance.dialog.SetActive(true);
        }
    }
}
