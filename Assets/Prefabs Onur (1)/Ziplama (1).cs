using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class ContinuousMoveWithJump : MonoBehaviour
{
    [Header("Move")]
    public Transform directionSource;       // Main Camera
    public float moveSpeed = 2.0f;

#if ENABLE_INPUT_SYSTEM
    public InputActionProperty moveAction;  // Vector2 (stick)
#endif

    [Header("Jump Physics")]
    public float ziplamaYuksekligi = 0.25f; // 0.18–0.35
    public float yercekimi = -12f;          // -10…-18

#if ENABLE_INPUT_SYSTEM
    [Header("Jump Input (XR)")]
    public InputActionProperty jumpAction;  // Button (RightHand primaryButton = A)
#endif

    [Header("Debug")]
    public bool debugLog = false;

    private CharacterController cc;
    private float yHizi;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (directionSource == null && Camera.main != null)
            directionSource = Camera.main.transform;
    }

#if ENABLE_INPUT_SYSTEM
    void OnEnable()
    {
        if (moveAction.action != null) moveAction.action.Enable();
        if (jumpAction.action != null) jumpAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction.action != null) moveAction.action.Disable();
        if (jumpAction.action != null) jumpAction.action.Disable();
    }
#endif

    void Update()
    {
        // -------- MOVE (XZ) --------
        Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        if (moveAction.action != null)
            input = moveAction.action.ReadValue<Vector2>();
#endif

        // PC fallback (editörde)
        if (input == Vector2.zero)
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 forward = directionSource ? directionSource.forward : transform.forward;
        Vector3 right = directionSource ? directionSource.right : transform.right;

        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 moveXZ = (right * input.x + forward * input.y);
        if (moveXZ.sqrMagnitude > 1f) moveXZ.Normalize();
        moveXZ *= moveSpeed;

        // -------- JUMP / GRAVITY --------
        bool grounded = cc.isGrounded;

      

        // Yere deðince düþüþ hýzýný resetle (grounded tekrar düzgün çalýþsýn)
        if (grounded && yHizi < 0f) yHizi = -2f;

        bool jumpPressed = false;

#if ENABLE_INPUT_SYSTEM
        // VR için doðru yöntem: InputAction
        if (jumpAction.action != null)
            jumpPressed = jumpAction.action.WasPressedThisFrame();

        // PC’de Space testi (Input System)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;
#else
        // Eski input varsa sadece PC testi
        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;
#endif

        if (jumpPressed)
            yHizi = Mathf.Sqrt(ziplamaYuksekligi * -2f * yercekimi);

        yHizi += yercekimi * Time.deltaTime;

        // -------- TEK Move çaðrýsý --------
        cc.Move(Vector3.up * yHizi * Time.deltaTime);


    }
}
