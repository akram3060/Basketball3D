// FILEPATH: c:/Users/akram/GAMES_UNITY/Basketball3D/Assets/Scripts/ScoreManager.cs

using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int localScore = 0;
    public int opponentScore = 0;
    public TextMeshProUGUI scoreText;
    private GameSessionManager gameSessionManager;

    [SerializeField]
    private AudioClip scoreSound;
    private AudioSource audioSource;

    private void Start()
    {
        gameSessionManager = FindObjectOfType<GameSessionManager>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        localScore += points;
        UpdateScoreUI();
        if (gameSessionManager != null)
        {
            gameSessionManager.UpdateScore(localScore, opponentScore);
        }
        else
        {
            Debug.LogWarning("GameSessionManager not found. Unable to update score on server.");
        }

        PlayScoreSound();
    }

    public void UpdateScore(int newLocalScore, int newOpponentScore)
    {
        localScore = newLocalScore;
        opponentScore = newOpponentScore;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {localScore} - {opponentScore}";
        }
        else
        {
            Debug.LogWarning("ScoreText is not assigned in the ScoreManager.");
        }
    }

    private void PlayScoreSound()
    {
        if (audioSource != null && scoreSound != null)
        {
            audioSource.PlayOneShot(scoreSound);
        }
    }

    public void ResetScore()
    {
        localScore = 0;
        opponentScore = 0;
        UpdateScoreUI();
    }

    public int GetLocalScore()
    {
        return localScore;
    }

    public int GetOpponentScore()
    {
        return opponentScore;
    }
}

