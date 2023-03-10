using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LudoPathObjectController : MonoBehaviour
{

    public List<GameObject> pawns = new List<GameObject>();
    public bool isProtectedPlace;
    // Use this for initialization
    void Start()
    {
        GetComponent<Image>().enabled = false;
    }

    public void AddPawn(GameObject pawn)
    {
        pawns.Add(pawn);
    }

    public void RemovePawn(GameObject pawn)
    {
        pawns.Remove(pawn);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
