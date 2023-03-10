using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SetMyData : MonoBehaviour
{

    public GameObject avatar;
    public GameObject name;
    public GameObject matchCanvas;
    public GameObject controlAvatars;
    public GameObject backButton;
    public GameObject bkButton;
    public GameObject gameConfig;


    // Use this for initialization
    public float timeLeft = 45.0f;
    public Text startText; // used for showing countdown from 60, 59, 58 ...
	public Text playerMsg;
	public Text timeCountMsg;

    void OnEnable()
    {
        timeLeft = 45.0f;
    }

    void Update()
    {
		if (GameManager.Instance.type == MyGameType.Private) {
			timeCountMsg.gameObject.SetActive (false);
			startText.gameObject.SetActive (false);
			playerMsg.text = "Please add players to start game";
			bkButton.SetActive (true);
		} else {
			timeCountMsg.gameObject.SetActive (true);
			startText.gameObject.SetActive (true);
			if (timeLeft < 0) {
				//Debug.Log("Is continiues workimg...?" + timeLeft);
				playerMsg.text = "All Players are Not Available, \n Please Come Back Again";
				bkButton.SetActive (true);
				if (timeLeft < -10 && GameManager.Instance.payoutCoins > 100) {
					gameObject.SetActive (false);
					//gameConfig.SetActive(false);
					//Debug.Log("Game value---" + GameManager.Instance.payoutCoins);
					/*PhotonNetwork.LeaveRoom ();*/
					timeLeft = 45.0f;
				} else if (timeLeft < -9 && GameManager.Instance.payoutCoins <= 100) {
					gameObject.SetActive (false);
					//gameConfig.SetActive(false);
					//Debug.Log("Game value---" + GameManager.Instance.payoutCoins);
					/*PhotonNetwork.LeaveRoom ();*/
					timeLeft = 45.0f;

				} else {
					timeLeft -= Time.deltaTime;
				}

			} else {
				timeLeft -= Time.deltaTime;
				startText.text = (timeLeft).ToString ("0") + " Sec";
				playerMsg.text = "Waiting for other players to join room!";
				bkButton.SetActive (false);
			}
		}
    }

    public void MatchPlayer()
    {

        //name.GetComponent<Text>().text = GameManager.Instance.nameMy;
        if (GameManager.Instance.avatarMy != null)
            avatar.GetComponent<Image>().sprite = GameManager.Instance.avatarMy;


        controlAvatars.GetComponent<ControlAvatars>().reset();

    }

    public void setBackButton(bool active)
    {
        backButton.SetActive(active);
    }
}
