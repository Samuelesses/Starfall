using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float score = 0f;
    private float misses = 0;

    public float Score => score;
    public float Misses => misses;
    public TextMeshProUGUI scoreCount;
    public TextMeshProUGUI missCount;

    void Start()
    {
    }

    void Update()
    {
        Debug.Log(score - (misses * 200));
    }

    public void AddScore(float amount)
    {
        score += amount;
        scoreCount.SetText($"Score: {score:F0}");
    }

    public void CountMiss()
    {
        misses += 1;
        missCount.SetText($"Misses: {misses:F0}");
    }
}
