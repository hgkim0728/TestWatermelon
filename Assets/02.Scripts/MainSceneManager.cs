using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using static GameManager;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private Button btnSatartGame;
    [SerializeField] private Button btnRank;
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnRankViewExit;

    [SerializeField] private GameObject objRankView;
    [SerializeField] private GameObject objRankContents;

    [SerializeField] private Transform trsRankContents;

    private List<UserScore> listScore = new List<UserScore>();
    private string scoreKey = "ScoreKey";

    public enum enumScene
    {
        MainScene,
        PlayScene
    }

    private void Awake()
    {
        #region 버튼 설정
        MainSceneManager sc = GetComponent<MainSceneManager>();

        btnSatartGame.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync((int)enumScene.PlayScene);
        });

        btnRank.onClick.AddListener(() =>
        {
            objRankView.SetActive(true);
        });

        btnExit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        #endregion
        SetRank();
        InitRank();
    }

    private void InitRank()
    {
        ClearAllRank();
        CreateRankContents();
    }

    private void ClearAllRank()
    {
        int count = trsRankContents.childCount;

        for(int i = count - 1; i > -1; i--)
        {
            Destroy(trsRankContents.GetChild(i).gameObject);
        }
    }

    private void SetRank()
    {
        if(PlayerPrefs.HasKey(scoreKey))
        {
            string scoreValue = PlayerPrefs.GetString(scoreKey);

            if(scoreValue == string.Empty)
            {
                ClearAllScore();
            }
            else
            {
                listScore = JsonConvert.DeserializeObject<List<UserScore>>(scoreValue);

                if (listScore.Count != 10)
                {
                    Debug.LogError($"리스트 스코어의 갯수가 이상합니다. 리스트 스코어의 갯수 = {listScore.Count}");
                }
            }
        }
        else
        { 
            ClearAllScore(); 
        }
    }

    private void ClearAllScore()
    {
        listScore.Clear();

        for(int i = 0; i < 10; i++)
        {
            listScore.Add(new UserScore());
        }

        string scoreValue = JsonConvert.SerializeObject(listScore);
        PlayerPrefs.SetString(scoreKey, scoreValue );
    }

    private void CreateRankContents()
    {

    }
}
