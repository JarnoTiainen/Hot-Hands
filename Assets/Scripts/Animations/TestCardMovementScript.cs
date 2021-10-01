using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCardMovementScript : MonoBehaviour
{
    public GameObject card;

    [SerializeField]
    public AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            card.GetComponent<CardMovement>().OnCardMove(new Vector3(2, 7, 0), 3);
        }

        if (Input.GetKeyDown(KeyCode.A)) {
            card.GetComponent<CardMovement>().OnCardMove(new Vector3(2, 7, 0), 3, curve);
        }
    }
}
