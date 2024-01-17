using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using static GameManager;

public class MainSceneManager : MonoBehaviour
{
    #region 변수
    [SerializeField, Tooltip("게임시작 버튼")] private Button btnGameStart;
    [SerializeField, Tooltip("랭크 버튼")] private Button btnRank;
    [SerializeField, Tooltip("게임 나가기 버튼")] private Button btnExit;
    [SerializeField, Tooltip("랭크 나가기 버튼")] private Button btnRankViewExit;

    [SerializeField, Tooltip("랭크 패널")] private GameObject objRankView;
    [SerializeField, Tooltip("유저 랭크 프리팹")] private GameObject objRankContents;

    [SerializeField, Tooltip("유저 랭크 오브젝트가 들어갈 곳")] private Transform trsRankContents;

    // 유저 랭크 리스트
    private List<UserScore> listScore = new List<UserScore>();
    private string scoreKey = "ScoreKey";   // 유저 랭크 저장 키

    // 씬 enum
    public enum enumScene
    {
        MainScene,
        PlayScene
    }
    #endregion

    private void Awake()
    {
        #region 버튼 설정
        MainSceneManager sc = GetComponent<MainSceneManager>();

        // 게임 시작 버튼을 누르면
        btnGameStart.onClick.AddListener(() =>
        {
            // 게임 씬으로 이동
            SceneManager.LoadSceneAsync((int)enumScene.PlayScene);
        });

        // 랭크 버튼을 누르면
        btnRank.onClick.AddListener(() =>
        {
            // 랭크 패널 활성화
            objRankView.SetActive(true);
        });

        // 나가기 버튼을 누르면
        btnExit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            // 유니티 에디터에서 게임을 실행중이라면
            // 에디터 내에서 게임 종료
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        // 랭크 나가기 버튼을 눌렀다면
        btnRankViewExit.onClick.AddListener(() =>
        {
            // 랭크 패널 비활성화
            objRankView.SetActive(false);
        });
        #endregion
        SetRank();
        InitRank();
    }

    /// <summary>
    /// 유저 랭크 정보를 가져오고
    /// 표시할 준비
    /// </summary>
    private void InitRank()
    {
        ClearAllRank();
        CreateRankContents();
    }

    /// <summary>
    /// 이전에 만들었던 유저 랭크 오브젝트를 전부 지우기
    /// </summary>
    private void ClearAllRank()
    {
        int count = trsRankContents.childCount;

        for(int i = count - 1; i > -1; i--)
        {
            Destroy(trsRankContents.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 저장된 랭크 정보 가져와서 리스트 채우기
    /// </summary>
    private void SetRank()
    {
        // 랭크 저장 키가 있다면
        if(PlayerPrefs.HasKey(scoreKey))
        {
            // 저장된 정보 가져오기
            string scoreValue = PlayerPrefs.GetString(scoreKey);

            // 값이 비었거나 잘못됐다면
            if(scoreValue == string.Empty)
            {
                ClearAllScore();
            }
            else// 값이 정상이라면
            {
                // 가져온 값으로 리스트 채우기
                listScore = JsonConvert.DeserializeObject<List<UserScore>>(scoreValue);

                if (listScore.Count != 10)
                {
                    Debug.LogError($"리스트 스코어의 갯수가 이상합니다. 리스트 스코어의 갯수 = {listScore.Count}");
                }
            }
        }
        else// 랭크 저장 키가 없다면
        { 
            ClearAllScore(); 
        }
    }

    /// <summary>
    /// 저장된 랭크 정보가 이상하거나
    /// 저장된 정보가 없다면 실행
    /// </summary>
    private void ClearAllScore()
    {
        listScore.Clear();  // 잘못 저장된 리스트를 비우고

        // 정상적인 값으로 채우기
        for(int i = 0; i < 10; i++)
        {
            listScore.Add(new UserScore());
        }

        // 저장
        string scoreValue = JsonConvert.SerializeObject(listScore);
        PlayerPrefs.SetString(scoreKey, scoreValue );
    }

    /// <summary>
    /// 랭크 오브젝트 생성
    /// </summary>
    private void CreateRankContents()
    {
        int count = listScore.Count;

        for(int i = 0; i < count; i++)
        {
            UserScore data = listScore[i];  // 리스트에 저장된 유저 정보 가져오기
            GameObject obj = Instantiate(objRankContents, trsRankContents);     // 랭크 오브젝트 생성
            RankContents objSc = obj.GetComponent<RankContents>();  // 랭크 오브젝트의 스크립트 가져오기
            objSc.SetRankContents($"{i + 1}", $"{data.score}", $"{data.name}");     // 유저 정보 입력
        }
    }
}
