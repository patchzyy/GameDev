using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Random = System.Random;

namespace TheCure.Managers
{
    public class SoundManager : Manager<SoundManager>
    {
        public SoundEffect _playerShot;
        public SoundEffect _friendlyShot;
        public SoundEffect _zombieHit;
        public SoundEffect _entityHeal;
        public SoundEffect _zombieDeath;
        public SoundEffect _friendlyHit;
        public SoundEffect _upgradeUnlock;

        public void Load()
        {
            var content = ContentsManager.Get().GetContent();

            _playerShot = content.Load<SoundEffect>("SFX/PlayerBullet");
            _friendlyShot = content.Load<SoundEffect>("SFX/Gunshot");
            _zombieHit = content.Load<SoundEffect>("SFX/ZombieHit");
            _entityHeal = content.Load<SoundEffect>("SFX/ZombieHeal");
            _zombieDeath = content.Load<SoundEffect>("SFX/ZombieDying");
            _friendlyHit = content.Load<SoundEffect>("SFX/FriendlyHit");
            _upgradeUnlock = content.Load<SoundEffect>("SFX/UpgradeUnlock");
        }

        // https://pixabay.com/sound-effects/film-special-effects-grenade-launcher-106342/
        public void PlayPlayerShoot()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _playerShot.Play(0.15f, pitch, 0f);
        }

        // https://f8studios.itch.io/snakes-second-authentic-gun-sounds-pack
        public void PlayFriendlyShoot()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _friendlyShot.Play(0.15f, pitch, 0f);
        }

        // https://pixabay.com/sound-effects/horror-zombie-call-357977/
        public void PlayZombieHit()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _zombieHit?.Play(0.5f, pitch, 0f);
        }

        // https://pixabay.com/sound-effects/film-special-effects-healpop-46004/
        public void PlayHeal()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _entityHeal?.Play(0.1f, pitch, 0f);
        }

        // https://pixabay.com/sound-effects/horror-zombie-call-357977/
        public void PlayZombieDeath()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _zombieDeath?.Play(0.6f, pitch, 0f);
        }

        // https://pixabay.com/sound-effects/film-special-effects-punch-02-123106/
        public void PlayFriendlyHit()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _friendlyHit?.Play(0.1f, pitch, 0f);
        }

        // https://pixabay.com/sound-effects/upgrade-unlock-106342/
        public void PlayUpgradeUnlock()
        {
            _upgradeUnlock?.Play(0.1f, 0f, 0f);
        }
    }
}