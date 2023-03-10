using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class AboutUsPanel : MonoBehaviour
{

    public Button cancelButton;
    public GameObject modalPanelObject;

    private static AboutUsPanel modalPanel;

    public static AboutUsPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(AboutUsPanel)) as AboutUsPanel;
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }
    public void clicked(Button button)
    {
        Debug.Log("Button click!");
        modalPanelObject.SetActive(true);
    }
    public void Choice(string question, UnityAction yesEvent, UnityAction noEvent, UnityAction cancelEvent)
    {
        modalPanelObject.SetActive(true);
        Debug.Log("Button click2!");
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(cancelEvent);
        cancelButton.onClick.AddListener(ClosePanel);

        cancelButton.gameObject.SetActive(true);
    }


    void ClosePanel()
    {
        Debug.Log("Button click32!");
        modalPanelObject.SetActive(false);
    }
}

