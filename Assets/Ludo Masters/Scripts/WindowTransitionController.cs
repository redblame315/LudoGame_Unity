using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowTransitionController : MonoBehaviour
{

    private Animator animator;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        animator.Play("ShowScreen");
    }

    public void HideScreen()
    {
        animator.Play("HideScreen");
    }

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}
