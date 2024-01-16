using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField, Tooltip("점수 표시")] private TMP_Text textScore;
    [SerializeField, Tooltip("게임 종료 이미지")] private GameObject gameOverImg;
    [SerializeField, Tooltip("게임 종료 패널")] private GameObject gameOverPanel;
    [SerializeField] private float timeActiveGameOverImg = 2.0f;

    [Header("게임종료 패널 안의 UI들")]
    [SerializeField, Tooltip("랭킹 텍스트. 게임 점수 10위 내에 들어갈 때만 활성화")] private TMP_Text textRank;
    [SerializeField, Tooltip("점수. 게임 진행 중 표시되는 것과는 다름")] private TMP_Text textGameOverScore;
    [SerializeField, Tooltip("랭킹에 들어갈 이름. 게임 점수 10위 내에 들어갈 때만 활성화")] private TMP_InputField inputFieldUserName;
    [SerializeField, Tooltip("랭킹에 들어갈 이름과 점수를 저장할 때 쓰는 버튼. 게임 점수 10위 내에 들어갈 때만 활성화")] private GameObject buttonScoreSave;
    [SerializeField, Tooltip("게임 재시작 버튼")] private GameObject buttonReplay;
    [SerializeField, Tooltip("나가기 버튼")] private GameObject buttonExit;

    GameManager gameManager;

    private bool activeGameOverImg = false;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        SetTextScore();

        if(activeGameOverImg)
        {
            timeActiveGameOverImg -= Time.deltaTime;

            if(timeActiveGameOverImg <= 0)
            {
                gameOverImg.SetActive(false);
                activeGameOverImg = false;
                gameManager.CheckRank();
            }
        }
    }

    private void SetTextScore()
    {
        textScore.text = gameManager.Score.ToString();
    }

    public void ActiveGameOverImg()
    {
        gameOverImg.SetActive(true);
        activeGameOverImg = true;
    }

    private void SetGameOverPanel(int _score)
    {
        textGameOverScore.text = _score.ToString();
        gameOverPanel.SetActive(true);
    }

    public void NewRank(int _rank, int _score)
    {
        textRank.text = (_rank + 1).ToString();
        textRank.gameObject.SetActive(true);
        inputFieldUserName.text = string.Empty;
        inputFieldUserName.gameObject.SetActive(true);
        buttonScoreSave.gameObject.SetActive(true);
        SetGameOverPanel(_score);
    }

    public void NotRank(int _score)
    {
        buttonReplay.gameObject.SetActive(true);
        buttonExit.gameObject.SetActive(true);
        SetGameOverPanel(_score);
    }

    public void SaveNewRank()
    {
        string name = inputFieldUserName.text;
        gameManager.SetNewRank(name);
        buttonReplay.gameObject.SetActive(true);
        buttonExit.gameObject.SetActive(true);
        inputFieldUserName.gameObject.SetActive(false);
        buttonScoreSave.SetActive(false);
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
