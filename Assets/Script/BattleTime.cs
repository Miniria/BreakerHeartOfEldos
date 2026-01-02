public static class BattleTime
{
    public static bool IsPaused { get; private set; } = false;

    public static void PauseBattle()
    {
        IsPaused = true;
    }

    public static void ResumeBattle()
    {
        IsPaused = false;
    }
}
