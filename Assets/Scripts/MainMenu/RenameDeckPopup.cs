using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RenameDeckPopup : MonoBehaviour
{
    public TMP_InputField deckNameInput;
    [SerializeField] private TextMeshProUGUI deckNameCharacterCountText;
    [SerializeField] private TextMeshProUGUI renameText;
    [SerializeField] private GameObject renameButton;
    private int charLimit;

    private void OnEnable()
    {
        int deckIndex = -1;
        for (int i = 0; CollectionManager.Instance.deckToggles.Count > i; i++)
        {
            if (CollectionManager.Instance.deckToggles[i].deckSelected)
            {
                deckIndex = i;
                break;
            }
        }
        string deckName;
        if (CollectionManager.Instance.deckNames[deckIndex] == "") deckName = "Deck " + (deckIndex + 1);
        else deckName = CollectionManager.Instance.deckNames[deckIndex];
        renameText.text = "Rename " + deckName + ":";
        charLimit = deckNameInput.characterLimit;
        deckNameInput.onValueChanged.AddListener((call) => UpdateCharacterCount());
        UpdateCharacterCount();
        deckNameInput.ActivateInputField();
        deckNameInput.Select();
    }

    private void OnDisable()
    {
        deckNameInput.onValueChanged.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CollectionButtonScript renameButtonScript = renameButton.GetComponent<CollectionButtonScript>();
            renameButtonScript.OnClickElement();
            renameButtonScript.meshRenderer.material.SetInt("_IsClicked", 0);
            renameButtonScript.StopAllCoroutines();
        }
    }

    private void UpdateCharacterCount()
    {
        int msgLength = deckNameInput.text.Length;
        deckNameCharacterCountText.text = msgLength + "/" + charLimit;
        if (msgLength >= charLimit)
        {
            deckNameCharacterCountText.color = Color.yellow;
        }
        else
        {
            deckNameCharacterCountText.color = Color.white;
        }
    }
}
