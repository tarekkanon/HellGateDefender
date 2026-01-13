/// <summary>
/// Constants for enemy type IDs.
/// Use these constants instead of hard-coded strings to prevent typos.
/// Add new enemy types here as they are created.
/// </summary>
public static class EnemyTypes
{
    // Base enemy types from GDD
    public const string Scout = "Scout";
    public const string Grunt = "Grunt";
    public const string Tank = "Tank";

    // Add new enemy types here as needed:
    // public const string BossGoblin = "BossGoblin";
    // public const string FlyingDemon = "FlyingDemon";
    // public const string ArmoredKnight = "ArmoredKnight";

    /// <summary>
    /// Get all defined enemy type IDs
    /// </summary>
    public static string[] GetAll()
    {
        return new string[]
        {
            Scout,
            Grunt,
            Tank
            // Add new types to this array as well
        };
    }

    /// <summary>
    /// Check if an enemy type ID is defined
    /// </summary>
    public static bool IsDefined(string enemyTypeId)
    {
        return enemyTypeId == Scout ||
               enemyTypeId == Grunt ||
               enemyTypeId == Tank;
               // Add new types to this check as well
    }
}
