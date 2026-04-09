namespace TheCure
{
    public class ScorePopup
    {
        public string Text;
        public float TimeLeft;

        public ScorePopup(string text, float duration = 2f)
        {
            Text = text;
            TimeLeft = duration;
        }
    }
}