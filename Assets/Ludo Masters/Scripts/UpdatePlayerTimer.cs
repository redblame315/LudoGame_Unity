using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

public class UpdatePlayerTimer : MonoBehaviour
{
    private float playerTime;
    public GameObject timerObject;
    private Image timer;
    private bool timeSoundsStarted;
    public AudioSource[] audioSources;
    public GameObject GUIController;
    public bool myTimer;
    public bool paused = false;
	//public int missedChance = 1;

	//public GameObject modalPanelObject;
	//public Button closeBtn;
	//public Text textMsg;
    // Use this for initialization
    void Start()
    {
        timer = gameObject.GetComponent<Image>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        timer = gameObject.GetComponent<Image>();
    }

    public void Pause()
    {
        paused = true;
        audioSources[0].Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
            updateClock();
    }

    public void restartTimer()
    {
        paused = false;
        timer.fillAmount = 1.0f;
    }


    void OnDisable()
    {
        if (timer != null)
        {
            timer.fillAmount = 1.0f;
            paused = false;
            audioSources[0].Stop();
        }
    }

    private void updateClock()
    {
        float minus;

        playerTime = GameManager.Instance.playerTime;
        if (GameManager.Instance.offlineMode)
            playerTime = GameManager.Instance.playerTime + GameManager.Instance.cueTime;
        minus = 1.0f / playerTime * Time.deltaTime;

        timer.fillAmount -= minus;
        if (timer.fillAmount < 0.25f && !timeSoundsStarted)
        {
            audioSources[0].Play();
            timeSoundsStarted = true;
        }
        if (timer.fillAmount == 0)
        {

            Debug.Log("TIME 0");

            audioSources[0].Stop();
            GameManager.Instance.stopTimer = true;
            paused = true;
            if (!GameManager.Instance.offlineMode)
            {
                if (myTimer)
                {
                    Debug.Log("Timer call finish turn");
                    GUIController.GetComponent<GameGUIController>().SendFinishTurn();
                    StaticStrings.missedChance++;
                }
                //PhotonNetwork.RaiseEvent(9, null, true, null);
            }
            else
            {
                GameManager.Instance.wasFault = true;
                GameManager.Instance.cueController.setTurnOffline(true);
            }

			Debug.Log ("You run out of time.");
			//Debug.Log (missedChance);
			
            print("StaticStrings.missedChance ---- " + StaticStrings.missedChance);
			if ( StaticStrings.missedChance == 2) {
				//modalPanelObject.SetActive(true);
				//textMsg.text = "Please roll dice, this is your last chance.";
			}
			if ( StaticStrings.missedChance == 3) {//ju1026
				// Show game off message.
				//modalPanelObject.SetActive(false);
				//modalPanelObject.SetActive(true);
				//closeBtn.gameObject.SetActive(false);
				//textMsg.text = "Oop's your game has ended. Please play another game, Good Luck!";

				//ToDo
				// Make this comment live after testing...
				GUIController.GetComponent<GameGUIController> ().LeaveGame (true);

			}
            //showMessage("You " + StaticStrings.runOutOfTime);

            /*if (!GameManager.Instance.offlineMode)
            {
                GameManager.Instance.cueController.setOpponentTurn();
            }*/

        }


    }
}
