using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region 변수
    public static GameManager Instance;     // 게임매니저 인스턴스

    // 랭킹 저장을 위한 클래스
    public class UserScore
    {
        public int score;   // 랭크를 달성한 유저의 점수
        public string name; // 랭크를 달성한 유저의 이름
    }

    [SerializeField, Tooltip("동글 프리팹 리스트")] List<GameObject> listDongleObj;
    [SerializeField, Tooltip("동글 오브젝트가 들어갈 레이어")] private Transform layerDongle;
    [SerializeField, Tooltip("떨어뜨릴 동글이 중 가장 큰 동글 인덱스(+1해서 입력해야 함)")] private int maxCurDongleIdx = 4;

    [SerializeField, Tooltip("UI 매니저")] private UIManager uiManager;


    [SerializeField, Tooltip("선에 닿고 이만큼의 시간이 지나면 게임 오버")] private float timeGameOver = 2.0f;

    // 유저 랭킹 저장 리스트
    private List<UserScore> listUserScore = new List<UserScore>();

    private Camera mainCam;     // 메인 카메라

    private GameObject curDongle;   // 이번에 떨어뜨릴 동글이
    private Dongle curDongleSc; // 이번에 떨어뜨릴 동글이의 동글 스크립트

    private int nextDongleIdx;  // 다음에 떨어뜨릴 동글이 번호
    private int curScore = 0;   // 현재 플레이어가 달성한 점수
    private int newRank = 0;    // 순위 내의 점수가 달성됐을 때 해당되는 랭크를 담을 변수

    private string scoreKey = "ScoreKey";   // 유저 랭크 저장 키

    private bool isGameOver = false;    // 게임 오버인지 아닌지
    #endregion

    #region 프로퍼티
    // 플레이어의 현재 점수의 프로퍼티
    public int Score
    {
        get { return curScore; }
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
        mainCam = Camera.main;  // 메인 카메라 변수에 메인 카메라 넣기
        SetUserRank();     // 이전에 달성했던 유저 랭크를 불러와서 문제가 없는지 확인
        NextDongleSet();
        CurrentDongleSet();     // 떨어뜨릴 동글이 생성
    }

    void Update()
    {
        // 게임오버되지 않았다면
        if (isGameOver == false)
        {
            OnClick();
        }
    }

    /// <summary>
    /// 클릭했을 때 동글이의 위치의 x값을 마우스의 x값이 되도록 하고
    /// 동글이를 떨어지게 한다.
    /// </summary>
    private void OnClick()
    {
        // 마우스를 왼쪽 버튼을 누른 상태였다가 손을 떼면
        if(Input.GetKeyUp(KeyCode.Mouse0) == true)
        {
            curDongleSc.Drop();     // 현재 선택된 동글이를 떨어뜨리고
            curDongle = null;   // curDongle 변수를 비운다
        }

        // 마우스 왼쪽 버튼을 클릭하지 않거나 curDongle 변수가 비어있으면 중단
        if(Input.GetKey(KeyCode.Mouse0) == false || curDongle == null)
        {
            return;
        }


        Vector2 mPos = Input.mousePosition;     // 현재 마우스 위치를 가져옴
        Vector2 pos = mainCam.ScreenToWorldPoint(mPos);     // 마우스의 위치를 스크린에서 월드 기준으로 변경
        CheckDonglePosition(pos);   // 마우스의 위치에 따라서 동글이 위치 변경
    }

    /// <summary>
    /// 동글이의 위치의 x값을 마우스의 x값으로 바꾸고
    /// 동글이가 화면 밖으로 나가지 않게 조절
    /// </summary>
    /// <param name="_mousePos">마우스 위치</param>
    private void CheckDonglePosition(Vector2 _mousePos)
    {
        Vector2 pos = mainCam.WorldToViewportPoint(_mousePos);  // 마우스의 위치를 월드에서 뷰포트 기준으로 변경

        // 마우스 위치가 오른쪽 화면 밖이라면 동글이가 오른쪽 화면 끝에 붙도록
        if (pos.x > 1 - (curDongle.transform.localScale.x * 0.1f))
        {
            pos.x = 1 - curDongle.transform.localScale.x * 0.11f;
            curDongle.transform.position = new Vector2(mainCam.ViewportToWorldPoint(pos).x, curDongle.transform.position.y);
        }
        else if(pos.x < curDongle.transform.localScale.x * 0.1f)// 마우스 위치가 왼쪽 화면 밖이라면 동글이가 왼쪽 화면 끝에 붙도록
        {
            pos.x = curDongle.transform.localScale.x * 0.11f;
            curDongle.transform.position = new Vector2(mainCam.ViewportToWorldPoint(pos).x, curDongle.transform.position.y);
        }
        else// 마우스가 화면 안에 있다면 그 x값이 동글이 위치의 x값이 되도록
        {
            curDongle.transform.position = new Vector2(mainCam.ViewportToWorldPoint(pos).x, curDongle.transform.position.y);
        }
    }

    /// <summary>
    /// 플레이어가 다음으로 떨어뜨릴 동글이를 랜덤으로 선정
    /// </summary>
    public void CurrentDongleSet()
    {
        // 가장 작은 동글이부터 떨어뜨릴 수 있는 가장 큰 동글이 인덱스 중 랜덤으로 선정
        int idx = nextDongleIdx;
        NextDongleSet();
        uiManager.SetNextDongleImg(nextDongleIdx);

        // 동글이 생성
        curDongle = Instantiate(listDongleObj[idx], new Vector2(0, 4.0f), Quaternion.identity, layerDongle);
        curDongleSc = curDongle.GetComponent<Dongle>();     // 동글이 스크립트 가져오기
        curDongleSc.DongleIndex = idx;      // 생성된 동글이의 번호 전달(자신이 몇 번째로 큰 동글인지를 알려주는 용도)
        curDongleSc.DongleScore = SetDongleScore(idx);  // 합체할 때 획득할 점수 전달
        curDongleSc.TimeGameOver = timeGameOver;    // 탈락선에 몇 초 동안 접촉하면 탈락인지 전달
    }

    /// <summary>
    /// 다음으로 떨어뜨릴 동글이 설정
    /// </summary>
    private void NextDongleSet()
    {
        nextDongleIdx = Random.Range(0, maxCurDongleIdx);
    }

    /// <summary>
    /// 같은 동글이끼리 충돌했을 때 호출
    /// 한 단계 위의 동글이로 교체
    /// </summary>
    /// <param name="_dongle1">이 함수를 호출한 동글이</param>
    /// <param name="_dongle2">호출한 동글이와 충돌한 동글이</param>
    /// <param name="_contactPoint">두 동글이가 충돌한 점의 좌표</param>
    public void SumDongle(GameObject _dongle1, GameObject _dongle2, Vector2 _contactPoint)
    {
        Dongle _dongleSc = _dongle1.GetComponent<Dongle>();     // 함수를 호출한 동글이의 스크립트 가져오기
        int idx = _dongleSc.DongleIndex;    // 몇 번째로 큰 동글이인지 전달 받기

        curScore += _dongleSc.DongleScore;      // 현재까지 획득한 점수에 이 동글이의 점수 추가

        // 제일 큰 동글이라면
        if (idx == listDongleObj.Count - 1)
        {
            Destroy(_dongle2.gameObject);   // 충돌한 상대 동글이를 제거

            _dongleSc.IsMatch = false;  // 호출한 동글이는 다시 이 함수를 호출할 수 있게
            return;
        }
        else// 제일 큰 동글이가 아니라면
        {
            // 충돌한 모든 동글이 제거
            Destroy(_dongle1.gameObject);
            Destroy(_dongle2.gameObject);
        }

        // 충돌한 동글이보다 한 단계 더 큰 동글이 생성
        GameObject obj = Instantiate(listDongleObj[idx + 1], _contactPoint, Quaternion.identity, layerDongle);
        Dongle objSc = obj.GetComponent<Dongle>();  // 스크립트 가져오기
        objSc.Drop();   // 이미 떨어진 동글이라는 걸 알려주기
        objSc.DongleIndex = idx + 1;    // 몇 번째로 큰 동글인지 전달
        objSc.DongleScore = SetDongleScore(idx + 1);    // 합체하면 획득할 점수 전달
        objSc.TimeGameOver = timeGameOver;  // 탈락선에 몇 초 닿으면 달락인지 전달
        objSc.OnSpawn = true;   // 이미 떨어진 동글이라는 걸 알려주기
    }

    /// <summary>
    /// 동글이 점수 설정
    /// </summary>
    /// <param name="_idx">동글이 번호</param>
    /// <returns></returns>
    private int SetDongleScore(int _idx)
    {
        int score = (_idx + 1) * 10;    // 몇 번째로 큰 동글인지 * 10
        return score;
    }

    /// <summary>
    /// 키가 있는지 확인하고
    /// 저장된 순위를 가져온다
    /// </summary>
    private void SetUserRank()
    {
        // 유저 랭크 저장용 키가 있다면
        if(PlayerPrefs.HasKey(scoreKey))
        {
            // 저장된 유저 랭크 값 가져오기
            string savedValue = PlayerPrefs.GetString(scoreKey);

            // 저장된 값이 없거나 잘못됐다면
            if(savedValue == string.Empty)
            {
                // 잘못된 데이터를 삭제하고 다시 생성
                ClearScore();
            }
            else// 제대로 된 값이라면
            {
                // 유저 랭크 리스트에 정리
                listUserScore = JsonConvert.DeserializeObject<List<UserScore>>(savedValue);
            }
        }
        else// 키가 없다면
        {
            // 유저 랭크 데이터 생성
            ClearScore();
        }
    }

    /// <summary>
    /// 게임이 종료 된 후 플레이어가 달성한 점수가
    /// 랭크 내에 들었는지 확인
    /// </summary>
    /// <returns>랭크 내에 들었다면 달성한 랭크를 반환
    /// 랭크 내에 들지 못했다면 -1을 반환</returns>
    private int GetPlayerRank()
    {
        int count = listUserScore.Count;    // 저장되는 랭크의 범위(10)

        for(int i = 0; i < count; i++)
        {
            UserScore userScore = listUserScore[i];

            // 플레이어의 점수가 랭크 i의 유저의 점수보다 높다면
            if(curScore > userScore.score)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 플레이어가 랭크 내에 들었고
    /// 플레이어가 이름을 입력했다면
    /// </summary>
    /// <param name="_name">새로 랭크를 달성한 유저의 이름</param>
    public void SetNewRank(string _name)
    {
        // 새로운 랭크를 달성한 유저를 리스트에 추가
        listUserScore.Insert(newRank, new UserScore(){ name = _name, score = curScore });
        listUserScore.RemoveAt(listUserScore.Count - 1);    // 랭크에서 벗어난 유저를 리스트에서 삭제

        // 새로운 랭크 리스트를 저장
        string saveValue = JsonConvert.SerializeObject(listUserScore);
        PlayerPrefs.SetString(scoreKey, saveValue);
    }

    /// <summary>
    /// 잘못된 랭크 정보를 지우고
    /// 또는 랭크 정보가 존재 하지 않으면
    /// 새로 랭크 리스트를 만들고 저장
    /// </summary>
    private void ClearScore()
    {
        // 잘못된 랭크 리스트 비우기
        listUserScore.Clear();

        // 랭크 리스트 채우기
        for(int i = 0; i < 10; i++)
        {
            listUserScore.Add(new UserScore());
        }

        // 랭크 리스트 저장
        string saveValue = JsonConvert.SerializeObject(listUserScore);
        PlayerPrefs.SetString(scoreKey, saveValue);
    }

    /// <summary>
    /// 게임오버 됐을 때 플레이어가 랭크 내에 들었는지 체크
    /// </summary>
    public void CheckRank()
    {
        // 플레이어가 랭크 내에 들었다면 달성한 랭크
        // 랭크 내에 들어가지 못했다면 -1
        int rank = GetPlayerRank();

        // 랭크 내에 들어가지 못했다면
        if(rank == -1)
        {
            uiManager.NotRank(curScore);    // 이름 입력 및 저장을 생략하고 다시하기 & 나가기 버튼 활성화
        }
        else// 랭크 내에 들어갔다면
        {
            newRank = rank; // 달성한 랭크 전달
            uiManager.NewRank(rank, curScore);  // 플레이어 이름 입력 및 저장으로 넘어감
        }
    }

    /// <summary>
    /// 동글이가 탈락선에 닿고 정해둔 시간이 지났을 때 호출
    /// </summary>
    public void GameOver()
    {
        // 게임오버된 상태가 아니라면
        if (isGameOver == false)
        {
            uiManager.ActiveGameOverImg();  // 게임오버 이미지 활성화
            isGameOver = true;  // 게임오버 상태로 전환
        }
    }
}
