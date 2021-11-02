using System.Collections.Generic;
using System;

[Serializable]
public class GetDecksMessage
{
    public List<string> deck0;
    public List<string> deck1;
    public List<string> deck2;
    public List<string> deck3;
    public List<string> deck4;
    public int activeDeckIndex;
}
