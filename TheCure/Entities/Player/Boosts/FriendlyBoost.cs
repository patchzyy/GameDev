using TheCure.PlayerActions;

namespace TheCure.Boosts;

public class FriendlyBoost : Boost
{
    public FriendlyBoost(string iconName) : base(iconName)
    {
        BoostDuration = 8f;
        BoostBaseMultiplier = 1.05f;
        BoostUpgradeStep = 0.05f;
        BoostSettings.Add(SettingsConst.FRIENDLY.ATTACK_DAMAGE);
        BoostSettings.Add(SettingsConst.FRIENDLY.SIZE);
    }
}
