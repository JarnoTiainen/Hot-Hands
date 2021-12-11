using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenLinkConfirmationScript : MonoBehaviour
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    void Start()
    {
        noButton.onClick.AddListener(() => CloseLinkConfirmation());
    }

    private void CloseLinkConfirmation()
    {
        yesButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}
