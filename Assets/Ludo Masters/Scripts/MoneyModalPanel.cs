using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class MoneyModalPanel : MonoBehaviour {

    public Button cancelButton;
    public GameObject modalPanelObject;

    private static MoneyModalPanel modalPanel;

    public static MoneyModalPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(MoneyModalPanel)) as MoneyModalPanel;
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }
    public void clicked(Button button)
    {
        modalPanelObject.SetActive(true);
    }
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
