using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGameManager : MonoBehaviour
{
    public GameObject gameManager;

    void Start()
    {
        if (FindObjectOfType<GameManager>()) MainMenu.Instance.MainMenuButtonsSetActive(true);

        else
        {
            GameObject newGameManager = Instantiate(gameManager, new Vector3(0, 0, 0), Quaternion.identity);
            newGameManager.name = "GameManager";
            Debug.Log("Created new GameManager");
        }
    }
}
