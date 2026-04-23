using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheCure.Managers;

namespace TheCure
{
    public class ScoreManager : Manager<ScoreManager>
    {
        private int _score;
        private List<ScorePopup> _scorePopups = new List<ScorePopup>();


        public void AddScore(int pointsToAdd, string reason = "")
        {
            _score += pointsToAdd;
            _scorePopups.Add(new ScorePopup($"{reason}  +{pointsToAdd}", 2f));
        }

        public int GetScore()
        {
            return _score;
        }

        public void Reset()
        {
            _score = 0;
        }

        public List<ScorePopup> GetScorePopups()
        {
            return _scorePopups;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = _scorePopups.Count - 1; i >= 0; i--)
            {
                _scorePopups[i].TimeLeft -= deltaTime;

                if (_scorePopups[i].TimeLeft <= 0)
                {
                    _scorePopups.RemoveAt(i);
                }
            }
        }
    }
}