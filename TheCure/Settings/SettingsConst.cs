using Microsoft.Xna.Framework.Input;
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
            public static readonly SettingKey<float> ATTACK_COOL_DOWN = new(_group, "ATTACK_COOL_DOWN");
            public static readonly SettingKey<float> START_HEALTH = new(_group, "START_HEALTH");
            public static readonly SettingKey<float> MAX_HEALTH = new(_group, "MAX_HEALTH");
        }

        public static class SPAWNING
        {
            private static string _group = "SPAWNING";

            public static readonly SettingKey<float> ZOMBIE_SPAWN_INTERVAL = new(_group, "ZOMBIE_SPAWN_INTERVAL");
            public static readonly SettingKey<int> ENEMIES_PER_WAVE = new(_group, "ENEMIES_PER_WAVE");
            public static readonly SettingKey<int> MAX_ENEMIES_ON_SCREEN = new(_group, "MAX_ENEMIES_ON_SCREEN");
            public static readonly SettingKey<float> BRUTE_SPAWN_CHANCE = new(_group, "BRUTE_SPAWN_CHANCE");
            public static readonly SettingKey<int> MAX_BRUTES = new(_group, "MAX_BRUTES");
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

        public static class KEY_BINDS
        {
            private static string _group = "KEY_BINDS";

            // PLAYER ACTIONS
            public static readonly SettingKey<Keys> ACTION_1 = new(_group, "ACTION_1");
            public static readonly SettingKey<Keys> ACTION_2 = new(_group, "ACTION_2");
            public static readonly SettingKey<Keys> ACTION_3 = new(_group, "ACTION_3");
            public static readonly SettingKey<Keys> ACTION_4 = new(_group, "ACTION_4");
            public static readonly SettingKey<Keys> ACTION_5 = new(_group, "ACTION_5");
            public static readonly SettingKey<Keys> ACTION_6 = new(_group, "ACTION_6");
        }
    }
}