using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AssemblyCSharp;

public class StartScriptController : MonoBehaviour
{

    public GameObject splashCanvas;

    public GameObject menuCanvas;
    public GameObject[] go;

    public GameObject fbButton;

    public GameObject fbLoginCoinText;
    public GameObject guestLoginCoinText;
    public GameObject loginSelectCanvas;

    // Use this for initialization
    void Start()
    {
        fbLoginCoinText.GetComponent<Text>().text = StaticStrings.initCoinsCountFacebook.ToString();
        guestLoginCoinText.GetComponent<Text>().text = StaticStrings.initCoinsCountGuest.ToString();
        //         Debug.Log("START SCRIPT");
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey("LoggedType"))
        {
            splashCanvas.SetActive(true);
        }
        else
        {
            loginSelectCanvas.SetActive(true);
        }

#if UNITY_WEBGL
        fbButton.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideAllElements()
    {
        menuCanvas.SetActive(true);
    }

    public void QuitGameNow()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


}
