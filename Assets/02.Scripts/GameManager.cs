using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public class UserScore
    {
        public int score;
        public string name;
    }

    [SerializeField, Tooltip("동글이 리스트")] List<GameObject> listDongleObj;
    [SerializeField, Tooltip("이번에 떨어뜨릴 동글")] private GameObject curDongle;
    [SerializeField, Tooltip("동글 오브젝트가 들어갈 레이어")] private Transform layerDongle;
    private Dongle curDongleSc;
    GameObject[] sumDongles = new GameObject[2];

    [SerializeField, Tooltip("UI 매니저")] private UIManager uiManager;

    private Camera mainCam;

    [SerializeField, Tooltip("선에 닿고 이만큼의 시간이 지나면 게임 오버")] private float timeGameOver = 2.0f;
    [SerializeField, Tooltip("테스트용. 끝나면 하이어라키에서 지울 것")] private int curScore = 0;

    private List<UserScore> listUserScore = new List<UserScore>();

    private string scoreKey = "ScoreKey";
    private int newRank = 0;

    private bool spawn = true;
    private bool gameOver = false;

    #region 프로퍼티
    public int Score
    {
        get { return curScore; }
    }

    public bool Spawn
    {
        set { spawn = value; }
    }
    #endregion

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        mainCam = Camera.main;
        CurrentDongleSet();
        //SetScore();
    }

    void Update()
    {
        if (gameOver == false)
        {
            OnClick();
        }
    }

    /// <summary>
    /// 클릭했을 때
    /// </summary>
    private void OnClick()
    {
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            curDongleSc.Drop();
            curDongle = null;
        }

        if(Input.GetKey(KeyCode.Mouse0) == false || curDongle == null)
        {
            return;
        }

        Vector2 mPos = Input.mousePosition;
        Vector2 pos = mainCam.ScreenToWorldPoint(mPos);
        //curDongle.transform.position = new Vector2(pos.x, curDongle.transform.position.y);
        CheckDonglePosition(pos);
    }

    /// <summary>
    /// 동글이가 화면 밖으로 나가지 않게 조절
    /// </summary>
    /// <param name="_mousePos">마우스 위치</param>
    private void CheckDonglePosition(Vector2 _mousePos)
    {
        Vector2 pos = mainCam.WorldToViewportPoint(_mousePos);

        if (pos.x > 1 - (curDongle.transform.localScale.x * 0.1f))
        {
            pos.x = 1 - curDongle.transform.localScale.x * 0.11f;
            curDongle.transform.position = new Vector2(mainCam.ViewportToWorldPoint(pos).x, curDongle.transform.position.y);
        }
        else if(pos.x < curDongle.transform.localScale.x * 0.1f)
        {
            pos.x = curDongle.transform.localScale.x * 0.11f;
            curDongle.transform.position = new Vector2(mainCam.ViewportToWorldPoint(pos).x, curDongle.transform.position.y);
        }
        else
        {
            curDongle.transform.position = new Vector2(mainCam.ViewportToWorldPoint(pos).x, curDongle.transform.position.y);
        }
    }

    /// <summary>
    /// 플레이어가 다음으로 떨어뜨릴 동글이를 랜덤으로 선정
    /// </summary>
    public void CurrentDongleSet()
    {
        // 다음 차례의 동글을 정하는 
        if(spawn == false)
        {
            return;
        }

        int idx = Random.Range(0, 4);
        curDongle = Instantiate(listDongleObj[idx], new Vector2(0, 4.0f), Quaternion.identity, layerDongle);
        curDongleSc = curDongle.GetComponent<Dongle>();
        curDongleSc.DongleIndex = idx;
        spawn = false;
        curDongleSc.DongleScore = SetDongleScore(idx);
        curDongleSc.TimeGameOver = timeGameOver;
    }

    /// <summary>
    /// 같은 동글이끼리 충돌했을 때 호출
    /// 한 단계 위의 동글이로 교체
    /// </summary>
    /// <param name="_dongle1">이 함수를 호출한 동글이</param>
    /// <param name="_dongle2">호출한 동글이와 충돌한 동글이</param>
    public void SumDongle(GameObject _dongle1, GameObject _dongle2, Vector2 _contactPoint)
    {
        if (sumDongles[0] != null)
        {
            for(int i = 0; i < 2; i++)
            {
                if (sumDongles[i] == _dongle1 || _dongle2 != null)
                {
                    for(int j = 0; j < 2; j++)
                    {
                        sumDongles[j] = null;
                    }
                    return;
                }
            }
        }

        sumDongles[0] = _dongle1;
        sumDongles[1] = _dongle2;

        Dongle _dongleSc = _dongle1.GetComponent<Dongle>();
        int idx = _dongleSc.DongleIndex;

        curScore += _dongleSc.DongleScore;

        if (idx == listDongleObj.Count - 1)
        {
            for(int i = 0; i < 2; i++)
            {
                sumDongles[i] = null;
            }

            return;
        }

        //Vector2 donglePos = new Vector2(_dongle1.transform.position.x, 
        //    _dongle1.transform.position.y + _dongle1.transform.localScale.y / 2);

        Vector2 donglePos = _contactPoint;

        for(int i = 0; i < 2; i++)
        {
            Destroy(sumDongles[i]);
        }

        GameObject obj = Instantiate(listDongleObj[idx + 1], donglePos, Quaternion.identity, layerDongle);
        Dongle objSc = obj.GetComponent<Dongle>();
        objSc.Drop();
        objSc.DongleIndex = idx + 1;
        objSc.DongleScore = SetDongleScore(idx + 1);
        objSc.TimeGameOver = timeGameOver;
        objSc.OnSpawn = true;
    }

    /// <summary>
    /// 동글이 점수 설정
    /// </summary>
    /// <param name="_idx">동글이 번호</param>
    /// <returns></returns>
    private int SetDongleScore(int _idx)
    {
        int score = (_idx + 1) * 10;
        return score;
    }

    /// <summary>
    /// 키가 있는지 확인하고
    /// 저장된 순위를 가져온다
    /// </summary>
    private void SetScore()
    {
        if(PlayerPrefs.HasKey(scoreKey))
        {
            string savedValue = PlayerPrefs.GetString(scoreKey);

            if(savedValue == string.Empty)
            {
                ClearScore();
            }
            else
            {
                listUserScore = JsonConvert.DeserializeObject<List<UserScore>>(savedValue);
            }
        }
        else
        {
            ClearScore();
        }
    }

    private int GetPlayerRank()
    {
        int count = listUserScore.Count;

        for(int i = 0; i < count; i++)
        {
            UserScore userScore = listUserScore[i];

            if(curScore > userScore.score)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetNewRank(string _name)
    {
        listUserScore.Insert(newRank, new UserScore(){ name = _name, score = curScore });
        listUserScore.RemoveAt(listUserScore.Count - 1);

        string saveValue = JsonConvert.SerializeObject(listUserScore);
        PlayerPrefs.SetString(scoreKey, saveValue);
    }

    private void ClearScore()
    {
        listUserScore.Clear();

        for(int i = 0; i < 10; i++)
        {
            listUserScore.Add(new UserScore());
        }

        string saveValue = JsonConvert.SerializeObject(listUserScore);
        PlayerPrefs.SetString(scoreKey, saveValue);
    }

    public void CheckRank()
    {
        int rank = GetPlayerRank();

        if(rank == -1)
        {
            uiManager.NotRank(curScore);
        }
        else
        {
            newRank = rank;
            uiManager.NewRank(rank, curScore);
        }
    }

    public void GameOver()
    {
        if (gameOver == false)
        {
            uiManager.ActiveGameOverImg();
            gameOver = true;
        }
    }
}
