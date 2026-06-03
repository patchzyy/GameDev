using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TheCure.Weapons;

namespace TheCure
{
    public static class DefaultSettings
    {
        public static readonly Dictionary<SettingKey, object> Values = new()
        {
            // PLAYER
            { SettingsConst.PLAYER.MAX_HEALTH, 100f },
            { SettingsConst.PLAYER.MOVE_SPEED, 300f },

            // ZOMBIE
            { SettingsConst.ZOMBIE.SPEED, 60f },
            { SettingsConst.ZOMBIE.STAGGER, 1f },
            { SettingsConst.ZOMBIE.ATTACK_DAMAGE, 10f },
            { SettingsConst.ZOMBIE.ATTACK_COOL_DOWN, 1f },
            { SettingsConst.ZOMBIE.START_HEALTH, 30f },
            { SettingsConst.ZOMBIE.MAX_HEALTH, 100f },

            // SPAWNING
            { SettingsConst.SPAWNING.ZOMBIE_SPAWN_INTERVAL, 2f },
            { SettingsConst.SPAWNING.ENEMIES_PER_WAVE, 5 },
            { SettingsConst.SPAWNING.MAX_ENEMIES_ON_SCREEN, 20 },
            { SettingsConst.SPAWNING.BRUTE_SPAWN_CHANCE, 0.1f },
            { SettingsConst.SPAWNING.MAX_BRUTES, 0 },

            // FRIENDLY
            { SettingsConst.FRIENDLY.FOLLOW_DISTANCE, 60f },
            { SettingsConst.FRIENDLY.MOVE_SPEED, 50f },
            { SettingsConst.FRIENDLY.START_HEALTH, 100f },
            { SettingsConst.FRIENDLY.MAX_HEALTH, 100f },
            { SettingsConst.FRIENDLY.ATTACK_DAMAGE, 10f },
            { SettingsConst.FRIENDLY.HEALTH_LOSS_PER_SECOND, 2f },
            { SettingsConst.FRIENDLY.SIZE, 1f },
            { SettingsConst.FRIENDLY.RADIUS, 20f },
            { SettingsConst.FRIENDLY.START_WEAPON, FriendlyWeapons.HandGun },
            
            // SingleBulletWeapon
            { SettingsConst.SINGLE_BULLET_WEAPON.DAMAGE, 10f },

            // VIDEO
            { SettingsConst.VIDEO.WIDTH, 1920 },
            { SettingsConst.VIDEO.HEIGHT, 1080 },
            { SettingsConst.VIDEO.DISPLAY_MODE, DisplayModeSetting.Windowed },

            // KEY BINDS
            //PLAYER ACTIONS
            { SettingsConst.KEY_BINDS.ACTION_1, Keys.D1 },
            { SettingsConst.KEY_BINDS.ACTION_2, Keys.D2 },
            { SettingsConst.KEY_BINDS.ACTION_3, Keys.D3 },
            { SettingsConst.KEY_BINDS.ACTION_4, Keys.D4 },
            { SettingsConst.KEY_BINDS.ACTION_5, Keys.D5 },
            { SettingsConst.KEY_BINDS.ACTION_6, Keys.D6 },
            { SettingsConst.KEY_BINDS.ACTION_7, Keys.D7 },
        };
    }
}