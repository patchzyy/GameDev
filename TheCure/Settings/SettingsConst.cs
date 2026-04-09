using TheCure.Weapons;

namespace TheCure
{
    public static class SettingsConst
    {
        public static class PLAYER
        {
            private static string _group = "PLAYER";

            public static readonly SettingKey<float> MAX_HEALTH = new(_group, "MAX_HEALTH");
            public static readonly SettingKey<float> MOVE_SPEED = new(_group, "MOVE_SPEED");
        }

        public static class ZOMBIE
        {
            private static string _group = "ZOMBIE";

            public static readonly SettingKey<float> SPEED = new(_group, "MOVE_SPEED");
            public static readonly SettingKey<float> STAGGER = new(_group, "STAGGER");
            public static readonly SettingKey<int> ATTACK_DAMAGE = new(_group, "ATTACK_DAMAGE");
            public static readonly SettingKey<float> ATTACK_COOLDOWN = new(_group, "ATTACK_COOLDOWN");
            public static readonly SettingKey<float> START_HEALTH = new(_group, "START_HEALTH");
            public static readonly SettingKey<float> MAX_HEALTH = new(_group, "MAX_HEALTH");
        }

        public static class FRIENDLY
        {
            private static string _group = "FRIENDLY";

            public static readonly SettingKey<float> MOVE_SPEED = new(_group, "MOVE_SPEED");
            public static readonly SettingKey<float> START_HEALTH = new(_group, "START_HEALTH");
            public static readonly SettingKey<float> MAX_HEALTH = new(_group, "MAX_HEALTH");
            public static readonly SettingKey<float> FOLLOW_DISTANCE = new(_group, "FOLLOW_DISTANCE");
            public static readonly SettingKey<BaseWeapon> START_WEAPON = new(_group, "START_WEAPON");
            public static readonly SettingKey<float> RADIUS = new(_group, "RADIUS");
        }
    }
}