using UnityEngine;

public class PlayerSlopeHandler : MonoBehaviour
{
    [Header("Slope Settings")]
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;

    private Vector2 slopeNormal;
    private bool isOnSlope;
    public bool IsOnSlope => isOnSlope;

    public void HandleSlopeMovement(float targetSpeed, bool isGrounded)
    {
        if (!isGrounded || !isOnSlope)
        {
            return;
        }

        Vector2 slopeDirection = new Vector2(slopeNormal.y, -slopeNormal.x).normalized;

        //Idle = don't slide
        if (Mathf.Abs(targetSpeed) < 0.01f)
        {
            return;
        }

        //Move along slope
        Vector2 slopeVelocity = slopeDirection * targetSpeed;

        rb.linearVelocity = slopeVelocity;
    }
}