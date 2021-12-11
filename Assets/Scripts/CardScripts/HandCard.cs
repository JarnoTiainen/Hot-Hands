using UnityEngine;

public class HandCard : MonoBehaviour, IOnClickDownUIElement
{
    [SerializeField] private bool onTargetMode;
    [SerializeField] private GameObject twirlEffectPrefab;
    [SerializeField] private Line line;
    GameObject newTwirlEffect;
    public bool targetable = true;

    public void OnClickElement()
    {
        if(!Mouse.Instance.targetModeOn && targetable)
        {
            if (!GetComponent<InGameCard>().cardHidden)
            {
                transform.parent.GetComponent<Hand>().RemoveVisibleCard(gameObject);
                transform.localPosition = Vector3.zero;
                Mouse.Instance.SetNewHeldCard(gameObject);
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    public void StartPuffEffect()
    {
       
    }

    public void SwitchToSpellMode()
    {
        if(!onTargetMode)
        {
            GetComponent<InGameCard>().SpellBurn();
            newTwirlEffect = Instantiate(twirlEffectPrefab);
            newTwirlEffect.transform.SetParent(transform);
            newTwirlEffect.transform.localPosition = Vector3.zero;
            onTargetMode = true;

            if (gameObject.GetComponent<InGameCard>().GetData().targetting)
            {
                line = LineRendererManager.Instance.CreateNewLine(References.i.yourPlayerTarget, gameObject);
            }
        }
    }
    public void SwitchToCardMode()
    {
        if (onTargetMode)
        {
            GetComponent<InGameCard>().ReverseSpellBurn();
            Destroy(newTwirlEffect);
            onTargetMode = false;
            if(line != null)
            {
                line.RemoveLine();
            }
        }
    }
}
