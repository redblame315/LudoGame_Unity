using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightFollowPawn : MonoBehaviour
{

    private RectTransform tr;
    private RectTransform ptr;
    // Use this for initialization
    void Start()
    {
        tr = GetComponent<RectTransform>();
        ptr = tr.parent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //tr.anchoredPosition = ptr.anchoredPosition;
    }
}
