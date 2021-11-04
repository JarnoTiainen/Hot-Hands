using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public float skipDuration = 3;
    public Image skipBar;
    [SerializeField] private float skipTime;
    


    // Start is called before the first frame update
    void Start()
    {
        skipTime = 0;
        skipBar.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.E)) { 
            skipTime += Time.deltaTime;
            skipBar.fillAmount = skipTime / skipDuration;
        }

        if(skipTime >= skipDuration) {
            SceneManager.LoadScene(0);
        }

        if(Input.GetKeyUp(KeyCode.E)) {
            skipTime = 0;
            skipBar.fillAmount = 0;
        }
    }
}
