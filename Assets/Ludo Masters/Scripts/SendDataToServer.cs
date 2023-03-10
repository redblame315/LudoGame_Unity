using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class SendDataToServer : MonoBehaviour {
    void Start()
{
        //Debug.Log("Web server testing...");
        StartCoroutine(Upload());
}

IEnumerator Upload()
{
    WWWForm form = new WWWForm();
    form.AddField("myField", "myData");

    UnityWebRequest www = UnityWebRequest.Post("http://varchartech.com/ludodata/ludoserver.php", form);
    yield return www;

    if (www.isNetworkError || www.isHttpError)
    {
        Debug.Log(www.error);
    }
    else
    {
        //Debug.Log("Form upload complete!");
        //Debug.Log(www);
    }
}
}
