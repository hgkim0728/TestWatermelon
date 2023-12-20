using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum hitType
    {
        WallCheck,
        HitCheck
    }

    Rigidbody2D rigid;
    Vector3 moveDir;
    CapsuleCollider2D playerCol;
    private Animator anim;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] bool isGround = false;  // false 공중에 떠있는 상태, true 땅에 붙어있는 상태

    private bool isJump = false;
    private float verticalVelocity; // 수직으로 받는 힘
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpForce = 5f;
    private Camera mainCam;

    [Header("투척무기")]
    Transform trsHand;
    bool isPlayerLookAtRightDirection;
    [SerializeField] GameObject objSword;
    Transform trsSword;
    [SerializeField] Transform trsObjDynamic;
    [SerializeField] float throwLimit = 0.3f;
    float throwTimer = 0.3f;

    bool gamePause = false;

    [Header("대시")]
    private bool dash = false;
    private float dashTimer = 0.0f;
    private TrailRenderer dashEffect;
    [SerializeField] private float dashLimit = 0.2f;

    [Header("벽점프")]
    private bool wallStep = false;
    private bool doWallStep = false;
    private bool doWallStepTimer = false;
    private float wallStepTimer = 0;
    [SerializeField] private float wallStepTime = 0.3f;

    private void OnDrawGizmos()
    {
        if (playerCol != null)
        {
            Gizmos.color = Color.red;
            Vector3 pos = playerCol.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, playerCol.bounds.size);
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        playerCol = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        trsHand = transform.Find("Hand");
        trsSword = trsHand.GetChild(0);
        dashEffect = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if(gamePause)
        {
            return;
        }

        CheckMouse();

        CheckGround();
        Moving();
        //Turning();

        Jumping();
        CheckGravity();

        CheckDash();

        DoAnimation();

        CheckDoStepWallTimer();
    }

    private void CheckMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCam.transform.position.z;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mousePos);

        // 플레이어 좌우 확인
        Vector3 distanceMouseToPlayer = mouseWorldPos - transform.position;

        if(distanceMouseToPlayer.x > 0 && transform.localScale.x != -1)
        {
            transform.localScale = new Vector3(-1f, 1, 1);
            isPlayerLookAtRightDirection = true;
        }
        else if(distanceMouseToPlayer.x < 0 && transform.localScale.x != 1)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isPlayerLookAtRightDirection = false;
        }

        // 마우스 에임
        //Vector3 direction = isPlayerLookAtRightDirection == true ? Vector3.right : Vector3.left;

        Vector3 direction = Vector3.right;
        if (isPlayerLookAtRightDirection == false)
        {
            direction = Vector3.left;
        }

        float angle = Quaternion.FromToRotation(direction, distanceMouseToPlayer).eulerAngles.z;
        
        if(isPlayerLookAtRightDirection == true)
        {
            angle = -angle;
        }

        //trsHand.rotation = Quaternion.Euler(0, 0, angle);
        trsHand.localEulerAngles = new Vector3(trsHand.localEulerAngles.x, trsHand.localEulerAngles.y, angle);

        if(throwTimer != 0.0f)
        {
            throwTimer -= Time.deltaTime;

            if(throwTimer < 0.0f)
            {
                throwTimer = 0.0f;
            }
        }

        if(Input.GetKey(KeyCode.Mouse0) && throwTimer == 0.0f)
        {
            Shoot();
            throwTimer = throwLimit;
        }
    }

    private void CheckGround()
    {
        isGround = false;

        if(verticalVelocity > 0)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.BoxCast(playerCol.bounds.center, playerCol.bounds.size, 0f,
            Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        
        //RaycastHit2D hit = Physics2D.Raycast(playerCol.bounds.center, Vector3.down, distance, LayerMask.GetMask("Ground"));

        if(hit.transform != null)
        {
            isGround = true;
        }
    }

    private void Moving()
    {
        if(dash || doWallStepTimer)
        {
            return;
        }

        moveDir.x = Input.GetAxisRaw("Horizontal") * moveSpeed;
        moveDir.y = rigid.velocity.y;
        rigid.velocity = moveDir;
    }

    private void Turning()
    {
        if(moveDir.x > 0 && transform.localScale.x > -1)// 오른쪽으로 이동중
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if(moveDir.x < 0 && transform.localScale.x < 1)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void Jumping()
    {
        if(!isGround && Input.GetKeyDown(KeyCode.Space) 
            && wallStep && (moveDir.x < 0 || moveDir.x > 0))
        {
            doWallStep = true;
        }
        else if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isJump = true;
        }
    }

    private void CheckGravity()
    {
        if(dash)
        {
            return;
        }

        if(doWallStep)// 벽점프를 해야할 때
        {
            Vector2 dir = rigid.velocity;
            dir.x *= -1;
            rigid.velocity = dir;   // 튀는 방향    // 입력하고 있던 방향의 역방향으로 점프하게 되고
            verticalVelocity = jumpForce;    // 힘은 점프 포스의 1/2 의 힘으로 점프
            doWallStep = false;
            doWallStepTimer = true;
        }

        if (!isGround)
        {
            verticalVelocity -= gravity * Time.deltaTime;

            if(verticalVelocity < -10.0f)   // 떨어지는 속도가 -10보다 작아지면 -10으로 제한
            {
                verticalVelocity = -10f;
            }
        }
        else // 땅에 붙어있을 때
        {
            verticalVelocity = 0;
        }

        if(isJump)
        {
            isJump = false;
            verticalVelocity = jumpForce;
        }

        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }

    private void CheckDash()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && !dash)
        {
            dash = true;

            verticalVelocity = 0f;
            rigid.velocity = new Vector2(isPlayerLookAtRightDirection == true ? 20 : -20, 0);

            dashEffect.enabled = true;
        }
        else if(dash)
        {
            dashTimer += Time.deltaTime;

            if(dashTimer > dashLimit)
            {
                dashTimer = 0f;
                dashEffect.enabled = false;
                dashEffect.Clear();
                dash = false;
            }
        }
    }

    private void DoAnimation()
    {
        anim.SetBool("IsGround", isGround);
        anim.SetInteger("Horizontal", (int)moveDir.x);
    }

    private void CheckDoStepWallTimer()
    {
        if(doWallStepTimer)
        {
            wallStepTimer += Time.deltaTime;

            if(wallStepTimer >= wallStepTime)
            {
                wallStepTimer = 0f;
                doWallStepTimer = false;
            }
        }
    }

    private void Shoot()
    {
        GameObject obj = Instantiate(objSword, trsSword.position, trsSword.rotation, trsObjDynamic);
        Sword objSc = obj.GetComponent<Sword>();
        //Vector2 throwForce = isPlayerLookAtRightDirection == true ? new Vector2(10.0f, 0f) : new Vector2(-10.0f, 0f);
        Vector2 throwForce = new Vector2(10.0f, 0f);

        if(!isPlayerLookAtRightDirection)
        {
            //throwForce = new Vector2(-10.0f, 0f);
            throwForce.x = -10.0f;
        }

        objSc.SetForce(trsSword.rotation * throwForce, isPlayerLookAtRightDirection);
    }

    public void TriggerEnter(hitType _type, Collider2D _collision)
    {
        if (_type == hitType.WallCheck && _collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            wallStep = true;
        }
        else if(_type == hitType.HitCheck && _collision.gameObject.tag == "Item")
        {
            Item sc = _collision.gameObject.GetComponent<Item>();
            sc.GetItem();
        }
    }

    public void TriggerExit(hitType _type, Collider2D _collision)
    {
        if(_type == hitType.WallCheck)
        {
            wallStep = false;
        }
    }
}
