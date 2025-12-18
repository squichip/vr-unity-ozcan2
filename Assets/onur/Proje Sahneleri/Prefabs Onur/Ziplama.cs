using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class ContinuousMoveWithJump : MonoBehaviour
{
    [Header("Move")]
    public Transform directionSource;      // Main Camera
    public float moveSpeed = 2.0f;

#if ENABLE_INPUT_SYSTEM
    public InputActionProperty moveAction; // Eski continuous move’daki action’ý buraya ver
#endif

    [Header("Jump")]
    public float ziplamaYuksekligi = 0.25f; // hop gibi: 0.18–0.35
    public float yercekimi = -12f;          // -10…-18

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
    }

    void OnDisable()
    {
        if (moveAction.action != null) moveAction.action.Disable();
    }
#endif

    void Update()
    {
        // ---- Yatay hareket (tek yerden) ----
        Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        if (moveAction.action != null)
            input = moveAction.action.ReadValue<Vector2>();
#endif

        // Eðer action boþsa PC’de denemek için klasik axis (varsa)
        if (input == Vector2.zero)
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 forward = directionSource ? directionSource.forward : transform.forward;
        Vector3 right = directionSource ? directionSource.right : transform.right;

        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 moveXZ = (right * input.x + forward * input.y);
        if (moveXZ.sqrMagnitude > 1f) moveXZ.Normalize();
        moveXZ *= moveSpeed;

        // ---- Zýplama / yerçekimi ----
        bool grounded = cc.isGrounded;
        if (grounded && yHizi < 0f) yHizi = -2f;

        // PC: Space | VR: A (JoystickButton0)
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);

#if ENABLE_INPUT_SYSTEM
        // Input System ile Space de otomatik yakalanýr ama garanti olsun diye:
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;
#endif

        if (jumpPressed && grounded)
            yHizi = Mathf.Sqrt(ziplamaYuksekligi * -2f * yercekimi);

        yHizi += yercekimi * Time.deltaTime;

        // ---- TEK Move çaðrýsý (çakýþma yok) ----
        Vector3 finalVelocity = new Vector3(moveXZ.x, yHizi, moveXZ.z);
        cc.Move(finalVelocity * Time.deltaTime);
    }
}
