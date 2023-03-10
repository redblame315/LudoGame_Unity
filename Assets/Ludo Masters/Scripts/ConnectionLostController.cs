using UnityEngine;
using System.Collections;

public class ConnectionLostController : MonoBehaviour {

    // Use this for initialization
    public GameObject canvas;
	private bool isQuiting;
	//public GameObject quitMessageGameObject;

    void Start() {
        DontDestroyOnLoad(transform.gameObject);
        GameManager.Instance.connectionLost = this;
/*        Debug.Log(Application.internetReachability);*/
/*        Debug.Log("----testing-----");*/
/*		Debug.Log (this.gameObject);*/
/*        Debug.Log(NetworkReachability.NotReachable);*/
        //if (Application.internetReachability == NetworkReachability.NotReachable) {
        //    showDialog();
        //}
    }

    // Update is called once per frame
	void Update () {
		if(isQuiting == true)
		{
			if(Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
			{
				Debug.Log("Escaped pressed times 02");
				Application.Quit();
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("Escaped pressed times 01");
			//quitMessageGameObject.SetActive(true);
			isQuiting = true;
			StartCoroutine(QuitingTimer());
		}
	}
	IEnumerator QuitingTimer()
	{
		yield return new WaitForSeconds(2);
		isQuiting = false;
		Debug.Log("No response");
		//quitMessageGameObject.SetActive(false);
	}

    public void destroy() {
        if (this.gameObject != null)
            DestroyImmediate(this.gameObject);
    }

    public void showDialog() {
        canvas.SetActive(true);
    }

    public void closeApp() {
        Application.Quit();
    }
}
