using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AssemblyCSharp;

public class InviteRewardTextScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GetComponent<Text>().text = "Earn " + StaticStrings.rewardCoinsForFriendInvite + " coins";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
