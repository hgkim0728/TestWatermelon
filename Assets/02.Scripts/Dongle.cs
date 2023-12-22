using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dongle : MonoBehaviour, IDragHandler, IDropHandler
{
    [SerializeField, Tooltip("동글이 번호")] private int dongleIndex = 0;

    private GameManager gameManager;
    private Rigidbody2D rb;

    public int DongleIndex
    {
        get { return dongleIndex; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Dongle"))
        {
            Dongle sc = collision.gameObject.GetComponent<Dongle>();
            int idx = sc.DongleIndex;

            if(idx == dongleIndex)
            {
                // 한 단계 위의 동글로 교체
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

    /// <summary>
    /// 드래그될 때 작동
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // 플랫포머 게임을 참고해서 완성
    }

    /// <summary>
    /// 드롭될 때 작동
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnDrop(PointerEventData eventData)
    {
        rb.gravityScale = 1;
    }

    // 슈팅 게임 참고해서 동글이 화면 밖으로 나가지 못하게 막기
}
