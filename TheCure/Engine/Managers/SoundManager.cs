using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Random = System.Random;

namespace TheCure.Managers
{
    public class SoundManager : Manager<SoundManager>
    {
        public SoundEffect _shoot;
        public SoundEffect _zombieHit;
        public SoundEffect _zombieDeath;
        // public SoundEffect PlayerHit;
        // public SoundEffect ButtonClick;

        public void Load()
        {
            var content = ContentsManager.Get().GetContent();

            _shoot = content.Load<SoundEffect>("SFX/Gunshot");
            _zombieHit = content.Load<SoundEffect>("SFX/ZombieHit");
            _zombieDeath = content.Load<SoundEffect>("SFX/ZombieDying");
            // PlayerHit = content.Load<SoundEffect>("Sounds/playerHit");
            // ButtonClick = content.Load<SoundEffect>("Sounds/buttonClick");
        }

        // https://f8studios.itch.io/snakes-second-authentic-gun-sounds-pack
        public void PlayShoot()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _shoot.Play(0.15f, pitch, 0f);
        }

        // https://pixabay.com/sound-effects/horror-zombie-call-357977/
        public void PlayZombieHit()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _zombieHit?.Play(0.5f, pitch, 0f);
        }

        // https://pixabay.com/sound-effects/horror-zombie-call-357977/
        public void PlayZombieDeath()
        {
            float pitch = Random.Shared.NextSingle() * 0.2f - 0.1f;
            _zombieDeath?.Play(0.6f, pitch, 0f);
        }

        // public void PlayPlayerHit()
        // {
        //     PlayerHit?.Play(0.6f, 0f, 0f);
        // }

        // public void PlayButtonClick()
        // {
        //     ButtonClick?.Play(0.4f, 0f, 0f);
        // }
    }
}