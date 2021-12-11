using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerInitializer : MonoBehaviour
{
    [SerializeField ] private GameObject gameManager;
    // Start is called before the first frame update
    void Awake()
    {
        if(!GameObject.FindGameObjectWithTag("GameManager"))
        {
            Instantiate(gameManager);
        }
    }
}
