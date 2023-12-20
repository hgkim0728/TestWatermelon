using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Player.hitType hitType;
    //[SerializeField] private LayerMask layer;
    Player player;

    void Start()
    {
        player = transform.GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //int value = (int)Mathf.Log(layer, 2);
        //if (collision.gameObject.layer == value)
        //{
        player.TriggerEnter(hitType, collision);
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player.TriggerExit(hitType, collision);
    }
}
