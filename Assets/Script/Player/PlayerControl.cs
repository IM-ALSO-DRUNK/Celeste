using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class PlayerControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D Rb;
    public float XSpeed;
    public float YSpeed;
    public float JumpSpeed;
    public float JumpAtWallImpulse;
    public float JumpGNum;
    public float FallGNum;
    public float RayCastDistance;
    public float CircleRadium;
    public float FallAtWallSpeed;
    public float WalkJumpLerpNum;
    public float  Dashmagnification;
    public float WaitDashNum;
    [Tooltip("残影的透明度")]
    [Range(0f, 1f)] public float alpha = 0.5f;
    [Tooltip("残影的生成间隔")]
    public float CreateFadeInterval;
    [Tooltip("残影的存活时间")]
    public float FadeLifeTime;
    public LayerMask RayCastLayerMask;
    public LayerMask CircleLayerMask;

    public GameObject Foot;
    public GameObject Right;
    public GameObject Left;

    //判断是左墙还是右墙
    public bool HaveRightWall;
    public bool HaveLeftWall;
    public bool AtGound;
    public bool AtWall;
    public bool Grab;
    public bool WallJump;
    public bool CanDash;
    public bool CanMove;


    public  ParticleSystem snowParticles;
    private ParticleSystem.VelocityOverLifetimeModule velocityMode;
    public ParticleSystem.MainModule mainModule;

    public SpriteRenderer sprite;
    public Animator animator;

    public GameObject RipplePrefab;
    public GameObject CopyRipple;


    private bool HaveRipple;
    
    private void Awake()
    {
        sprite=GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        snowParticles=gameObject.GetComponent<ParticleSystem>();
        mainModule = snowParticles.main;
        velocityMode = snowParticles.velocityOverLifetime;
        Foot =gameObject.transform.Find("Foot").gameObject;
        Right = gameObject.transform.Find("Right").gameObject;

        Left = gameObject.transform.Find("Left").gameObject;

        Grab = false;
        AtGound = false;
        WallJump = false;
        CanDash = true;
        CanMove = true;
        HaveRipple = false;


    }
    void Start()
    {
    }
    

    private void FixedUpdate()
    {


        GrabOnWall();
        if(HaveRipple&&CopyRipple==null)
        {
            CopyRipple = GameObject.Instantiate(RipplePrefab, gameObject.transform);
            CopyRipple.transform.localPosition = Vector3.zero;
        }
        else
        {
            if(CopyRipple!=null)
            {
                Destroy(CopyRipple);

            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        animator.SetFloat("YSpeed", Rb.velocity.y);
        animator.SetFloat("Y",y);


        IsGound();
        OnWall();
        //跳跃
        Jump();
        
        Walk(x, y);

        Dash(x,y);



        //在墙壁的下滑
        if (AtWall && !AtGound&&!Grab&&Rb.velocity.y<=0)
        {
            animator.SetBool("wallSlide", true);
            Rb.velocity = new Vector2(Rb.velocity.x, FallAtWallSpeed);
            animator.SetBool("OnRightWall", HaveRightWall);
            if (animator.GetBool("OnRightWall"))
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;

            }

        }
        else
        {
            animator.SetBool("wallSlide", false);

        }
       

        
        
        
        
    }
    public void GrabOnWall()
    {
        //爬在墙上
        if (Input.GetKey(KeyCode.J) && AtWall)
        {
            
            Grab = true;
            CanMove = false;
            animator.SetBool("canMove", false);
            Rb.velocity = new Vector2(Rb.velocity.x, 0);
            Rb.AddForce(-Physics2D.gravity, ForceMode2D.Force);
            animator.SetBool("Grab", true);
            animator.SetBool("OnRightWall", HaveRightWall);
            if (animator.GetBool("OnRightWall"))
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;

            }
        }
        else
        {
            animator.SetBool("Grab", false);
            CanMove = true;
            animator.SetBool("canMove", true);
            Grab = false;
        }

        //爬墙向上
        if (Grab && Input.GetKey(KeyCode.W))
        {
            Rb.velocity = new Vector2(Rb.velocity.x, YSpeed);
        }

    }
    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && AtGound)
        {
            animator.SetTrigger("jump");
            Rb.velocity += new Vector2(Rb.velocity.x, JumpSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && AtWall)//蹬墙跳
        {
            Rb.velocity = new Vector2(Rb.velocity.x, 0);
            Rb.velocity = new Vector2(Rb.velocity.x, JumpSpeed);
            if(HaveLeftWall)
            {
                Rb.AddForce(new Vector2(JumpAtWallImpulse, 0), ForceMode2D.Impulse);
            }
            if(HaveRightWall)
            {
                Rb.AddForce(new Vector2(-JumpAtWallImpulse, 0), ForceMode2D.Impulse);

            }
            WallJump = true;
            animator.SetTrigger("jump");



        }

        //通过不同的重力系数，来实现按住不同时间的空格，跳跃高度不同。
        if (Rb.velocity.y < 0)
        {
            Rb.velocity += Physics2D.gravity * (FallGNum - 1) * Time.deltaTime;
        }
        else if (Rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            Rb.velocity += Physics2D.gravity * (JumpGNum - 1) * Time.deltaTime;
        }
    }
    public void Walk(float x,float y)
    {
        

            if(!CanMove) return;

            if (!WallJump)
            {
                Rb.velocity = new Vector2(XSpeed * x, Rb.velocity.y);
                animator.SetFloat("X",x);

            }
            else
            {

                Rb.velocity = Vector2.Lerp(Rb.velocity, new Vector2(XSpeed * x, Rb.velocity.y), WalkJumpLerpNum * Time.deltaTime);
            }
            if (x > 0)
            {
                sprite.flipX = false;
            }
            if (x < 0)
            {
                sprite.flipX = true;
            }



    }
    public void Dash(float x,float y)
    {

        if (Input.GetKeyDown(KeyCode.L)&&CanDash)
        {

            HaveRipple=true;
            Camera.main.DOShakePosition(0.5f, 0.25f, 10, 90, true); 
            animator.SetTrigger("dash");
            CanDash = false;
            WallJump = true;
            Rb.velocity = Vector2.zero;
            Rb.velocity += new Vector2(x, y).normalized * Dashmagnification;

            float SnowForceX = -new Vector2(x, y).normalized.x * 5;


            velocityMode.x = new ParticleSystem.MinMaxCurve(SnowForceX - 2, SnowForceX + 2);
            snowParticles.Play();
            StartCoroutine(CreateFade());
            StartCoroutine(DashWait());
        }
        
     

    }
    IEnumerator CreateFade()
    {
        for(int i=1;i<=3;i++)
        {
            GreateAfterImage();
            yield return new WaitForSeconds(CreateFadeInterval);
        }
       
    }
    public void GreateAfterImage()
    {
        // 创建空物体作为残影
        GameObject afterimage = new GameObject("Afterimage");
        afterimage.transform.position = transform.position;
        afterimage.transform.rotation = transform.rotation;
        afterimage.transform.localScale =transform.localScale;

        // 添加SpriteRenderer组件
        SpriteRenderer sr = afterimage.AddComponent<SpriteRenderer>();
        sr.sprite = sprite.sprite;
        sr.flipX = sprite.flipX;
        sr.flipY = sprite.flipY;
        sr.sortingLayerID = sprite.sortingLayerID;
        sr.sortingOrder = sprite.sortingOrder - 1; // 确保残影在角色后面

        // 设置颜色（保持原色调，只修改透明度）
        Color color = sprite.color;
        color.a = alpha;
        sr.color = color;

        // 添加残影消失的动画效果
        AfterImage image = afterimage.AddComponent<AfterImage>();
        image.Initialize(FadeLifeTime);
    }
    IEnumerator DashWait( )
    {
        Rb.gravityScale = 0;
        Rb.drag = 6;
        yield return new  WaitForSeconds(WaitDashNum);
        HaveRipple = false;
        Rb.gravityScale = 1;
        Rb.drag = 0;


    }
    public void IsGound()
    {
        Collider2D Hit = Physics2D.OverlapCircle(Foot.transform.position, CircleRadium, RayCastLayerMask);
        AtGound = Hit != null;
        if (AtGound)
        {
            WallJump = false;
            CanDash = true;
            animator.SetBool("onGround", true);
        }
        else
        {
            animator.SetBool("onGround", false);

        }
    }
    public void OnWall()
    {

        Collider2D colliderL = Physics2D.OverlapCircle(Left.transform.position, CircleRadium, CircleLayerMask);
        Collider2D colliderR = Physics2D.OverlapCircle(Right.transform.position, CircleRadium, CircleLayerMask);
       
        
        if(colliderL != null)
        {
            AtWall = true;
            HaveLeftWall = true;
            WallJump = true;
            CanDash=true;
            animator.SetBool("onWall", true);
            return;
        }
        
        if(colliderR != null)
        {
            AtWall = true;
            HaveRightWall = true;
            WallJump = true;
            CanDash = true;
            animator.SetBool("onWall", true);

            return;
        }
        HaveLeftWall = false;
        HaveRightWall=false;
        AtWall = false;

        animator.SetBool("onWall", false);



    }
    public void OnDrawGizmos()
    {
        if(Foot==null) return;
        Gizmos.color =AtGound? Color.green : Color.red;
        Gizmos.DrawWireSphere(Foot.transform.position,CircleRadium);
        Gizmos.DrawWireSphere(Right.transform.position, CircleRadium);
        Gizmos.DrawWireSphere(Left.transform.position, CircleRadium);


    }
    
}
