using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchResultScreen : MonoBehaviour
{

    public void GoToResultScreen()
    {
        SceneManager.LoadScene(2);
    }

}
