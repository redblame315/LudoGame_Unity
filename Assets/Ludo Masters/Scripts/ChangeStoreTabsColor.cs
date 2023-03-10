using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStoreTabsColor : MonoBehaviour
{

    public GameObject[] tabs;

    private Color normalColor;
    private Color otherColor = new Color(0, 51.0f / 255f, 120.0f / 255.0f);
    // Use this for initialization
    void Start()
    {
        normalColor = tabs[2].GetComponent<Image>().color;

        SetSelectectedTab(2);

    }

    public void SetSelectectedTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i != index)
            {
                tabs[i].GetComponent<Image>().color = otherColor;

            }
            else
            {
                tabs[i].GetComponent<Image>().color = normalColor;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
