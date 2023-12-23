using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField, Tooltip("동글이 리스트")] List<GameObject> listDongleObj;

    [SerializeField, Tooltip("이번에 떨어뜨릴 동글")] private GameObject curDongle;
    private Dongle curDongleSc;
    GameObject[] sumDongles = new GameObject[2];

    private Camera mainCam;

    private bool spawn = true;

    public bool Spawn
    {
        set { spawn = value; }
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        mainCam = Camera.main;
        CurrentDongleSet();
    }

    void Update()
    {
        OnClick();
    }

    /// <summary>
    /// 클릭했을 때
    /// </summary>
    private void OnClick()
    {
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            curDongleSc.Drop();
        }

        if(Input.GetKey(KeyCode.Mouse0) == false)
        {
            return;
        }

        Vector2 mPos = Input.mousePosition;
        Vector2 pos = mainCam.ScreenToWorldPoint(mPos);
        curDongle.transform.position = new Vector2(pos.x, curDongle.transform.position.y);

        
    }

    /// <summary>
    /// 플레이어가 다음으로 떨어뜨릴 동글이를 랜덤으로 선정
    /// </summary>
    public void CurrentDongleSet()
    {
        // 다음 차례의 동글을 정하는 
        if(spawn == false)
        {
            return;
        }

        curDongle = null;

        int idx = Random.Range(0, listDongleObj.Count);
        curDongle = Instantiate(listDongleObj[idx]);
        curDongleSc = curDongle.GetComponent<Dongle>();
        curDongle.transform.position = new Vector2(0, 4.5f);
        curDongleSc.DongleIndex = idx;
        spawn = false;
    }

    /// <summary>
    /// 같은 동글이끼리 충돌했을 때 호출
    /// 한 단계 위의 동글이로 교체
    /// </summary>
    /// <param name="_dongle1">이 함수를 호출한 동글이</param>
    /// <param name="_dongle2">호출한 동글이와 충돌한 동글이</param>
    public void SumDongle(GameObject _dongle1, GameObject _dongle2)
    {
        if (sumDongles[0] != null)
        {
            for(int i = 0; i < 2; i++)
            {
                if (sumDongles[i] == _dongle1)
                {
                    return;
                }
            }
        }

        sumDongles[0] = _dongle1;
        sumDongles[1] = _dongle2;

        int idx = _dongle1.GetComponent<Dongle>().DongleIndex;

        if (idx == listDongleObj.Count - 1) return;

        for(int i = 0; i < 2; i++)
        {
            Destroy(sumDongles[i]);
        }

        GameObject obj = Instantiate(listDongleObj[idx + 1], _dongle1.transform.position, Quaternion.identity);
        obj.GetComponent<Dongle>().Drop();
    }
}
