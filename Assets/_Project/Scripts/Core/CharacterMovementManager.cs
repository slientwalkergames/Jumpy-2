using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovementManager : MonoBehaviour
{
    [Header("Character Rigidbody Assignments")]
    [SerializeField] private Rigidbody topCharacter;
    [SerializeField] private Rigidbody bottomCharacter;

    [Header("Character Animators (YENİ)")]
    [SerializeField] private Animator topAnimator;
    [SerializeField] private Animator bottomAnimator;

    [Header("Jump Configurations")]
    [Tooltip("Zıplama yüksekliği (Eski uzun zıplamayı hissetmek için 10 ile 15 arası)")]
    [SerializeField] private float jumpForce = 12f;
    [Tooltip("SADECE DÜŞERKEN uygulanacak ekstra yerçekimi (Süzülmeyi engeller)")]
    [SerializeField] private float fallMultiplier = 4f;

    [Header("Jump Directions")]
    [SerializeField] private Vector3 topJumpDirection = Vector3.down;
    [SerializeField] private Vector3 bottomJumpDirection = Vector3.up;

    [Header("Advanced Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayLength = 1.3f;

    void FixedUpdate()
    {
        // 1. Akıllı Yerçekimini Uygula
        ApplySmartGravity(topCharacter, topJumpDirection);
        ApplySmartGravity(bottomCharacter, bottomJumpDirection);

        // 2. Animasyon Beynine (Animator) Yere Değip Değmediğimizi Bildir (Koşmaya dönmek için)
        if (topAnimator != null)
            topAnimator.SetBool("IsGrounded", IsGrounded(topCharacter.transform, topJumpDirection));

        if (bottomAnimator != null)
            bottomAnimator.SetBool("IsGrounded", IsGrounded(bottomCharacter.transform, bottomJumpDirection));
    }

    private void ApplySmartGravity(Rigidbody rb, Vector3 jumpDir)
    {
        if (rb == null) return;

        float upwardSpeed = Vector3.Dot(rb.linearVelocity, jumpDir);
        Vector3 gravityDirection = -jumpDir;

        if (upwardSpeed > 0)
        {
            rb.AddForce(gravityDirection * 9.81f, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(gravityDirection * (9.81f * fallMultiplier), ForceMode.Acceleration);
        }
    }

    // --- INPUT FONKSİYONLARI ---

    public void OnJumpLeftMouse(InputAction.CallbackContext context)
    {
        if (context.control.device is Touchscreen) return;
        // Sol tık -> Üst karakteri zıplat ve üst animatörü tetikle
        if (context.started) JumpCharacter(topCharacter, topJumpDirection, topAnimator);
    }

    public void OnJumpRightMouse(InputAction.CallbackContext context)
    {
        if (context.control.device is Touchscreen) return;
        // Sağ tık -> Alt karakteri zıplat ve alt animatörü tetikle
        if (context.started) JumpCharacter(bottomCharacter, bottomJumpDirection, bottomAnimator);
    }

    public void OnMobileTouch(InputAction.CallbackContext context)
    {
        if (context.control.device is Mouse) return;
        if (context.started)
        {
            Vector2 touchPosition = Pointer.current.position.ReadValue();
            if (touchPosition.x < Screen.width / 2f)
                JumpCharacter(topCharacter, topJumpDirection, topAnimator);
            else
                JumpCharacter(bottomCharacter, bottomJumpDirection, bottomAnimator);
        }
    }

    // --- FİZİK VE ANİMASYON TETİKLEME ---

    private void JumpCharacter(Rigidbody characterRb, Vector3 jumpDir, Animator characterAnimator)
    {
        if (characterRb != null && IsGrounded(characterRb.transform, jumpDir))
        {
            // Zıplama fiziği
            characterRb.linearVelocity = Vector3.zero;
            characterRb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);

            // Zıplama anında Animator'e "Hemen Zıpla!" komutunu (Trigger) gönder
            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger("JumpTrigger");
            }
        }
    }

    private bool IsGrounded(Transform characterTransform, Vector3 jumpDir)
    {
        Vector3 rayDirection = -jumpDir;
        Vector3 startPoint = characterTransform.position + (jumpDir * 0.1f);

        Debug.DrawRay(startPoint, rayDirection * rayLength, Color.red);
        return Physics.Raycast(startPoint, rayDirection, rayLength, groundLayer);
    }
}