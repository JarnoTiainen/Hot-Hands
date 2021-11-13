using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionToggle : MonoBehaviour
{
    public int index;

    private void Start()
    {
        gameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            ChangeDeck();
        });
    }

    public void ChangeDeck()
    {
        CollectionManager.Instance.ChangeActiveCardList(index);
    }
}
