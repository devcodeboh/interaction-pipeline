using System;

[Serializable]
public sealed class GameSaveData
{
    public int version = 1;
    public int difficulty;
    public int gridX;
    public int gridY;
    public int[] pairIds;
    public int[] cardStates;
    public int matches;
    public int turns;
}
