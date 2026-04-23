using TheCure.PlayerActions;

namespace TheCure.Boosts;

public class FriendlyBoost : Boost
{
    public FriendlyBoost(string iconName) : base(iconName)
    {
        BoostDuration = 8f;
        BoostBaseMultiplier = 1.5f;
        BoostUpgradeStep = 0.5f;
        BoostSetting = SettingsConst.FRIENDLY.ATTACK_DAMAGE;
    }
}