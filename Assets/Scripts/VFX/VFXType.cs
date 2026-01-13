namespace BaseDefender.VFX
{
    /// <summary>
    /// Enum defining all VFX types in the game.
    /// Add new types here when creating new effects.
    /// </summary>
    public enum VFXType
    {
        // Player Effects
        PlayerSpellProjectile,
        PlayerMuzzleFlash,
        PlayerFootsteps,

        // Tower Effects
        TowerInactive,
        TowerActivation,
        TowerIdleGlow,
        TowerSpellProjectile,
        TowerMuzzleFlash,

        // Combat/Impact Effects
        DemonicHitOnAngel,
        AngelicHitOnDemonic,
        AngelDeath,

        // Collection Effects
        CoinIdle,
        CoinCollect,
        CoinMagnetTrail,

        // Environment Effects
        AmbientAtmosphere,
        BaseShield,
        SpawnPortal,

        // UI Effects
        DamageNumber,
        TowerActivationPrompt
    }
}
