using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;

    public int score_1= 0;
    public int score_2= 0;
    public TextMeshProUGUI scoreText_1;
    public TextMeshProUGUI scoreText_2;
    void Awake() {
        Instance = this;
    }

    public void AddScore_1(int value) {
        score_1 += value;
        scoreText_1.text = "Score: " + score_1;
    }

    public void AddScore_2(int value) {
        score_2 += value;
        scoreText_2.text = "Score: " + score_2;
    }
}