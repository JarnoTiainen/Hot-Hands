using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNamesUIScript : MonoBehaviour
{
    public static PlayerNamesUIScript Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI player1NameText;
    [SerializeField] private TextMeshProUGUI player2NameText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player1NameText.text = PlayerPrefs.GetString("LoginName", "Player 1");
        WebSocketService.GetOpponentName();
    }

    public void SetOpponentName(string name)
    {
        player2NameText.text = name;
    }
}
