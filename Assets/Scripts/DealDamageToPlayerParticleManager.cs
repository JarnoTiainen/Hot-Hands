using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DealDamageToPlayerParticleManager : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private int repeats;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    [Button] public void SetForceField(bool isYourCard)
    {
        if(!isYourCard)
        {
            particleSystem.externalForces.AddInfluence(References.i.yourForceField);
            particleSystem.trigger.AddCollider(References.i.yourLifeCalculatorCollider);
        }
        else
        {
            particleSystem.externalForces.AddInfluence(References.i.opponentForceField);
            particleSystem.trigger.AddCollider(References.i.opponentLifeCalculatorCollider);
        }
    }

    [Button] public void DealDamageToPlayer(int amount)
    {
        repeats = amount;
        InvokeRepeating("DealDamageToPlayer", 0, 0.1f);
    }

    public void DealDamageToPlayer()
    {
        particleSystem.Play();
        repeats--;
        if(repeats <= 0)
        {
            CancelInvoke();
        }
    }
}
