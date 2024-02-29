using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    #region 변수
    [SerializeField, Tooltip("동글이 번호")] private int dongleIndex = 0;

    private GameManager gameManager;    // 게임매니저
    private Rigidbody2D rb;

    private float timeContactDeadLine = 0;  // 탈락선에 닿은 시간
    private float timeGameOver = 3.0f;  // 탈락선에 닿아도 되는 제한시간

    private int dongleScore = 1;    // 동글이가 합체하면 획득하게 되는 점수

    // 첫 충돌이 발생하면 게임매니저에게 새로운 동글이를 만들라고 알려주기 위한 변수
    [SerializeField]private bool onSpawn = false;
    [SerializeField]private bool isDrop = false;    // 떨어뜨린 동글이인지 아닌지 알기 위한 변수
    // 같은 종류의 동글이와 부딪히기 전인지 후인지 알기 위한 변수
    [SerializeField] private bool isMatch = false;

    private bool lineContect = false;   // 동글이가 탈락선에 닿았는지 아닌지
    #endregion

    #region 프로퍼티
    // 탈락선에 닿아도 되는 제한시간
    public float TimeGameOver
    {
        set { timeGameOver = value; }
    }

    // 동글이가 합체하면 획득하는 점수
    public int DongleScore
    {
        get { return dongleScore; }
        set { dongleScore = value; }
    }

    // 몇번째로 큰 동글이인지
    public int DongleIndex
    {
        get { return dongleIndex; }
        set { dongleIndex = value; }
    }

    // 떨어져서 다른 곳에 충돌했는지 아닌지
    public bool OnSpawn
    {
        set { onSpawn = value; }
    }

    // 떨어뜨린 동글이인지 아닌지
    public bool IsDrop
    {
        get { return isDrop; }
    }

    // 같은 동글이와 충돌했는지 아닌지
    public bool IsMatch
    {
        get { return isMatch; }
        set { isMatch = value; }
    }
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        // 탈락선에 닿았다면
        if (lineContect == true)
        {
            timeContactDeadLine += Time.deltaTime;  // 제한시간 카운트

            // 제한시간이 지났다면
            if(timeContactDeadLine >= timeGameOver)
            {
                gameManager.GameOver();     // 게임오버
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 동글이가 떨어졌고
        // 아직 다른 물체와 충돌하지 않았고
        // 충돌한 대상이 박스가 아니라면
        if(onSpawn == false && collision.gameObject.CompareTag("Box") == false 
            && isDrop == true)
        {
            // 게임매니저에게 새로운 동글이를 생산하라고 알려주기
            gameManager.CurrentDongleSet();
            onSpawn = true;     // 떨어져서 한 번 이상 다른 오브젝트와 충돌한 상태로 변경
        }

        // 충돌한 대상이 동글이고 자신이 아직 같은 크기의 동글이와 충돌하지 않았다면
        if(collision.gameObject.CompareTag("Dongle") && isMatch == false
            && gameManager.IsGameOver == false)
        {
            Dongle sc = collision.gameObject.GetComponent<Dongle>();    // 충돌한 상대 동글이의 스크립트 가져오기
            int idx = sc.DongleIndex;   // 상대가 몇 번째 동글이인지 가져오기
            ContactPoint2D contactPoint = collision.contacts[0];    // 충돌지점

            // 상대가 자신과 같은 크기이고
            // 자신과 상대 모두 떨어진 상태이고
            // 상대 또한 아직 같은 동글이와 충돌하지 않았다면
            if (idx == dongleIndex && isDrop == true && sc.IsDrop == true 
                && sc.IsMatch == false)
            {
                isMatch = true; // 자신이 같은 동글이와 충돌한 이후임
                sc.IsMatch = true;  // 상대 또한 같은 동글이와 충돌했음
                // 게임 매니저에게 같은 동글이가 충돌했음을 전달
                gameManager.SumDongle(this.gameObject, collision.gameObject, contactPoint.point);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ContactDeadLine(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ContactDeadLine(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 탈락선에서 떨어지면
        if(onSpawn == true && collision.gameObject.CompareTag("DeadLine"))
        {
            lineContect = false;    // 탈락선에 닿지 않은 상태로 변경
            timeContactDeadLine = 0;    // 제한시간 리셋
        }
    }

    /// <summary>
    /// Is Trigger가 활성화된 오브젝트가 닿았을 때 실행
    /// </summary>
    /// <param name="_collision">닿은 상대의 콜라이더</param>
    private void ContactDeadLine(Collider2D _collision)
    {
        // 떨어져서 다른 오브젝트와 한 번 이상 충돌한 적 있고
        // 닿은 상대가 탈락선이라면
        if (onSpawn == true && _collision.gameObject.CompareTag("DeadLine"))
        {
            lineContect = true; // 탈락선에 닿은 상태로 변경
        }
    }

    /// <summary>
    /// 플레이어가 클릭해서 떨어지거나
    /// 동글이가 합체해서 한 단계 큰 동글이가 생성됐을 때 실행
    /// </summary>
    public void Drop()
    {
        rb.gravityScale = 1;    // 중력을 1로
        isDrop = true;  // 떨어지 상태롭 변경
    }
}
