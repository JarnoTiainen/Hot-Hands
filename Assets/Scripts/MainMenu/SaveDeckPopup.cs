using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveDeckPopup : MonoBehaviour
{
    [SerializeField] private Toggle togglePrefab;
    [SerializeField] private GameObject toggles;
    public TMP_InputField deckNameInput;
    public List<Toggle> saveDeckToggles = new List<Toggle>();

    private void OnEnable()
    {
        UpdateSaveToggles();
    }

    private void InstantiateSaveToggle(int index)
    {
        Toggle newToggle = Instantiate(togglePrefab) as Toggle;
        newToggle.name = "DECK " + (index + 1);
        newToggle.group = toggles.GetComponent<ToggleGroup>();
        newToggle.transform.SetParent(toggles.transform, false);
        saveDeckToggles.Add(newToggle);
    }

    private void UpdateSaveToggles()
    {
        if(saveDeckToggles.Count == 0)
        {
            for (int i = 0; CollectionManager.Instance.playerDeckLimit > i; i++)
            {
                InstantiateSaveToggle(i);
            }
        }

        List<string> deckNames = CollectionManager.Instance.deckNames;
        for (int i = 0; saveDeckToggles.Count > i; i++)
        {
            if(deckNames[i] == null || deckNames[i] == "")
            {
                saveDeckToggles[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "DECK " + (i + 1);
            }
            else
            {
                saveDeckToggles[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = deckNames[i];
            }
        }
    }
}
