using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{
    [Header("Settings Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float airMoveSpeed = 10f;
    [SerializeField] float stealthSpeed;
    Vector2 axis;
    Vector2 movementPlayer;
    bool facingRight = true;
    bool isMoving;
    bool isStealthMode;
    bool canMove = true;
    [SerializeField] bool modeGod;

    [Header("Settings Jumping")]
    [SerializeField] float jumpForce = 16f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] Vector2 groundCheckSize;
    [SerializeField] float wallCheckDistance;
    bool grounded;

    [Header("Settings WallSliding")]
    [SerializeField] float wallSlideSpeed;
    [SerializeField] Transform wallCheckPoint;
    [SerializeField] LayerMask wallLayer;
    bool isTouchingWall;
    bool isWallSliding;

    [Header("Settings WallJumping")]
    [SerializeField] float walljumpforce;
    [SerializeField] Vector2 walljumpAngle;
    float walljumpDirection = -1;

    //Other components
    Rigidbody2D rb2d;
    WaveSpawner waveSpawner;
    HealthPlayer healthPlayer;

    //Animator variables
    Animator anim;
    int IDSpeedParam;
    int IDIsGroundesParam;
    int IDJumpParam;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        waveSpawner = GetComponent<WaveSpawner>();
        anim = GetComponent<Animator>();
        healthPlayer = GetComponent<HealthPlayer>();
    }
    private void Start()
    {
        walljumpAngle.Normalize();
        IDSpeedParam = Animator.StringToHash("IsMoving");
        IDIsGroundesParam = Animator.StringToHash("IsGrounded");
        IDJumpParam = Animator.StringToHash("Jump");
    }
    private void Update()
    {
        CheckWorld();
        AnimationsManagers();
    }
    private void FixedUpdate()
    {
        Movement();
        WallSlide();
    }

    #region Movements
    void Movement()
    {
        //Comprobar si esta moviendo
        if (axis.x != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (modeGod)
        {
            movementPlayer = axis * moveSpeed;
        }else
        {
           
            movementPlayer = new Vector2(axis.x * moveSpeed, rb2d.velocity.y);
        }
        //Si se puede mover aplicas la velocidad al player
        if (canMove)
        {
            rb2d.velocity = movementPlayer;
        }
        anim.SetFloat("VelocityX", Mathf.Abs(rb2d.velocity.x));
        anim.SetFloat("VelocityY", rb2d.velocity.y);


        //Voltearse izquierda o derecha
        if (canMove)
        {
            if (axis.x < 0 && facingRight)
            {
                Flip();
            }
            else if (axis.x > 0 && !facingRight)
            {
                Flip();
            }
        }
    }
    public void SetModeGod()
    {
        modeGod =! modeGod;
        if (modeGod)
        {
            rb2d.isKinematic = true;
            gameObject.tag = "Untagged";
        }
        else
        {
            gameObject.tag = "Player";
            rb2d.isKinematic = false;
        }

    }
    void WallSlide()
    {
        if (isTouchingWall && !grounded && rb2d.velocity.y < 0 && axis.x != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
        //Si te estas deslizando por el muro,aplicas la fuerza de deslize
        if (isWallSliding)
        {
            if (rb2d.velocity.y < -wallSlideSpeed)
                rb2d.velocity = new Vector2(rb2d.velocity.x, -wallSlideSpeed);
        }
    }
    public void Jump()
    {
        if (canMove)
        {
            anim.SetTrigger(IDJumpParam);
            //Normal Jump
            if (grounded && !isWallSliding)
            {
                rb2d.velocity = Vector2.zero;
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            //WallJump
            if (isWallSliding)
            {
                canMove = false;
                rb2d.velocity = Vector2.zero;
                Vector2 moveTo = new Vector2(walljumpforce * walljumpAngle.x * walljumpDirection , walljumpforce * walljumpAngle.y);
                rb2d.AddForce(moveTo,ForceMode2D.Impulse);
                StartCoroutine(StopMovement());
            }
        }

    }
    #endregion
    void CheckWorld()
    {
        grounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);
        isTouchingWall = Physics2D.Raycast(wallCheckPoint.position, wallCheckPoint.up, wallCheckDistance, wallLayer);
        
    }
    void AnimationsManagers()
    {
        anim.SetBool(IDIsGroundesParam, grounded);
        anim.SetBool("IsSliding", isWallSliding);
    }
    void Flip()
    {
        walljumpDirection *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0); 
    }

    IEnumerator StopMovement()
    {
        canMove = false;

        //if (transform.localScale.x == 1)
        //{
        //    transform.localScale = new Vector2(-1, 1);
        //}
        //else
        //{
        //    transform.localScale = Vector2.one;
        //}
        //Flip();

        yield return new WaitForSeconds(.3f);

        //transform.localScale = Vector2.one;
        canMove = true;
    }
    IEnumerator WaveGround()
    {
        yield return new WaitForSeconds(0.01f);
        if (grounded)
        {
            waveSpawner.DoGroundWave();
        }
    }

    #region Setters
    public void SetAxis(Vector2 _axis)
    {
        axis = _axis;
    }
    public void SetStealth(bool _b)
    {
        isStealthMode = _b;
    }
    public void SetCanMove(bool _b)
    {
        canMove = _b;
    }
    #endregion

    #region Getters
    public bool IsMoving()
    {
        return isMoving;
    }
    public bool IsStealth()
    {
        return isStealthMode;
    }
    public bool IsWallSliding()
    {
        return isWallSliding;
    }
    public float CurrentVelocityX()
    {
        return Mathf.Abs(rb2d.velocity.x);
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(WaveGround());
    }
    private void OnDrawGizmosSelected()
    {
        //Line ground check
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);

        Gizmos.color = Color.green;

        Gizmos.DrawLine(wallCheckPoint.position, new Vector3(wallCheckPoint.position.x, wallCheckPoint.position.y + wallCheckDistance, 0));


    }
   
}
