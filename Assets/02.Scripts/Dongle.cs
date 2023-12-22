using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    [SerializeField, Tooltip("동글이 번호")] private int dongleIndex = 0;

    private GameManager gameManager;
    private Rigidbody2D rb;

    private int onSpawn = 0;    // 첫 충돌이 발생하면 게임매니저에 새로운 동글이를 만들라고 알려주기 위한 변수
    private bool drop = false;

    public int DongleIndex
    {
        get { return dongleIndex; }
        set { dongleIndex = value; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(onSpawn == 0 && drop == true)
        {
            Debug.Log("스폰!!!!!");
            gameManager.Spawn = true;
            gameManager.CurrentDongleSet();
            onSpawn++;
        }

        if(collision.gameObject.CompareTag("Dongle"))
        {
            Dongle sc = collision.gameObject.GetComponent<Dongle>();
            int idx = sc.DongleIndex;

            if(idx == dongleIndex)
            {
                // 한 단계 위의 동글로 교체
                gameManager.SumDongle(this.gameObject, collision.gameObject);
                // 점수 획득
            }
        }
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
        drop = true;
    }

    // 슈팅 게임 참고해서 동글이 화면 밖으로 나가지 못하게 막기
}
