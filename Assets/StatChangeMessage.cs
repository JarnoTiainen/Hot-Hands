using System.Collections.Generic;
using System;
[Serializable]
public class StatChangeMessage
{
    public List<string> targets;
    public List<CardDataMessage> convertedTargets;
}
