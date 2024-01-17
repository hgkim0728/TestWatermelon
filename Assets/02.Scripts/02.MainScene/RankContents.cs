using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankContents : MonoBehaviour
{
    [SerializeField, Tooltip("순위")] private TMP_Text textRank;
    [SerializeField, Tooltip("점수")] private TMP_Text textScore;
    [SerializeField, Tooltip("이름")] private TMP_Text textName;

    /// <summary>
    /// 랭크 내의 플레이어의 정보를 전달받고 텍스트에 입력
    /// </summary>
    /// <param name="_rank">순위</param>
    /// <param name="_score">점수</param>
    /// <param name="_name">이름</param>
    public void SetRankContents(string _rank, string _score, string _name)
    {
        textRank.text = _rank + "등";    // 유저 순위 입력
        textScore.text = _score + " 점";     // 유저 점수 입력
        textName.text = _name;  // 유저 이름 입력
    }
}
