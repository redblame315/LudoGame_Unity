using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinishWindowController : MonoBehaviour
{

    public GameObject Window;
    public GameObject[] AvatarsMain;
    public GameObject[] AvatarsImage;
    public GameObject[] Names;
    public GameObject[] Backgrounds;
    public GameObject[] PrizeMainObjects;
    public GameObject[] prizeText;
    public GameObject[] placeIndicators;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < AvatarsMain.Length; i++)
        {
            AvatarsMain[i].SetActive(false);
        }

    }

    public void showWindow(List<PlayerObject> playersFinished, List<PlayerObject> otherPlayers, int firstPlacePrize, int secondPlacePrize)
    {
        if (secondPlacePrize == 0)
        {
            PrizeMainObjects[1].SetActive(false);
        }

        prizeText[0].GetComponent<Text>().text = firstPlacePrize.ToString();
        prizeText[1].GetComponent<Text>().text = secondPlacePrize.ToString();

        Window.SetActive(true);
        for (int i = 0; i < playersFinished.Count; i++)
        {
            AvatarsMain[i].SetActive(true);
            AvatarsImage[i].GetComponent<Image>().sprite = playersFinished[i].avatar;
            Names[i].GetComponent<Text>().text = playersFinished[i].name;
            if (playersFinished[i].id.Equals(GameManager.Instance.myPlayerData.GetPlayerId()))
            {
                Backgrounds[i].SetActive(true);
            }
        }

        int counter = 0;
        for (int i = playersFinished.Count; i < playersFinished.Count + otherPlayers.Count; i++)
        {
            if (i == 1)
            {
                PrizeMainObjects[1].SetActive(false);
            }
            AvatarsMain[i].SetActive(true);
            AvatarsImage[i].GetComponent<Image>().sprite = otherPlayers[counter].avatar;
            Names[i].GetComponent<Text>().text = otherPlayers[counter].name;
            if (otherPlayers[counter].id.Equals(GameManager.Instance.myPlayerData.GetPlayerId()))
            {
                Backgrounds[i].SetActive(true);
            }
            if (otherPlayers.Count > 1)
                placeIndicators[i].SetActive(false);
            counter++;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
