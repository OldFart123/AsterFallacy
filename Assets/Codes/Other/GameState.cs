public static class GameState
{
    public static bool IsPaused = false;
    public static bool IsDialogue = false;
    public static bool IsInventoryOpen = false;

    public static bool GameplayBlocked => IsPaused || IsDialogue || IsInventoryOpen;
}