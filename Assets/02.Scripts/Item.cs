using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    InventoryManager invenManager;
    SpriteRenderer sr;
    Sprite sprite;

    private void Awake()
    {
        invenManager = InventoryManager.Instance;
        sr = GetComponent<SpriteRenderer>();
        sprite = sr.sprite;
    }

    public void GetItem()
    {
        if (invenManager.GetItem(sprite) == true)
        {
            Destroy(gameObject);    // �κ��Ŵ����κ��� �� �������� ����� �� �ִٸ� ��� �� ����
        }
        else
        {
            Debug.Log("������ â�� ���� á���ϴ�.");
        }
    }
}
