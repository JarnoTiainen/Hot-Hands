using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LIfeCounterCollisionManager : MonoBehaviour
{
    [SerializeField] private int collisionCount = 0;
    [SerializeField] private LifeCounterManager lifeCounterManager;
    [SerializeField] private bool isYourCounter;
    bool receivingOn;
    int total;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnParticleCollision(GameObject other)
    {
        collisionCount++;
        Debug.Log("COLLISION " + collisionCount);
        if(receivingOn)
        {
            lifeCounterManager.LoseOneHealth();
        }
    }

    public void StartReseivingEvent(int total)
    {
        this.total = total;
        receivingOn = true;
        StartCoroutine(ToggleReceivingOff());
    }

    private IEnumerator ToggleReceivingOff()
    {
        yield return new WaitForSeconds(2f);
        receivingOn = false;
        lifeCounterManager.SetNewNumber(isYourCounter, GameManager.Instance.playerStats.playerHealth);
    }
}
