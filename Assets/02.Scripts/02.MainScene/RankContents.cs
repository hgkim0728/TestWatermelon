using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankContents : MonoBehaviour
{
    [SerializeField] private TMP_Text textRank;
    [SerializeField] private TMP_Text textScore;
    [SerializeField] private TMP_Text textName;

    public void SetRankContents(string _rank, string _score, string _name)
    {
        textRank.text = _rank + "등";
        textScore.text = _score + " 점";
        textName.text = _name;
    }
}
