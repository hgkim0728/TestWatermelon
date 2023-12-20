using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("�մ�������!")]
    [SerializeField, Tooltip("������Ʈ�� ���� �� �� ����ϴ� ������Ʈ")] GameObject objInventory;    // ������Ʈ ��ü
    [SerializeField, Tooltip("������ ������Ʈ")] GameObject objItem;    // �κ��丮�� ������ ������ ������Ʈ�� ��������
    [SerializeField] Transform trsInven;    // �κ��丮 �ʱ�ȭ�� ����� �κ��丮���� ��ġ ������

    private List<Transform> listInven = new List<Transform>();

    [SerializeField] KeyCode openInvenKey;  // �Է��� Ű�ڵ带 ���� �����ϴ� �͵� ����

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

    private void InitInve() // �ʱ�ȭ�� ����
    {
        listInven.Clear();
        listInven.AddRange(trsInven.GetComponentsInChildren<Transform>());
        listInven.RemoveAt(0);  // �ڵ带 ������ �� ������Ʈ�� Transform�� �������� ������ 0���� ����
                                // (0���� �� ������Ʈ�� Transform)
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

            //objInventory.SetActive(!objInventory.activeSelf);   ���� ������ �ݴ��
        }
    }

    private int GetEmptyItemSlot()
    {
        int count = listInven.Count;

        for(int i  = 0; i < count; i++)
        {
            Transform slot = listInven[i];

            if(slot.childCount == 0)    // �ڽ� ������Ʈ�� ���ٸ�
            {
                return i;
            }
        }

        return -1;
    }

    public bool GetItem(Sprite _spr)
    {
        int slotNum = GetEmptyItemSlot();   // ��� �ִ�, �� ä�� ������ ������ ��ȣ

        if(slotNum == -1)   // �������� �� ���� �� ����
        {
            return false;
        }

        InvenDragUI ui = Instantiate(objItem, listInven[slotNum]).GetComponent<InvenDragUI>();
        // ui ��ũ��Ʈ���� �� ������ ������ ��Ͻ�ų ����
        ui.SetItem(_spr);

        return true;
    }
}
