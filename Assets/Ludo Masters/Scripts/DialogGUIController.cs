using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogGUIController : MonoBehaviour
{
    public static DialogGUIController instance = null;
    public GameObject Other;
    // Use this for initialization
    void Awake()
    {
        // DontDestroyOnLoad(gameObject);
        // if (FindObjectsOfType(GetType()).Length > 1)
        // {
        //     Destroy(gameObject);
        // }
        //Check if instance already exists
        if (instance == null)
        {

            //if not, set instance to this
            instance = this;
           // Other.GetComponent<AdMobObjectController>().Init();
            //If instance already exists and it's not this:
        }
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);


    }

    // Update is called once per frame
    void Update()
    {

    }
}
