using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dongle : MonoBehaviour, IDragHandler, IDropHandler
{
    [SerializeField, Tooltip("������ ��ȣ")] private int dongleIndex = 0;

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
                // �� �ܰ� ���� ���۷� ��ü
                // ���� ȹ��
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
    /// �巡�׵� �� �۵�
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // �÷����� ������ �����ؼ� �ϼ�
    }

    /// <summary>
    /// ��ӵ� �� �۵�
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnDrop(PointerEventData eventData)
    {
        rb.gravityScale = 1;
    }

    // ���� ���� �����ؼ� ������ ȭ�� ������ ������ ���ϰ� ����
}
