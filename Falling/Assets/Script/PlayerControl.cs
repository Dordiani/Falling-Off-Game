using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    AudioManager audioManager;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Move Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private LayerMask wallLayer;
    private bool mustPatrol;
    private bool mustTurn;

    [Header("Jump Variables")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float forwordPower;
    private float jumpBufferTime = 0.3f;
    private float jumpBufferCounter;
    private bool jumping;
    private bool doubleJump;
    private bool grounded;

    [Header("Day Night Variables")]
    [SerializeField] private GameObject dayPlayer;
    [SerializeField] private GameObject nightPlayer;
    [SerializeField] private GameObject dayBackRound;
    [SerializeField] private GameObject nightBackRound;
    [SerializeField] private GameObject dayTimer;
    [SerializeField] private GameObject NightTimer;
    [SerializeField] private GameObject dayJumpB;
    [SerializeField] private GameObject NightJumpB;
    [SerializeField] private GameObject dayCycleB;
    [SerializeField] private GameObject NightCycleB;
    [SerializeField] private float cycleTime;
    private bool dayCycle;
    public bool day;
    public bool night;

    [Header("Animator")]
    [SerializeField] private Animator dayAnimator;
    [SerializeField] private Animator nightAnimator;

    [Header("Dead")]
    [SerializeField] Shake shake;
    [SerializeField] float deaingTime;
    [SerializeField] private GameObject deathScroomDay;
    [SerializeField] private GameObject deathScroomNight;
    public bool dead = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        mustPatrol = true;
        day = false;
        night = true;
    }

    // Update is called once per frame
    private void Update()
    {
        IsGrounded();
        if (mustPatrol)
        {
            Patrol();
        }
        dayAnimator.SetBool("Grounded", grounded);
        nightAnimator.SetBool("Grounded", grounded);
    }

    private void FixedUpdate()
    {
        if (mustPatrol)
        {
            mustTurn = Physics2D.OverlapCircle(wallCheckPos.position, 0.1f, wallLayer);
        }
    }

    private void OnDayNight(InputValue value)
    {
        dayCycle = value.isPressed;
        audioManager.PlaySFX(audioManager.day_night);
        if (dayCycle && night == false)
        {
            dayPlayer.SetActive(true);
            dayBackRound.SetActive(true);
            dayTimer.SetActive(true);
            dayJumpB.SetActive(true);
            dayCycleB.SetActive(true);
            nightPlayer.SetActive(false);
            nightBackRound.SetActive(false);
            NightTimer.SetActive(false);
            NightJumpB.SetActive(false);
            NightCycleB.SetActive(false);

            StartCoroutine(DayTime());
        }

        if (dayCycle && day == false)
        {
            dayPlayer.SetActive(false);
            dayBackRound.SetActive(false);
            dayTimer.SetActive(false);
            dayJumpB.SetActive(false);
            dayCycleB.SetActive(false);
            nightPlayer.SetActive(true);
            nightBackRound.SetActive(true);
            NightTimer.SetActive(true);
            NightJumpB.SetActive(true);
            NightCycleB.SetActive(true);

            StartCoroutine(NightTime());
        }
    }

    private void IsGrounded()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private void OnJump(InputValue value)
    {
        jumping = value.isPressed;
        audioManager.PlaySFX(audioManager.jump);

        if (grounded && !jumping)
        {
            doubleJump = false;
        }

        jumping = value.isPressed;

        if (jumping)
        {
            dayAnimator.SetTrigger("Jump");
            nightAnimator.SetTrigger("Jump");
            if (grounded || doubleJump)
            {
                jumpBufferCounter = jumpBufferTime;
                audioManager.PlaySFX(audioManager.doublejump);

                doubleJump = !doubleJump;
                dayAnimator.SetTrigger("Doble Jump");
                nightAnimator.SetTrigger("Doble Jump");
            }

            //animator.SetBool("Jumping", true);
            //FindObjectOfType<AudioManager>().Play("Jump");
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }


        if (jumpBufferCounter > 0f || doubleJump)
        {
            rb.velocity += new Vector2(forwordPower, jumpPower);

            jumpBufferCounter = 0f;
        }

        if (!jumping && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

    }

    private void Patrol()
    {
        if (mustTurn)
        {
            Flip();
        }

        rb.velocity = new Vector2(moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
    }

    private void Flip()
    {
        mustPatrol = false;
        mustTurn = !mustTurn;
        Vector3 PlayerScale = transform.localScale;
        PlayerScale.x *= -1;
        transform.localScale = PlayerScale;
        moveSpeed *= -1;
        mustPatrol = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wallCheckPos.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Dead")
        {
            Time.timeScale = deaingTime;
            shake.start = true;
            dayAnimator.SetBool("Deaing", true);
            nightAnimator.SetBool("Deaing", true);
            moveSpeed = 0f;
            dayCycle = false;
            jumping = false;
            dead = true;
            StartCoroutine(DeaingTimer());
            audioManager.PlaySFX(audioManager.death);
        }
    }

    private IEnumerator DayTime()
    {
        yield return new WaitForSeconds(cycleTime);
        day = false;
        night = true;
    }

    private IEnumerator NightTime()
    {
        yield return new WaitForSeconds(cycleTime);
        day = true;
        night = false;
    }

    private IEnumerator DeaingTimer()
    {
        yield return new WaitForSeconds(deaingTime);
        if (night == false)
        {
            deathScroomDay.SetActive(true);
            dayAnimator.SetBool("Deaing", false);
            nightAnimator.SetBool("Deaing", false);
        }

        if (day == false)
        {
            deathScroomNight.SetActive(true);
            dayAnimator.SetBool("Deaing", false);
            nightAnimator.SetBool("Deaing", false);
        }
    }

}
