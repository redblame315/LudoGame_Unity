using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModal : MonoBehaviour {

    void OnEnable()
    {
        transform.SetAsLastSibling();
    }
}
