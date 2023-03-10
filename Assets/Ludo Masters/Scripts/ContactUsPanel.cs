using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ContactUsPanel : MonoBehaviour
{

	public Button cancelButton;
	public GameObject modalPanelObject;
	public Button emailBtn;
	public Button urlBtn;
	public Button openBtn;

	private static ContactUsPanel modalPanel;

	public static ContactUsPanel Instance()
	{
		if (!modalPanel)
		{
			modalPanel = FindObjectOfType(typeof(ContactUsPanel)) as ContactUsPanel;
			if (!modalPanel)
				Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
		}

		return modalPanel;
	}
	public void OpenPanel()
	{
		Debug.Log("Button click!");
		modalPanelObject.SetActive(true);
	}
	public void ClosePanel()
	{
		Debug.Log("Button click32!");
		modalPanelObject.SetActive(false);
	}
	public void emailBtnfn(){
		Debug.Log("Email clicked!");
		//email Id to send the mail to
		string email = "HelpDesk@LudoBetting.in";
		//subject of the mail
		string subject = MyEscapeURL("INQUIRY");
		//body of the mail which consists of Device Model and its Operating System
		string body = "";
		body = MyEscapeURL ("Please Enter your message here\n");
		Application.OpenURL ("mailto:" + email + "?subject=" + subject + "&body=" + body);
	}
	string MyEscapeURL (string url) 
	{
		return WWW.EscapeURL(url).Replace("+","%20");
	}
	public void urlBtnFn(){
		Debug.Log("Url clicked!");
		Application.OpenURL("http://www.ludobetting.in/");
	}
}
