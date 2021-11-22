using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public float skipDuration = 3;
    public float attackCoolDown = 3;
    public Image skipBar;
    public bool burnignAllowed;
    public bool summoningAllowed;
    [SerializeField] private int startBurnValue = 4;
    [SerializeField] private float skipTime;
    
    [SerializeField] private TutorialState tutorialState = TutorialState.Introduction;
    public static TutorialManager tutorialManagerInstance { get; private set; }

    public enum TutorialState
    {
        Introduction,
        CardDraw,
        Dialogue1,
        BurnCard,
        Dialogue2,
        CardPlay,
        Dialogue3,
        CardAttack,
        
        SpellCard,
        Damage
    }

    public TutorialState GetState()
    {
        return tutorialState;
    }

    public void NextTutorialState()
    {
        tutorialState++;
    }
    

    private void Awake()
    {
        tutorialManagerInstance = gameObject.GetComponent<TutorialManager>();
        WebSocketService.Instance.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        skipTime = 0;
        skipBar.fillAmount = 0;
        GameManager.Instance.playerNumber = 0;
        GameManager.Instance.playerStats.playerBurnValue = startBurnValue;
        References.i.yourBonfire.GetComponent<Bonfire>().burnValue.text = startBurnValue.ToString();
    }

   

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E)) {
            skipTime += Time.deltaTime;
            skipBar.fillAmount = skipTime / skipDuration;
        }

        if (skipTime >= skipDuration) {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyUp(KeyCode.E)) {
            skipTime = 0;
            skipBar.fillAmount = 0;
        }

        if (tutorialState == TutorialState.BurnCard) {
            burnignAllowed = true;
        }

        
        if (tutorialState == TutorialState.CardPlay) {
            summoningAllowed = true;
        }

    }

    public void SwitchState(TutorialState newState)
    {
        tutorialState = newState;

    }
}
