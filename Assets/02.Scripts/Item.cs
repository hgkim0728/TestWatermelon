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
            Destroy(gameObject);    // 인벤매니저로부터 이 아이템을 등록할 수 있다면 등록 후 삭제
        }
        else
        {
            Debug.Log("아이템 창이 가득 찼습니다.");
        }
    }
}
