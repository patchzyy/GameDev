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

            // BRUTE
            { SettingsConst.BRUTE.SPEED, 30f },
            { SettingsConst.BRUTE.STAGGER, 1.2f },
            { SettingsConst.BRUTE.ATTACK_DAMAGE, 30f },
            { SettingsConst.BRUTE.ATTACK_COOLDOWN, 2f },
            { SettingsConst.BRUTE.START_HEALTH, 200f },
            { SettingsConst.BRUTE.MAX_HEALTH, 200f },

            // BABY ZOMBIE
            { SettingsConst.BABY_ZOMBIE.MOVE_SPEED_MULTIPLIER, 2.0f },
            { SettingsConst.BABY_ZOMBIE.HEALTH_MULTIPLIER, 0.2f },
            { SettingsConst.BABY_ZOMBIE.SCALE, 1.25f },

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
            // PLAYER ACTIONS
            { SettingsConst.KEY_BINDS.SHOOT, InputBinding.FromMouse(MouseButton.Left) },
            { SettingsConst.KEY_BINDS.DASH, InputBinding.FromKey(Keys.LeftShift) },

            { SettingsConst.KEY_BINDS.ACTION_1, InputBinding.FromKey(Keys.D1) },
            { SettingsConst.KEY_BINDS.ACTION_2, InputBinding.FromKey(Keys.D2) },
            { SettingsConst.KEY_BINDS.ACTION_3, InputBinding.FromKey(Keys.D3) },
            { SettingsConst.KEY_BINDS.ACTION_4, InputBinding.FromKey(Keys.D4) },
            { SettingsConst.KEY_BINDS.ACTION_5, InputBinding.FromKey(Keys.D5) },

            // HEAL BOMB
            { SettingsConst.HEAL_BOMB.HEALING, 10f },
            { SettingsConst.HEAL_BOMB.RADIUS, 50 },
            { SettingsConst.HEAL_BOMB.TICKS, 5 },
            // HEAL BOMB TRAP (separate from throwable heal bomb)
            { SettingsConst.HEAL_BOMB_TRAP.HEALING, 5f },
            { SettingsConst.HEAL_BOMB_TRAP.TICK_INTERVAL, 0.4f },
            { SettingsConst.HEAL_BOMB_TRAP.RADIUS, 120 },

            // FREEZE TRAP
            { SettingsConst.FREEZE_TRAP.SLOW_FACTOR, 0.4f },
            { SettingsConst.FREEZE_TRAP.DURATION, 2.5f },

            // ELECTRIC TRAP
            { SettingsConst.ELECTRIC_TRAP.DAMAGE_PER_TICK, 8 },
            { SettingsConst.ELECTRIC_TRAP.DAMAGE_TICK_INTERVAL, 0.3f },
            { SettingsConst.ELECTRIC_TRAP.STUN_DURATION, 0.8f },
            { SettingsConst.ELECTRIC_TRAP.STUN_FORCE, 300f },

            // BOMB TRAP
            { SettingsConst.BOMB_TRAP.ACTIVATION_DELAY, 0.7f },
            { SettingsConst.BOMB_TRAP.EXPLOSION_DAMAGE, 25 },
            { SettingsConst.BOMB_TRAP.EXPLOSION_RADIUS, 100f },
            { SettingsConst.BOMB_TRAP.EXPLOSION_FADE_DURATION, 0.3f },

            // SPIKE TRAP
            { SettingsConst.SPIKE_TRAP.DAMAGE_PER_HIT, 15 },
            { SettingsConst.SPIKE_TRAP.DAMAGE_INTERVAL, 0.5f },
        };
    }
}