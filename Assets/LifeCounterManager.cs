using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifeCounterManager : MonoBehaviour
{
    public TextMeshProUGUI leftNumber;
    public TextMeshProUGUI rightNumber;

    public int displayedYourLifePoints;
    public int displayedOpponentLifePoints;


    public bool isYourCounter;



    private void Awake()
    {
        displayedYourLifePoints = GameManager.Instance.playerStats.playerHealth;
        displayedOpponentLifePoints = GameManager.Instance.enemyPlayerStats.playerHealth;
        if(isYourCounter) SetNewNumber(true, displayedYourLifePoints);
        if(!isYourCounter) SetNewNumber(false, displayedOpponentLifePoints);
    }

    private void Update()
    {
        if (isYourCounter && GameManager.Instance.playerStats.playerHealth != displayedYourLifePoints)
        {
            Debug.Log("Modifying your health: " + GameManager.Instance.playerStats.playerHealth + ", " + displayedYourLifePoints);
            HealthPointsChangeEvent(true, GameManager.Instance.playerStats.playerHealth - displayedYourLifePoints);
        }
        if (!isYourCounter && GameManager.Instance.enemyPlayerStats.playerHealth != displayedOpponentLifePoints)
        {
            HealthPointsChangeEvent(false, GameManager.Instance.enemyPlayerStats.playerHealth - displayedOpponentLifePoints);
        }
    }

    private void HealthPointsChangeEvent(bool isYou, int change)
    {
        if (change < 0)
        {
            if (isYou)
            {
                displayedYourLifePoints = GameManager.Instance.playerStats.playerHealth;
                SetNewNumber(true, displayedYourLifePoints);
            }
            else
            {
                displayedOpponentLifePoints = GameManager.Instance.enemyPlayerStats.playerHealth;
                SetNewNumber(false, displayedOpponentLifePoints);
            }
        }
        else
        {
            if (isYou)
            {
                Debug.Log("you gain health " + change);
                displayedYourLifePoints = GameManager.Instance.playerStats.playerHealth;
                SetNewNumber(true, displayedYourLifePoints);
            }
            else
            {
                Debug.Log("opponent gain health " + change);
                displayedOpponentLifePoints = GameManager.Instance.enemyPlayerStats.playerHealth;
                SetNewNumber(false, displayedOpponentLifePoints);
            }
        }
    }

    public void SetNewNumber(bool isYou, int newNumber)
    {
        string numberArray = newNumber.ToString();
        char firstNumber = '0';
        char lastNumber;
        if (numberArray.Length > 1)
        {
            firstNumber = numberArray[0];
            lastNumber = numberArray[1];
        }
        
        else
        {
            lastNumber = numberArray[0];
        }


        if (firstNumber == '-')
        {
            firstNumber = '0';
            lastNumber = '0';
        }
        leftNumber.text = firstNumber.ToString();
        rightNumber.text = lastNumber.ToString();
    }

    public void LoseOneHealth()
    {
        if(isYourCounter)
        {
            SetNewNumber(isYourCounter, displayedYourLifePoints - 1);
        }
        else
        {
            SetNewNumber(!isYourCounter, displayedOpponentLifePoints - 1);
        }
        
    }
}
