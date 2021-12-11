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
    [SerializeField] private TextMeshProUGUI orderToggleIcon;

    private void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SortAllLists()
    {
        CollectionCardList.SortMethod sortMethod = GetSortMethod();
        CollectionManager.Instance.collectionCardList.GetComponent<CollectionCardList>().SortList(sortMethod, reverse);
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
            orderToggleIcon.text = "<<";
        }
        else
        {
            orderToggleIcon.text = ">>";
        }
        this.reverse = reverse;
        SortAllLists();
    }
}
