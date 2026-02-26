namespace Core;

public class ScoreManager
{
    public int Score { get; private set; }

    public void AddScore(int points)
    {
        if (points > 0)
            Score += points;
    }

    public void Reset()
    {
        Score = 0;
    }
}
