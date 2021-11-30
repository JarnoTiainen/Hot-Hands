using System;

[Serializable]
public class TriggerSpellMessage
{
    public bool denied;
    public int index;

    public TriggerSpellMessage(bool denied, int index)
    {
        this.denied = denied;
        this.index = index;
    }
}
