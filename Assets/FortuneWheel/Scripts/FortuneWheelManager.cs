using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.Events;
using System.Collections.Generic;
using AssemblyCSharp;
using SocketIO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FortuneWheelManager : MonoBehaviour
{
    public GameObject FreeTurnIndicator;
    [HideInInspector]
    public Text timeToFreeText;
    public GameObject TimeToFreeTurnIndicator;

    [Header("Game Objects for some elements")]
    public Button PaidTurnButton;               // This button is showed when you can turn the wheel for coins
    public Button FreeTurnButton;               // This button is showed when you can turn the wheel for free
    public GameObject Circle;                   // Rotatable GameObject on scene with reward objects
    public Text DeltaCoinsText;                 // Pop-up text with wasted or rewarded coins amount
    public Text CurrentCoinsText;               // Pop-up text with wasted or rewarded coins amount
    public GameObject NextTurnTimerWrapper;
    public Text NextFreeTurnTimerText;          // Text element that contains remaining time to next free turn

    [Header("How much currency one paid turn costs")]
    public int TurnCost = 300;                  // How much coins user waste when turn whe wheel

    private bool _isStarted;                    // Flag that the wheel is spinning

    [Header("Params for each sector")]
    public FortuneWheelSector[] Sectors;        // All sectors objects

    private float _finalAngle;                  // The final angle is needed to calculate the reward
    private float _startAngle;                  // The first time start angle equals 0 but the next time it equals the last final angle
    private float _currentLerpRotationTime;     // Needed for spinning animation
    private int _currentCoinsAmount = 1000;     // Started coins amount. In your project it should be picked up from CoinsManager or from PlayerPrefs and so on
    private int _previousCoinsAmount;

    // Here you can set time between two free turns
    [Header("Time Between Two Free Turns")]
    public int TimerMaxHours;
    [RangeAttribute(0, 59)]
    public int TimerMaxMinutes;
    [RangeAttribute(0, 59)]
    public int TimerMaxSeconds = 10;

    // Remaining time to next free turn
    private int _timerRemainingHours;
    private int _timerRemainingMinutes;
    private int _timerRemainingSeconds;

    private DateTime _nextFreeTurnTime;

    // Key name for storing in PlayerPrefs
    private const string LAST_FREE_TURN_TIME_NAME = "LastFreeTurnTimeTicks";

    // Set TRUE if you want to let players to turn the wheel for coins while free turn is not available yet
    [Header("Can players turn the wheel for currency?")]
    public bool IsPaidTurnEnabled = true;

    // Set TRUE if you want to let players to turn the wheel for FREE from time to time
    [Header("Can players turn the wheel for FREE from time to time?")]
    public bool IsFreeTurnEnabled = true;

    // Flag that player can turn the wheel for free right now
    private bool _isFreeTurnAvailable;

    private FortuneWheelSector _finalSector;
    private SocketIOComponent socketIO;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    /// 





    void Start()
    {
        timeToFreeText = TimeToFreeTurnIndicator.GetComponent<Text>();
    }

    private void Awake()
    {


        Debug.Log("Fortune wheel awake");

        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
        PlayerPrefs.SetString(LAST_FREE_TURN_TIME_NAME, GameManager.Instance.myPlayerData.GetLastFortuneTime());

        _previousCoinsAmount = _currentCoinsAmount;
        // Show our current coins amount
        CurrentCoinsText.text = _currentCoinsAmount.ToString();

        // Show sector reward value in text object if it's set
        foreach (var sector in Sectors)
        {
            if (sector.ValueTextObject != null)
                sector.ValueTextObject.GetComponent<Text>().text = sector.RewardValue.ToString();
        }

        if (IsFreeTurnEnabled)
        {
            // Start our timer to next free turn
            SetNextFreeTime();

            if (!PlayerPrefs.HasKey(LAST_FREE_TURN_TIME_NAME))
            {
                PlayerPrefs.SetString(LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString());
            }
        }
        else
        {
            NextTurnTimerWrapper.gameObject.SetActive(false);
        }


    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        DeltaCoinsText.gameObject.SetActive(false);
    }

    private void TurnWheelForFree() { TurnWheel(true); }
    private void TurnWheelForCoins() { TurnWheel(false); }

    private void TurnWheel(bool isFree)
    {
        Debug.Log("turn wheel");

        _currentLerpRotationTime = 0f;

        // All sectors angles
        int[] sectorsAngles = new int[Sectors.Length];

        // Fill the necessary angles (for example if we want to have 12 sectors we need to fill the angles with 30 degrees step)
        // It's recommended to use the EVEN sectors count (2, 4, 6, 8, 10, 12, etc)
        for (int i = 1; i <= Sectors.Length; i++)
        {
            sectorsAngles[i - 1] = 360 / Sectors.Length * i;
        }

        //int cumulativeProbability = Sectors.Sum(sector => sector.Probability);

        double rndNumber = UnityEngine.Random.Range(1, Sectors.Sum(sector => sector.Probability));

        // Calculate the propability of each sector with respect to other sectors
        int cumulativeProbability = 0;
        // Random final sector accordingly to probability
        int randomFinalAngle = sectorsAngles[0];
        _finalSector = Sectors[0];

        for (int i = 0; i < Sectors.Length; i++)
        {
            cumulativeProbability += Sectors[i].Probability;

            if (rndNumber <= cumulativeProbability)
            {
                // Choose final sector
                randomFinalAngle = sectorsAngles[i];
                _finalSector = Sectors[i];
                break;
            }
        }

        int fullTurnovers = 5;

        // Set up how many turnovers our wheel should make before stop
        _finalAngle = fullTurnovers * 360 + randomFinalAngle;

        // Stop the wheel
        _isStarted = true;

        _previousCoinsAmount = _currentCoinsAmount;

        // Decrease money for the turn if it is not free turn
        if (!isFree)
        {
            _currentCoinsAmount -= TurnCost;

            // Show wasted coins
            DeltaCoinsText.text = String.Format("-{0}", TurnCost);

            DeltaCoinsText.gameObject.SetActive(true);

            // Animations for coins
            StartCoroutine(HideCoinsDelta());
            StartCoroutine(UpdateCoinsAmount());
        }
        else
        {
            // At this step you can save current time value to your server database as last used free turn
            // We can't save long int to PlayerPrefs so store this value as string and convert to long later
            PlayerPrefs.SetString(LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString());
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(MyPlayerData.FortuneWheelLastFreeKey, DateTime.Now.Ticks.ToString());
            /*GameManager.Instance.myPlayerData.UpdateUserData(data);*/

            // Restart timer to next free turn
            SetNextFreeTime();
        }
    }

    public void TurnWheelButtonClick()
    {
        if (_isFreeTurnAvailable)
        {
            TurnWheelForFree();
        }
        else
        {
            // If we have enabled paid turns
            if (IsPaidTurnEnabled)
            {
                // If player have enough coins
                if (GameManager.Instance.myPlayerData.GetCoins() >= TurnCost)
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    GameManager.Instance.myPlayerData.user_data[MyPlayerData.CoinsKey] = (GameManager.Instance.myPlayerData.GetCoins() - TurnCost).ToString();
                    JSONObject jsonData = new JSONObject(GameManager.Instance.myPlayerData.user_data);

                    socketIO.Emit("UPDATE_USER_INFO", jsonData);
                    TurnWheelForCoins();
                }
            }
        }
    }



    public void SetNextFreeTime()
    {

        Debug.Log("Next free turn");

        // Reset the remaining time values
        _timerRemainingHours = TimerMaxHours;
        _timerRemainingMinutes = TimerMaxMinutes;
        _timerRemainingSeconds = TimerMaxSeconds;

        // Get last free turn time value from storage
        // We can't save long int to PlayerPrefs so store this value as string and convert to long
        _nextFreeTurnTime = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(LAST_FREE_TURN_TIME_NAME, DateTime.Now.Ticks.ToString())))
            .AddHours(TimerMaxHours)
            .AddMinutes(TimerMaxMinutes)
            .AddSeconds(TimerMaxSeconds);

        _isFreeTurnAvailable = false;

        int miliSeconds = (TimerMaxHours * 3600000) + (TimerMaxMinutes * 60000) + (TimerMaxSeconds * 1000);


        LocalNotification.CancelNotification(1);

        if (PlayerPrefs.GetInt(StaticStrings.NotificationsKey, 0) == 0)
        {
            Debug.Log("Start notification");
            LocalNotification.SendNotification(1, miliSeconds, StaticStrings.notificationTitle, StaticStrings.notificationMessage, new Color32(0xff, 0x44, 0x44, 255), true, true, true, "app_icon");
        }
        else
        {
            Debug.Log("Notification disabled");
        }

    }

    private void ShowTurnButtons()
    {
        if (_isFreeTurnAvailable)               // If have free turn
        {
            ShowFreeTurnButton();
            EnableFreeTurnButton();

        }
        else
        {                               // If haven't free turn

            if (!IsPaidTurnEnabled)         // If our settings allow only free turns
            {
                ShowFreeTurnButton();
                DisableFreeTurnButton();        // Make button inactive while spinning or timer to next free turn

            }
            else
            {                           // If player can turn for coins
                ShowPaidTurnButton();

                if (_isStarted || GameManager.Instance.myPlayerData.GetCoins() < TurnCost)
                    DisablePaidTurnButton();    // Make button non interactable if user has not enough money for the turn of if wheel is turning right now
                else
                    EnablePaidTurnButton(); // Can make paid turn right now
            }
        }
    }

    private void Update()
    {

        // We need to show TURN FOR FREE button or TURN FOR COINS button
        ShowTurnButtons();

        // Show timer only if we enable free turns
        if (IsFreeTurnEnabled)
            UpdateFreeTurnTimer();

        if (!_isStarted)
            return;

        // Animation time
        float maxLerpRotationTime = 4f;

        // increment animation timer once per frame
        _currentLerpRotationTime += Time.deltaTime;

        // If the end of animation
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            _startAngle = _finalAngle % 360;

            //GiveAwardByAngle ();
            _finalSector.RewardCallback.Invoke();

            StartCoroutine(HideCoinsDelta());
        }
        else
        {
            // Calculate current position using linear interpolation
            float t = _currentLerpRotationTime / maxLerpRotationTime;

            // This formulae allows to speed up at start and speed down at the end of rotation.
            // Try to change this values to customize the speed
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
            Circle.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    /// <summary>
    /// Sample callback for giving reward (in editor each sector have Reward Callback field pointed to this method)
    /// </summary>
    /// <param name="awardCoins">Coins for user</param>
    public void RewardCoins(int awardCoins)
    {

        _currentCoinsAmount += awardCoins;
        // Show animated delta coins
        DeltaCoinsText.text = String.Format("+{0}", awardCoins);
        DeltaCoinsText.gameObject.SetActive(true);
        StartCoroutine(UpdateCoinsAmount());


        GameManager.Instance.myPlayerData.user_data[MyPlayerData.CoinsKey] = (GameManager.Instance.myPlayerData.GetCoins() + awardCoins).ToString();
        JSONObject jsonData = new JSONObject(GameManager.Instance.myPlayerData.user_data);

        socketIO.Emit("UPDATE_USER_INFO", jsonData);
    }

    // Hide coins delta text after animation
    private IEnumerator HideCoinsDelta()
    {
        yield return new WaitForSeconds(1f);
        DeltaCoinsText.gameObject.SetActive(false);
    }

    // Animation for smooth increasing and decreasing the number of coins
    private IEnumerator UpdateCoinsAmount()
    {
        const float seconds = 0.5f; // Animation duration
        float elapsedTime = 0;

        while (elapsedTime < seconds)
        {
            CurrentCoinsText.text = Mathf.Floor(Mathf.Lerp(_previousCoinsAmount, _currentCoinsAmount, (elapsedTime / seconds))).ToString();
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _previousCoinsAmount = _currentCoinsAmount;

        CurrentCoinsText.text = _currentCoinsAmount.ToString();
    }

    // Change remaining time to next free turn every 1 second
    private void UpdateFreeTurnTimer()
    {
        // Don't count the time if we have free turn already
        if (_isFreeTurnAvailable)
            return;

        // Update the remaining time values
        _timerRemainingHours = (int)(_nextFreeTurnTime - DateTime.Now).Hours;
        _timerRemainingMinutes = (int)(_nextFreeTurnTime - DateTime.Now).Minutes;
        _timerRemainingSeconds = (int)(_nextFreeTurnTime - DateTime.Now).Seconds;

        // If the timer has ended
        if (_timerRemainingHours <= 0 && _timerRemainingMinutes <= 0 && _timerRemainingSeconds <= 0)
        {
            FreeTurnIndicator.SetActive(true);
            TimeToFreeTurnIndicator.SetActive(false);
            NextFreeTurnTimerText.text = "Ready!";
            // Now we have a free turn
            _isFreeTurnAvailable = true;
        }
        else
        {
            FreeTurnIndicator.SetActive(false);
            // Show the remaining time
            TimeToFreeTurnIndicator.SetActive(true);
            NextFreeTurnTimerText.text = String.Format("{0:00}:{1:00}:{2:00}", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
            timeToFreeText.text = String.Format("{0:00}:{1:00}:{2:00}", _timerRemainingHours, _timerRemainingMinutes, _timerRemainingSeconds);
            // We don't have a free turn yet
            _isFreeTurnAvailable = false;
        }
    }



    private void EnableButton(Button button)
    {
        button.interactable = true;
        button.GetComponent<Image>().color = new Color(255, 255, 255, 1f);
    }

    private void DisableButton(Button button)
    {
        button.interactable = false;
        button.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
    }

    // Function for more readable calls
    private void EnableFreeTurnButton() { EnableButton(FreeTurnButton); }
    private void DisableFreeTurnButton() { DisableButton(FreeTurnButton); }
    private void EnablePaidTurnButton() { EnableButton(PaidTurnButton); }
    private void DisablePaidTurnButton() { DisableButton(PaidTurnButton); }

    private void ShowFreeTurnButton()
    {
        FreeTurnButton.gameObject.SetActive(true);
        /* FreeTurnButton.gameObject.transform.position = new Vector3(FreeTurnButton.gameObject.transform.position.x,
                                 FreeTurnButton.gameObject.transform.position.y, FreeTurnButton.gameObject.transform.position.z + 1);*/
        //PaidTurnButton.gameObject.SetActive(false);
    }

    private void ShowPaidTurnButton()
    {
        PaidTurnButton.gameObject.SetActive(true);
        FreeTurnButton.gameObject.SetActive(false);
    }

    public void ResetTimer()
    {
        PlayerPrefs.DeleteKey(LAST_FREE_TURN_TIME_NAME);
    }
}

/**
 * One sector on the wheel
 */
[Serializable]
public class FortuneWheelSector : System.Object
{
    [Tooltip("Text object where value will be placed (not required)")]
    public GameObject ValueTextObject;

    [Tooltip("Value of reward")]
    public string RewardValue = "100";

    [Tooltip("Chance that this sector will be randomly selected")]
    [RangeAttribute(0, 100)]
    public int Probability = 100;

    [Tooltip("Method that will be invoked if this sector will be randomly selected")]
    public UnityEvent RewardCallback;
}

/**
 * Draw custom button in inspector
 */
#if UNITY_EDITOR
[CustomEditor(typeof(FortuneWheelManager))]
public class FortuneWheelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FortuneWheelManager myScript = (FortuneWheelManager)target;
        if (GUILayout.Button("Reset Timer"))
        {
            myScript.ResetTimer();
        }
    }
}
#endif