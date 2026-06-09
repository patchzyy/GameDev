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
            public static readonly SettingKey<float> ATTACK_DAMAGE = new(_group, "ATTACK_DAMAGE");
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
            public static readonly SettingKey<float> ATTACK_DAMAGE = new(_group, "ATTACK_DAMAGE");
            public static readonly SettingKey<float> HEALTH_LOSS_PER_SECOND = new(_group, "HEALTH_LOSS_PER_SECOND");
            public static readonly SettingKey<float> SIZE = new(_group, "SIZE");
            public static readonly SettingKey<float> FOLLOW_DISTANCE = new(_group, "FOLLOW_DISTANCE");
            public static readonly SettingKey<BaseWeapon> START_WEAPON = new(_group, "START_WEAPON");
            public static readonly SettingKey<float> RADIUS = new(_group, "RADIUS");
        }

        public static class SINGLE_BULLET_WEAPON
        {
            private static string _group = "SINGLE_BULLET_WEAPON";

            public static readonly SettingKey<float> DAMAGE = new(_group, "DAMAGE");
        }

        public static class VIDEO
        {
            public static readonly SettingKey<int> WIDTH = new("VIDEO", "WIDTH");

            public static readonly SettingKey<int> HEIGHT = new("VIDEO", "HEIGHT");
            public static readonly SettingKey<DisplayModeSetting> DISPLAY_MODE = new("VIDEO", "DISPLAY_MODE");
        }

        public static class KEY_BINDS
        {
            private static string _group = "KEY_BINDS";

            // PLAYER ACTIONS
            public static readonly SettingKey<InputBinding> SHOOT = new(_group, "SHOOT");
            public static readonly SettingKey<InputBinding> DASH = new(_group, "DASH");

            public static readonly SettingKey<InputBinding> ACTION_1 = new(_group, "ACTION_1");
            public static readonly SettingKey<InputBinding> ACTION_2 = new(_group, "ACTION_2");
            public static readonly SettingKey<InputBinding> ACTION_3 = new(_group, "ACTION_3");
            public static readonly SettingKey<InputBinding> ACTION_4 = new(_group, "ACTION_4");
            public static readonly SettingKey<InputBinding> ACTION_5 = new(_group, "ACTION_5");
        }

        public static class HEAL_BOMB
        {
            private static string _group = "HEAL_BOMB";

            public static readonly SettingKey<int> RADIUS = new(_group, "RADIUS");
            public static readonly SettingKey<float> HEALING = new(_group, "HEALING");
            public static readonly SettingKey<int> TICKS = new(_group, "TICKS");
        }

        public static class HEAL_BOMB_TRAP
        {
            private static string _group = "HEAL_BOMB_TRAP";

            public static readonly SettingKey<float> HEALING = new(_group, "HEALING");
            public static readonly SettingKey<float> TICK_INTERVAL = new(_group, "TICK_INTERVAL");
            public static readonly SettingKey<int> RADIUS = new(_group, "RADIUS");
        }
        
        public static class FREEZE_TRAP
        {
            private static string _group = "FREEZE_TRAP";

            public static readonly SettingKey<float> DURATION = new(_group, "DURATION");
            public static readonly SettingKey<float> SLOW_FACTOR = new(_group, "SLOW_FACTOR");
        }
        
        public static class ELECTRIC_TRAP
        {
            private static string _group = "ELECTRIC_TRAP";

            public static readonly SettingKey<int> DAMAGE_PER_TICK = new(_group, "DAMAGE_PER_TICK");
            public static readonly SettingKey<float> STUN_FORCE = new(_group, "STUN_FORCE");
            public static readonly SettingKey<float> STUN_DURATION = new(_group, "STUN_DURATION");
            public static readonly SettingKey<float> DAMAGE_TICK_INTERVAL = new(_group, "DAMAGE_TICK_INTERVAL");
        }
        public static class BOMB_TRAP
        {
            private static string _group = "BOMB_TRAP";

            public static readonly SettingKey<float> ACTIVATION_DELAY = new(_group, "ACTIVATION_DELAY");
            public static readonly SettingKey<int> EXPLOSION_DAMAGE = new(_group, "EXPLOSION_DAMAGE");
            public static readonly SettingKey<float> EXPLOSION_RADIUS = new(_group, "EXPLOSION_RADIUS");
            public static readonly SettingKey<float> EXPLOSION_FADE_DURATION = new(_group, "EXPLOSION_FADE_DURATION");
        }
        public static class SPIKE_TRAP
        {
            private static string _group = "SPIKE_TRAP";

            public static readonly SettingKey<int> DAMAGE_PER_HIT = new(_group, "DAMAGE_PER_HIT");
            public static readonly SettingKey<float> DAMAGE_INTERVAL = new(_group, "DAMAGE_INTERVAL");
        }
        
    }
}