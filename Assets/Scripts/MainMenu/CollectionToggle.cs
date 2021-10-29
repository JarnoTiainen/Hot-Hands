using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionToggle : MonoBehaviour
{
    public int index;
    private Toggle toggle;

    private void Start()
    {
        toggle = gameObject.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate
        {
            ChangeDeck();
        });
    }

    public void ChangeDeck()
    {
        GameObject.Find("SettingsManager").GetComponent<CollectionManager>().ChangeActiveCardList(index);
    }

}
