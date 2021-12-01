using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InGameSpell : MonoBehaviour
{
    [SerializeField] public CardData cardData;
    [SerializeField] private int spellSlotNumber;
    [SerializeField] private Texture2D cardImage;

    private Quaternion startRot;
    private Quaternion endRot;
    [SerializeField] private bool rotating;
    [SerializeField] private AnimationCurve rotCurve;
    private float elapsedRotationTime = 0;
    [SerializeField] private float rotSpeed;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private List<MeshRenderer> lightMeshes = new List<MeshRenderer>();
    [SerializeField] private Material lightMaterial;
    private int lightsOn;
    public bool slotTaken;
    float time;
    [SerializeField] private float speed;
    private bool animating = false;
    public List<Line> lines;
    bool faceup;

    private void Awake()
    {
        foreach(MeshRenderer lightMesh in lightMeshes)
        {
            lightMesh.material = lightMaterial;
            lightMesh.material.SetInt("_LightOn", 0);
        }
    }

    [Button] public void TurnLightsOn()
    {
        foreach (MeshRenderer lightMesh in lightMeshes)
        {
            lightMesh.material.SetInt("_LightOn", 1);
        }
    }

    public void SetNewCardData(CardData cardData)
    {
        Debug.Log("Setting new spell to coin");
        this.cardData = cardData;
        spriteRenderer.sprite = cardData.cardSprite;
        TurnLightsOn();
        FlipSpell(true);
    }

    public void SetNewSpellToslot(GameObject newSpell)
    {
        SetNewCardData(newSpell.GetComponent<InGameCard>().GetData());
        slotTaken = true;
        Debug.LogWarning("Play sound and effect here. New card with name: " + newSpell.GetComponent<InGameCard>().GetData().cardName + " set");
    }

    [Button] public void RemoveSpellFromSlot()
    {
        Debug.Log("Removing spell from slot");
        cardData = null;
        slotTaken = false;
    }

    [Button]public void FlipSpell(bool toUp)
    {
        Debug.Log("flipping " + toUp);

        startRot = transform.rotation;
        if(toUp)
        {
            endRot = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            endRot = Quaternion.Euler(0, -180, 90);
            RemoveSpellFromSlot();
        }
        elapsedRotationTime = 0;
    }

    private void Update()
    {
        if(rotating)
        {
            elapsedRotationTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, endRot, rotCurve.Evaluate(elapsedRotationTime / rotSpeed));
            if(transform.rotation.eulerAngles == endRot.eulerAngles)
            {
                rotating = false;
            }
        }
        if(animating)
        {
            time -= Time.deltaTime * speed;
            foreach(MeshRenderer lightMesh in lightMeshes)
            {
                lightMesh.material.SetFloat("_LightAnimationStep", time);
            }
            if(time <= 0)
            {
                animating = false;
                time = 1;
                FlipSpell(false);
            }
        }
    }

    [Button] public void StartCounter(float duration)
    {
        CancelInvoke();
        TurnLightsOn();
        lightsOn = 6;
        int numberOfLights = 6;
        float timeBetweenTicks = duration / numberOfLights;
        InvokeRepeating("Tick", timeBetweenTicks, timeBetweenTicks);
    }

    public void Tick()
    {
        lightsOn--;
        lightMeshes[lightsOn].material.SetInt("_LightOn", 0);
        if (lightsOn <= 0)
        {
            CancelInvoke();
        }
    }

    [Button]public void StartActivateEffect()
    {
        if(slotTaken)
        {
            animating = true;
            time = 1;
            slotTaken = false;
            foreach (Line line in lines)
            {
                line.RemoveLine();
            }
            lines = new List<Line>();
        }
    }
}
