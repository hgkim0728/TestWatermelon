using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField, Tooltip("점수 표시")] private TMP_Text textScore;

    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        SetTextScore();
    }

    private void SetTextScore()
    {
        textScore.text = gameManager.Score.ToString();
    }
}
