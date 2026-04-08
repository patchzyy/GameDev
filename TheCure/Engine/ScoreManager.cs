namespace TheCure
{
    public class ScoreManager
    {
        private int _score;

        public void AddScore(int amount)
        {
            _score += amount;
        }

        public int GetScore()
        {
            return _score;
        }

        public void Reset()
        {
            _score = 0;
        }
    }
}