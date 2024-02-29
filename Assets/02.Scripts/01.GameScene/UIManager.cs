using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region 변수
    [SerializeField, Tooltip("점수 표시")] private TMP_Text textScore;
    [SerializeField, Tooltip("동글이 스프라이트")] List<Sprite> dongleSprite;
    [SerializeField, Tooltip("다음에 떨어뜨릴 동글이 표시")] Image imgNextDongle;
    [SerializeField, Tooltip("게임 종료 이미지")] private GameObject gameOverImg;
    [SerializeField, Tooltip("게임 종료 패널")] private GameObject gameOverPanel;
    [SerializeField] private float timeActiveGameOverImg = 2.0f;    // 게임오버 이미지가 보이는 시간

    [Header("게임종료 패널 안의 UI들")]
    [SerializeField, Tooltip("랭킹 텍스트. 게임 점수 10위 내에 들어갈 때만 활성화")] private TMP_Text textRank;
    [SerializeField, Tooltip("점수. 게임 진행 중 표시되는 것과는 다름")] private TMP_Text textGameOverScore;
    [SerializeField, Tooltip("랭킹에 들어갈 이름. 게임 점수 10위 내에 들어갈 때만 활성화")] private TMP_InputField inputFieldUserName;
    [SerializeField, Tooltip("랭킹에 들어갈 이름과 점수를 저장할 때 쓰는 버튼. 게임 점수 10위 내에 들어갈 때만 활성화")] private GameObject buttonScoreSave;
    [SerializeField, Tooltip("게임 재시작 버튼")] private GameObject buttonReplay;
    [SerializeField, Tooltip("나가기 버튼")] private GameObject buttonExit;

    GameManager gameManager;    // 게임매니저
    #endregion

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        SetTextScore();     // 현재 점수 표시
    }

    /// <summary>
    /// 현재 점수를 표시
    /// </summary>
    private void SetTextScore()
    {
        textScore.text = gameManager.Score.ToString();  // 게임매니저에서 현재 점수를 가져와 텍스트로 표시
    }

    /// <summary>
    /// 다음으로 떨어뜨릴 동글이 표시
    /// </summary>
    /// <param name="_nextDongleIdx"></param>
    public void SetNextDongleImg(int _nextDongleIdx)
    {
        imgNextDongle.sprite = dongleSprite[_nextDongleIdx];
    }

    /// <summary>
    /// 게임오버 이미지 활성화
    /// </summary>
    public void ActiveGameOverImg()
    {
        gameOverImg.SetActive(true);    // 게임오버 이미지 활성화

        Invoke("SleepGameOverImg", timeActiveGameOverImg);  // 정해둔 시간이 지난 후 게임오버 이미지 비활성화
    }

    /// <summary>
    /// 게임오버 이미지 비활성화
    /// </summary>
    private void SleepGameOverImg()
    {
        gameOverImg.SetActive(false);   // 게임오버 이미지 비활성화
        gameManager.CheckRank();    // 게임매니저에게 플러이어가 랭크 내에 들었는지 체크하게 하기
    }

    /// <summary>
    /// 게임오버 패널 활성화
    /// </summary>
    /// <param name="_score">플레이어의 점수</param>
    private void SetGameOverPanel(int _score)
    {
        textGameOverScore.text = _score.ToString();     // 플레이어가 달성한 점수 표시
        gameOverPanel.SetActive(true);  // 게임오버 패널 활성화
    }

    /// <summary>
    /// 플레이어가 랭크 내에 들었을 때 호출
    /// </summary>
    /// <param name="_rank">플레이어가 달성한 랭크</param>
    /// <param name="_score">플레이어의 점수</param>
    public void NewRank(int _rank, int _score)
    {
        textRank.text = (_rank + 1).ToString() + "등";     // 플레이어가 달성한 랭크 표시
        textRank.gameObject.SetActive(true);    // 랭크 텍스트 활성화
        inputFieldUserName.text = string.Empty;     // 플레이어 이름 입력창 비우기
        inputFieldUserName.gameObject.SetActive(true);  // 플레이어 이름 입력 인풋필드 활성화
        buttonScoreSave.gameObject.SetActive(true);     // 랭크 저장 버튼 활성화
        SetGameOverPanel(_score);   // 게임오버 패널 활성화
    }

    /// <summary>
    /// 플레이어가 랭크 내에 들어가지 못했을 때 호출
    /// </summary>
    /// <param name="_score">플레이어의 점수</param>
    public void NotRank(int _score)
    {
        buttonReplay.gameObject.SetActive(true);    // 다시하기 버튼 활성화
        buttonExit.gameObject.SetActive(true);  // 나가기 버튼 활성화
        SetGameOverPanel(_score);   // 게임오버 패널 활성화
    }

    /// <summary>
    /// 랭크 내에 들어간 플레이어가
    /// 이름을 입력하고 저장 버튼을 누르면 호출됨
    /// </summary>
    public void SaveNewRank()
    {
        string name = inputFieldUserName.text;  // 인풋필드에 입력된 플레이어 이름 가져오기
        gameManager.SetNewRank(name);   // 새로운 랭크 저장
        buttonReplay.gameObject.SetActive(true);    // 다시하기 버튼 활성화
        buttonExit.gameObject.SetActive(true);  // 나가기 버튼 활성화
        inputFieldUserName.gameObject.SetActive(false);     // 플레이어 이름 입력 인풋필드 비활성화
        buttonScoreSave.SetActive(false);   // 저장 버튼 비활성화
    }

    /// <summary>
    /// 다시하기 버튼을 눌렀을 때 실행
    /// </summary>
    public void Replay()
    {
        // 현재 씬 다시 불러오기
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    // 나가기 버튼을 눌렀을 때 실행
    public void Exit()
    {
        SceneManager.LoadSceneAsync(0);  // 메인씬으로 이동
    }
}
