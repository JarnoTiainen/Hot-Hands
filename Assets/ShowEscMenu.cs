using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowEscMenu : MonoBehaviour
{
    public GameObject escMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(escMenu.activeSelf)
            {
                escMenu.SetActive(false);
            }
            else
            {
                escMenu.SetActive(true);

            }
        }

        
    }
}
