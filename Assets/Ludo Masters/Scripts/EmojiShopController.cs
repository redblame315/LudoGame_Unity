using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

public class EmojiShopController : MonoBehaviour
{

    public GameObject priceText;
    public GameObject chatName;
    public GameObject button;
    public GameObject buttonText;
    private int price;
    private int index;
    public GameObject[] bubbles;
    public GameObject parent;
    public GameObject emojiPrefab;
    Sprite[] emojiSprites;
    int emojiPerPack;
    int packsCount;
    // Use this for initialization
    void Start()
    {

    }

    public void fillData(int i)
    {
        emojiSprites = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emoji;
        emojiPerPack = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().emojiPerPack;
        packsCount = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().packsCount;

        this.index = i;

        int price = StaticStrings.emojisPrices[i];
        this.price = price;
        priceText.GetComponent<Text>().text = price.ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');


        for (int j = 0; j < emojiPerPack; j++)
        {
            GameObject emoji = Instantiate(emojiPrefab);
            emoji.transform.SetParent(parent.transform, false);
            emoji.GetComponent<Image>().sprite = emojiSprites[(i + 1) * emojiPerPack + j];
        }



        if (GameManager.Instance.myPlayerData.GetEmoji() != null &&
            GameManager.Instance.myPlayerData.GetEmoji().Length > 0 && GameManager.Instance.myPlayerData.GetEmoji().Contains("'" + i + "'"))
        {
            button.GetComponent<Button>().interactable = false;
            buttonText.GetComponent<Text>().text = "Owned";
        }
    }

    // Update is called once per frame
    void Update()
    {

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

