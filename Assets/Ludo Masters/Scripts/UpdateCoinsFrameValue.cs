using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
public class UpdateCoinsFrameValue : MonoBehaviour
{
    
    private int currentValue = 0;
    private Text text;
    private SocketIOComponent socketIO;
    public GameObject blockWindow;

    // Use this for initialization
    void Start()
    {
        socketIO = GameObject.Find("gameSocketIO").gameObject.GetComponent<SocketIOComponent>();
        text = GetComponent<Text>();
        InvokeRepeating("CheckAndUpdateValue", 0.2f, 0.2f);
        
    }
    

    private void CheckAndUpdateValue()
    {
        //Debug.Log("My data" + GameManager.Instance.myPlayerData);
        if (currentValue != GameManager.Instance.myPlayerData.GetCoins())
        {
            currentValue = GameManager.Instance.myPlayerData.GetCoins();
            if (currentValue != 0)
            {
                text.text = GameManager.Instance.myPlayerData.GetCoins().ToString("0,0", CultureInfo.InvariantCulture).Replace(',', ' ');
            }
            else
            {
                text.text = "0";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
