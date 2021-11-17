using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionLogoManager : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer meshRenderer;

    


    private void Awake()
    {
        meshRenderer.material = material;
    }

    public void SetNewImage(List<Enchantment> enchantmnets)
    {
        

        foreach (Enchantment enchantment in enchantmnets)
        {
            switch (enchantment.trigger)
            {
                case Enchantment.Trigger.Opener:
                    meshRenderer.material.SetTexture("_Texture2D", References.i.openerIcon);
                    meshRenderer.material.SetColor("_SpriteColor", References.i.openerColor);
                    break;
                case Enchantment.Trigger.Sacrifice:
                    meshRenderer.material.SetTexture("_Texture2D", References.i.sacrifice);
                    meshRenderer.material.SetColor("_SpriteColor", References.i.sacrificeColor);
                    break;
                case Enchantment.Trigger.LastBreath:
                    meshRenderer.material.SetTexture("_Texture2D", References.i.lastBreathIcon);
                    meshRenderer.material.SetColor("_SpriteColor", References.i.lastBreathColor);
                    break;
                case Enchantment.Trigger.Drawtivation:
                    meshRenderer.material.SetTexture("_Texture2D", References.i.drawtivationIcon);
                    meshRenderer.material.SetColor("_SpriteColor", References.i.drawtivationColor);
                    break;
            }
        }
    }
}
