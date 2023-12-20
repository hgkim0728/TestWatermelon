using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("손대지말것!")]
    [SerializeField, Tooltip("오브젝트를 껐다 켤 때 사용하는 오브젝트")] GameObject objInventory;    // 오브젝트 본체
    [SerializeField, Tooltip("프리팹 오브젝트")] GameObject objItem;    // 인벤토리에 생성될 프리팹 오브젝트의 오리지널
    [SerializeField] Transform trsInven;    // 인벤토리 초기화에 사용할 인벤토리들의 위치 데이터

    private List<Transform> listInven = new List<Transform>();

    [SerializeField] KeyCode openInvenKey;  // 입력할 키코드를 직접 설정하는 것도 가능

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        InitInve();
    }

    private void InitInve() // 초기화로 시작
    {
        listInven.Clear();
        listInven.AddRange(trsInven.GetComponentsInChildren<Transform>());
        listInven.RemoveAt(0);  // 코드를 실행한 이 오브젝트의 Transform도 가져오기 때문에 0번을 삭제
                                // (0번은 이 오브젝트의 Transform)
    }

    void Update()
    {
        OpenInventory();
    }

    private void OpenInventory()
    {
        if(Input.GetKeyDown(openInvenKey))
        {
            if(objInventory.activeSelf == true)
            {
                objInventory.SetActive(false);
            }
            else
            {
                objInventory.SetActive(true);
            }

            //objInventory.SetActive(!objInventory.activeSelf);   현재 상태의 반대로
        }
    }

    private int GetEmptyItemSlot()
    {
        int count = listInven.Count;

        for(int i  = 0; i < count; i++)
        {
            Transform slot = listInven[i];

            if(slot.childCount == 0)    // 자식 오브젝트가 없다면
            {
                return i;
            }
        }

        return -1;
    }

    public bool GetItem(Sprite _spr)
    {
        int slotNum = GetEmptyItemSlot();   // 비어 있는, 곧 채울 아이템 슬롯의 번호

        if(slotNum == -1)   // 아이템을 더 넣을 수 없음
        {
            return false;
        }

        InvenDragUI ui = Instantiate(objItem, listInven[slotNum]).GetComponent<InvenDragUI>();
        // ui 스크립트에서 이 아이템 정보를 등록시킬 예정
        ui.SetItem(_spr);

        return true;
    }
}
