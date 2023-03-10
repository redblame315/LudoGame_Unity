using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEmojiBubbles : MonoBehaviour
{

    public GameObject prefab;
    public GameObject parent;
    // Use this for initialization
    void Start()
    {
        int packsCount = GameObject.Find("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController>().packsCount;

        for (int i = 0; i < packsCount - 1; i++)
        {
            GameObject myObject = Instantiate(prefab);
            myObject.GetComponent<EmojiShopController>().fillData(i);
            myObject.transform.parent = parent.transform;
            myObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
