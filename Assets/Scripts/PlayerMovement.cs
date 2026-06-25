using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Podstawowy ruch")]
    public float moveSpeed = 6f;
    public float rotateSpeed = 140f;
    public int playerNumber = 1;

    [Header("Dash")]
    public float dashForce = 18f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 3.5f;

    [Header("Fizyka")]
    public float linearDrag = 4.5f;

    [Header("Trail")]
    public TrailRenderer trailRenderer;

    private Rigidbody2D rb;
    private float moveInput;
    private float rotateInput;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (playerNumber == 1)
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        else
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);

        // Wczytaj opcje
        if (GameOptions.Instance != null)
        {
            moveSpeed = GameOptions.Instance.tankSpeed;
            rotateSpeed = GameOptions.Instance.tankRotateSpeed;
        }
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        moveInput = 0f;
        rotateInput = 0f;

        if (playerNumber == 1)
        {
            if (keyboard.wKey.isPressed) moveInput = 1f;
            if (keyboard.sKey.isPressed) moveInput = -1f;
            if (keyboard.aKey.isPressed) rotateInput = 1f;
            if (keyboard.dKey.isPressed) rotateInput = -1f;

            if (keyboard.leftShiftKey.wasPressedThisFrame && CanDash())
                StartDash();
        }
        else if (playerNumber == 2)
        {
            if (keyboard.upArrowKey.isPressed) moveInput = 1f;
            if (keyboard.downArrowKey.isPressed) moveInput = -1f;
            if (keyboard.leftArrowKey.isPressed) rotateInput = 1f;
            if (keyboard.rightArrowKey.isPressed) rotateInput = -1f;

            if (keyboard.rightCtrlKey.wasPressedThisFrame && CanDash())
                StartDash();
        }

        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) isDashing = false;
        }

        UpdateTrail();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        if (rotateInput != 0)
            transform.Rotate(0f, 0f, rotateInput * rotateSpeed * Time.fixedDeltaTime);

        Vector2 forward = -transform.up;
        rb.AddForce(forward * moveInput * moveSpeed * 55f, ForceMode2D.Force);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        Vector2 dashDir = -transform.up;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);

        StartCoroutine(DashVisualEffect());
    }

    private bool CanDash() => !isDashing && dashCooldownTimer <= 0f;

    private void UpdateTrail()
    {
        if (trailRenderer == null) return;
        bool shouldEmit = moveInput > 0.3f || isDashing;
        trailRenderer.emitting = shouldEmit;
    }

    private System.Collections.IEnumerator DashVisualEffect()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.7f, originalScale.z);
        yield return new WaitForSeconds(0.07f);
        transform.localScale = new Vector3(originalScale.x * 0.85f, originalScale.y * 1.25f, originalScale.z);
        yield return new WaitForSeconds(0.13f);
        transform.localScale = originalScale;
    }

    // Dla TankTreadMarks
    public Vector2 GetMoveInput() => new Vector2(rotateInput, moveInput);
    public bool IsDashing() => isDashing;
}