using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SortMethodDropdown : MonoBehaviour
{
    public static SortMethodDropdown Instance { get; private set; }
    public bool reverse = false;
    [SerializeField] private GameObject dropdownButton;
    [SerializeField] private GameObject orderToggle;

    private void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SortAllLists()
    {
        CollectionCardList.SortMethod sortMethod = GetSortMethod();
        foreach(GameObject list in CollectionManager.Instance.cardLists)
        {
            list.GetComponent<CollectionCardList>().SortList(sortMethod, reverse);
        }
        dropdownButton.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sortMethod.ToString().ToUpper();
        CollectionManager.Instance.UpdatePageText();
        gameObject.SetActive(false);
    }

    public CollectionCardList.SortMethod GetSortMethod()
    {
        int sortIndex = -1;
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Toggle>().isOn) sortIndex = child.GetSiblingIndex();
        }
        return (CollectionCardList.SortMethod)sortIndex;
    }

    public void SetListOrder(bool reverse)
    {
        GameObject iconContainer = orderToggle.transform.Find("IconContainer").gameObject;
        if (reverse)
        {
            iconContainer.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "<=";
            iconContainer.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "|''";
        }
        else
        {
            iconContainer.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "=>";
            iconContainer.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "''|";
        }
        this.reverse = reverse;
        SortAllLists();
    }
}
