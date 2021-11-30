using System.Collections.Generic;
using System;
[Serializable]
public class StatChangeMessage
{
    public List<string> targets;
    public List<CardDataMessage> convertedTargets;
    public CardDataMessage.BuffType buffType;

    public StatChangeMessage(CardDataMessage.BuffType buffType, CardDataMessage target = null)
    {
        this.buffType = buffType;

        if (target != null) {
            convertedTargets = new List<CardDataMessage>();
            convertedTargets.Add(target);
        }
    }
}
