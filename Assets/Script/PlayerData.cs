[System.Serializable]
public class PlayeraData
{
    public string playerName;
    public PlayerClass playerClass;
    public int level;
    public int coin;
}

public enum PlayerClass
{
    Knight,
    Archer,
    Mage,
    Assassin,
    Cleric
}