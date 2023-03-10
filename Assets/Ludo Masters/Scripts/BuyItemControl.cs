using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuyItemControl : MonoBehaviour {

    public int index = 1;
    public GameObject priceText;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start() {
        //if (GameManager.Instance.IAPControl.controller != null) {
        //    if (this.index == 1) {
        //        priceText.GetComponent<Text>().text = GameManager.Instance.IAPControl.controller.products.WithID(GameManager.Instance.IAPControl.SKU_50_COINS).metadata.localizedPriceString;
        //    } else if (this.index == 2) {
        //        priceText.GetComponent<Text>().text = GameManager.Instance.IAPControl.controller.products.WithID(GameManager.Instance.IAPControl.SKU_100_COINS).metadata.localizedPriceString;
        //    } else if (this.index == 3) {
        //        priceText.GetComponent<Text>().text = GameManager.Instance.IAPControl.controller.products.WithID(GameManager.Instance.IAPControl.SKU_500_COINS).metadata.localizedPriceString;
        //    } else if (this.index == 4) {
        //        priceText.GetComponent<Text>().text = GameManager.Instance.IAPControl.controller.products.WithID(GameManager.Instance.IAPControl.SKU_1000_COINS).metadata.localizedPriceString;
        //    } else if (this.index == 5) {
        //        priceText.GetComponent<Text>().text = GameManager.Instance.IAPControl.controller.products.WithID(GameManager.Instance.IAPControl.SKU_1500_COINS).metadata.localizedPriceString;
        //    }
        //}

    }

    public void buyItem() {
        switch(index)
        {
            case 0:
                Application.OpenURL("http://192.168.113.140/ludowala/buycoin/" + 20);
                break;
            case 1:
                Application.OpenURL("http://192.168.113.140/ludowala/buycoin/" + 50);
                break;
            case 2:
                Application.OpenURL("http://192.168.113.140/ludowala/buycoin/" + 100);
                break;
            case 3:
                Application.OpenURL("http://54.65.185.106/ludowala/buycoin/" + 500);
                break;
            case 4:
                Application.OpenURL("http://54.65.185.106/ludowala/buycoin/" + 1000);
                break;
            case 5:
                Application.OpenURL("http://54.65.185.106/ludowala/buycoin/" + 1500);
                break;
        }
    }

    public void withDrawCoin()
    {

    }
}
