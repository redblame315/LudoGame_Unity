using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController2 : MonoBehaviour
{

    public GameObject mainDice;

    // Use this for initialization
    void Start()
    {

    }

    public void FinishAnim()
    {
        mainDice.GetComponent<GameDiceController>().SetDiceValue();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
