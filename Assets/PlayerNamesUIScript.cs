using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNamesUIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1NameText;
    [SerializeField] private TextMeshProUGUI player2NameText;

    void Start()
    {
        player1NameText.text = PlayerPrefs.GetString("LoginName", "Player 1");
    }
}
