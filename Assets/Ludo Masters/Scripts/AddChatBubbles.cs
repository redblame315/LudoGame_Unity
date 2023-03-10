using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class AddChatBubbles : MonoBehaviour
{

    public GameObject prefab;
    public GameObject parent;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < StaticStrings.chatNames.Length; i++)
        {
            GameObject myObject = Instantiate(prefab);
            myObject.GetComponent<ChatShopController>().fillData(i);
            myObject.transform.parent = parent.transform;
            myObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
