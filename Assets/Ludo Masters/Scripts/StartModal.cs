
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class StartModal : MonoBehaviour
{
    private ModalPanelControl modalPanel;

    private UnityAction myCancelAction;

    void Awake()
    {
        modalPanel = ModalPanelControl.Instance();

        myCancelAction = new UnityAction(TestCancelFunction);
    }

    //  Send to the Modal Panel to set up the Buttons and Functions to call
    public void TestYNC()
    {
        modalPanel.Choice("Do you want to spawn this sphere?", TestYesFunction, TestNoFunction, TestCancelFunction);
    }

    //  These are wrapped into UnityActions
    void TestYesFunction()
    {
        //displayManager.DisplayMessage("Heck yeah! Yup!");
    }

    void TestNoFunction()
    {
        //displayManager.DisplayMessage("No way, José!");
    }

    void TestCancelFunction()
    {
        //displayManager.DisplayMessage("I give up!");
    }
}