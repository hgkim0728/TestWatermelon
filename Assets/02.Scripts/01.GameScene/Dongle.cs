using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    [SerializeField, Tooltip("동글이 번호")] private int dongleIndex = 0;

    private GameManager gameManager;
    private Rigidbody2D rb;

    private float timeContactDeadLine = 0;
    private float timeGameOver = 3.0f;  // 게임오버 선에 동글이가 닿아도 되는 제한시간

    private int dongleScore = 1;

    [SerializeField]private bool onSpawn = false;    // 첫 충돌이 발생하면 게임매니저에 새로운 동글이를 만들라고 알려주기 위한 변수
    [SerializeField]private bool isDrop = false;
    [SerializeField] private bool isMatch = false;

    private bool lineContect = false;

    #region 프로퍼티
    public float TimeGameOver
    {
        set { timeGameOver = value; }
    }

    public int DongleScore
    {
        get { return dongleScore; }
        set { dongleScore = value; }
    }

    public int DongleIndex
    {
        get { return dongleIndex; }
        set { dongleIndex = value; }
    }

    public bool OnSpawn
    {
        set { onSpawn = value; }
    }

    //public bool IsDrop
    //{
    //    get { return isDrop; }
    //}

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
        if (lineContect == true)
        {
            timeContactDeadLine += Time.deltaTime;

            if(timeContactDeadLine >= timeGameOver)
            {
                gameManager.GameOver();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(onSpawn == false && collision.gameObject.CompareTag("Box") == false 
            && isDrop == true)
        {
            //gameManager.Spawn = true;
            gameManager.CurrentDongleSet();
            onSpawn = true;
        }

        if(collision.gameObject.CompareTag("Dongle") && isMatch == false)
        {
            Dongle sc = collision.gameObject.GetComponent<Dongle>();
            int idx = sc.DongleIndex;
            ContactPoint2D contactPoint = collision.contacts[0];

            if (idx == dongleIndex && isDrop == true && isMatch == false && sc.IsMatch == false)
            {
                isMatch = true;
                sc.IsMatch = true;
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
        if(onSpawn == true && collision.gameObject.CompareTag("DeadLine"))
        {
            lineContect = false;
            timeContactDeadLine = 0;
        }
    }

    private void ContactDeadLine(Collider2D _collision)
    {
        if (onSpawn == true && _collision.gameObject.CompareTag("DeadLine"))
        {
            lineContect = true;
        }
    }

    public void Drop()
    {
        rb.gravityScale = 1;
        isDrop = true;
    }
}
