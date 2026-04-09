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
            { SettingsConst.PLAYER.MAX_HEALTH, 10f },
            { SettingsConst.PLAYER.MOVE_SPEED, 300f },

            // ZOMBIE
            { SettingsConst.ZOMBIE.SPEED, 60f },
            { SettingsConst.ZOMBIE.STAGGER, 1f },
            { SettingsConst.ZOMBIE.ATTACK_DAMAGE, 1 },
            { SettingsConst.ZOMBIE.ATTACK_COOLDOWN, 1f },
            { SettingsConst.ZOMBIE.START_HEALTH, 3f },
            { SettingsConst.ZOMBIE.MAX_HEALTH, 10f },

            // FRIENDLY
            { SettingsConst.FRIENDLY.FOLLOW_DISTANCE, 60f },
            { SettingsConst.FRIENDLY.MOVE_SPEED, 80f },
            { SettingsConst.FRIENDLY.START_HEALTH, 10f },
            { SettingsConst.FRIENDLY.MAX_HEALTH, 10f },
            { SettingsConst.FRIENDLY.RADIUS, 20f },
            { SettingsConst.FRIENDLY.START_WEAPON, FriendlyWeapons.HandGun },

            // KEYBINDS
            //PLAYER ACTIONS
            { SettingsConst.KEYBINDS.ACTION_1, Keys.D1 },
            { SettingsConst.KEYBINDS.ACTION_2, Keys.D2 },
            { SettingsConst.KEYBINDS.ACTION_3, Keys.D3 },
            { SettingsConst.KEYBINDS.ACTION_4, Keys.D4 },
            { SettingsConst.KEYBINDS.ACTION_5, Keys.D5 },
            { SettingsConst.KEYBINDS.ACTION_6, Keys.D6 },
        };
    }
}