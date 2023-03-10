using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

//  This script will be updated in Part 2 of this 2 part series.
public class ModalPanelControl : MonoBehaviour
{
   
    public Button cancelButton;
    public GameObject modalPanelObject;

    private static ModalPanelControl modalPanel;

    public static ModalPanelControl Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ModalPanelControl)) as ModalPanelControl;
        }

        return modalPanel;
    }
    public void clicked(Button button) {
        modalPanelObject.SetActive(true);
    }
    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
    public void Choice(string question, UnityAction yesEvent, UnityAction noEvent, UnityAction cancelEvent)
    {
        modalPanelObject.SetActive(true);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(cancelEvent);
        cancelButton.onClick.AddListener(ClosePanel);

        cancelButton.gameObject.SetActive(true);
    }


    void ClosePanel()
    {
        modalPanelObject.SetActive(false);
    }
}