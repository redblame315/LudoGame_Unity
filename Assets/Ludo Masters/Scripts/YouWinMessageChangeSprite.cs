using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AssemblyCSharp;

public class YouWinMessageChangeSprite : MonoBehaviour
{

    public Sprite other;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeSprite()
    {
        GetComponent<Image>().sprite = other;
    }

    public void loadWinnerScene()
    {
        if (GameManager.Instance.offlineMode)
        {
            GameManager.Instance.playfabManager.roomOwner = false;
            GameManager.Instance.roomOwner = false;
            GameManager.Instance.resetAllData();

            print("POINT --- 10");
            SceneManager.LoadScene("MenuScene");
            /*PhotonNetwork.BackgroundTimeout = StaticStrings.photonDisconnectTimeoutLong; ;*/
            // if (GameManager.Instance.offlineMode && StaticStrings.showAdWhenLeaveGame)
            //     AdsManager.Instance.adsScript.ShowAd();

        }
        else
        {
            SceneManager.LoadScene("WinnerScene");
        }

    }
}
