using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildCardUI : MonoBehaviour
{
    public string cardName;
    public int amount;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI amountText;

    public void UpdateName()
    {
        nameText = gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        nameText.text = cardName;
    }

    public void UpdateAmount()
    {
        amountText = gameObject.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        amountText.text = "x" + amount.ToString();
    }
}
