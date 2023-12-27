using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    [SerializeField, Tooltip("동글이 번호")] private int dongleIndex = 0;

    private GameManager gameManager;
    private Rigidbody2D rb;

    private bool onSpawn = false;    // 첫 충돌이 발생하면 게임매니저에 새로운 동글이를 만들라고 알려주기 위한 변수
    private bool isDrop = false;
    private bool isMatch = false;

    #region 프로퍼티
    public int DongleIndex
    {
        get { return dongleIndex; }
        set { dongleIndex = value; }
    }

    public bool IsDrop
    {
        get { return isDrop; }
    }

    public bool IsMatch
    {
        set { isMatch = value; }
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(onSpawn == false && isDrop == true && isMatch == false
            && collision.gameObject.CompareTag("DeadLine") == false
            && collision.gameObject.CompareTag("Box") == false)
        {
            Debug.Log("스폰!!!!!");
            gameManager.Spawn = true;
            gameManager.CurrentDongleSet();
            onSpawn = true;
        }

        if(collision.gameObject.CompareTag("Dongle"))
        {
            Dongle sc = collision.gameObject.GetComponent<Dongle>();
            int idx = sc.DongleIndex;

            if (idx == dongleIndex && isDrop == true && sc.IsDrop == true)
            {
                // 한 단계 위의 동글로 교체
                gameManager.SumDongle(this.gameObject, collision.gameObject);
                // 점수 획득
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isDrop == true && collision.gameObject.CompareTag("DeadLine"))
        {
            gameManager.GameOver();
        }
    }

    private void CallCurrentDongleSet()
    {
        gameManager.CurrentDongleSet();
    }

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
        
    }

    public void Drop()
    {
        rb.gravityScale = 1;
        isDrop = true;
    }

    // 슈팅 게임 참고해서 동글이 화면 밖으로 나가지 못하게 막기
}
