using UnityEngine;

/// <summary>
/// Handles player movement, jumping, and dashing functionalities.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float wallJumpHorizontalForce = 2.5f;
    public float wallJumpVerticalMultiplierForce = 1f;
    public float jumpHorizontalForceDuration = 0.35f;
    public float dashCooldown = 0.6f;
    public float verticalDashDuration = 0.3f;
    public float grabbingFallSpeed = -1f;
    public float maxFallSpeed = 15f;

    public float dashPower = 100f;

    public int nSecondJumps = 1;

    private Rigidbody2D _rigidbody2D;
    private bool _isGrounded;
    private bool _isTouchingWallLeft;
    private bool _isTouchingWallRight;
    private bool _isTouchingWall;
    private bool _isGrabbingWallLeft;
    private bool _isGrabbingWallRight;
    private bool _isGrabbingWall;
    private bool _wallJumping;
    private bool _canSlide;
    private bool _isFalling;
    private bool _inTheAir;
    private float _lookingHDirection;
    private float _lookingVDirection;
    
    private bool _movementEnabled = true;

    public Transform groundCheck;
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float checkRadius = 0.1f;
    public LayerMask whatIsGround;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private float _maxVelocity = 100f;

    private void FixedUpdate()
    {
        HandleMovement();
        UpdateActions();

        HandleWallGrabbing();

        PerformAction();

        ClampPlayerVelocity();
    }

    private void PerformAction()
    {
        if (!_movementEnabled) return;
        if (PerformedDash()) return;
        if (PerformedJump()) return;
    }
    
    private bool PerformedJump()
    {
        if (CanJump())
        {
            PerformJump();
            return true;
        }
        else if (CanDoubleJump())
        {
            PerformConsecutiveJump();
            return true;
        }
        return false;
    }

    private bool PerformedDash()
    {
        
        if (CanDashVertical())
        {
            PerformDashVertical();
            return true;
        }

        if (CanDashHorizontal())
        {
            PerformDashHorizontal();
            return true;
        }
        
        return false;
    }

    private int _consecutiveJumpsMade;

    private bool CanDoubleJump()
    {
        bool wPressed = Input.GetKey(KeyCode.W);
        if (wPressed && !_jumpedWithW && !_wallJumping && _inTheAir && _consecutiveJumpsMade < nSecondJumps)
        {
            return true;
        }

        if (!wPressed)
        {
            _jumpedWithW = false;
        }

        return false;
    }

    private void ClampPlayerVelocity()
    {
        Vector2 velocity = _rigidbody2D.velocity;
        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;
        if (velocity.x > _maxVelocity) velocity.x = _maxVelocity;
        if (velocity.x < -_maxVelocity) velocity.x = -_maxVelocity;
        _rigidbody2D.velocity = velocity;
    }

    private bool _dashed;
    private bool _horizontalDashCooldownActive;
    private bool _verticalDashCooldownActive;

    private float _betweenDashDelay = 0.4f;

    private bool CanDashVertical()
    {
        return Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.B) && !_dashed && !_verticalDashCooldownActive && !_wallJumping;
    }

    private bool CanDashHorizontal()
    {
        return (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && Input.GetKey(KeyCode.B) && !_dashed && !_horizontalDashCooldownActive && !_wallJumping;
    }

    private void PerformDashVertical()
    {
        float vDirection = Input.GetKey(KeyCode.S) ? -0.125f : 0;

        // Reset velocities
        ResetVelocities();

        Vector2 dashDirection = new Vector2(0, vDirection);
        dashDirection.y *= dashPower;
        _rigidbody2D.AddForce(dashDirection, ForceMode2D.Impulse);

        _dashed = true;
        _verticalDashCooldownActive = true;
        ResetCooldownsAfterDelayV();
    }

    private void ResetCooldownsAfterDelayV()
    {
        Invoke(nameof(ResetVerticalDashCooldown), dashCooldown);
        Invoke(nameof(ResetVerticalVelocity), verticalDashDuration);
        Invoke(nameof(ResetBetweenDashCooldown), _betweenDashDelay);
    }

    private void PerformDashHorizontal()
    {
        float hDirection = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;

        // Reset velocities
        ResetVelocities();

        Vector2 dashDirection = new Vector2(hDirection, 0);
        dashDirection.x *= dashPower;
        _rigidbody2D.AddForce(dashDirection, ForceMode2D.Impulse);

        _dashed = true;
        _horizontalDashCooldownActive = true;
        ResetCooldownsAfterDelay();
    }

    private void ResetCooldownsAfterDelay()
    {
        Invoke(nameof(ResetHorizontalDashCooldown), dashCooldown);
        Invoke(nameof(ResetBetweenDashCooldown), _betweenDashDelay);
    }

    private void ResetVerticalDashCooldown()
    {
        _verticalDashCooldownActive = false;
    }

    private void ResetHorizontalDashCooldown()
    {
        _horizontalDashCooldownActive = false;
    }
    
    private void ResetBetweenDashCooldown()
    {
        _dashed = false;
    }
    
    //Pushes the player in a direction, the direction value should range from -1 to 1
    private void GetPushed(Vector2 direction, float pushPower=10f)
    {
        ResetVelocities();
        _rigidbody2D.AddForce(direction * pushPower, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        ApplyMovementDisplacementWhenDamaged(other);
    }

    private void ApplyMovementDisplacementWhenDamaged(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _movementEnabled = false;
            Vector2 direction = transform.position - other.transform.position;
            direction.Normalize();
            GetPushed(direction);
            Invoke(nameof(ResetMovementDisabled), 0.4f);
        }
    }

    private void HandleMovement()
    {
        UpdateHorizontalLookDirection();
        UpdateVerticalLookDirection();

        HandeHorizontalMovement();
    }

    private void HandeHorizontalMovement()
    {
        // Only apply regular movement if not in a wall jump state
        if (!_wallJumping && _movementEnabled)
        {
            _rigidbody2D.velocity = new Vector2(_lookingHDirection * moveSpeed, _rigidbody2D.velocity.y);
        }
    }

    private void UpdateHorizontalLookDirection()
    {
        _lookingHDirection = Input.GetAxis("Horizontal");
    }

    private void UpdateVerticalLookDirection()
    {
        _lookingVDirection = Input.GetAxis("Vertical");
    }


    private Vector2 _boxSize = new Vector2(0.1f, 1f); // Box width is 0.1, height is half of the player's height

    private bool IsTouchingGround()
    {
        return Physics2D.OverlapBox(groundCheck.position, _boxSize, 0f, whatIsGround);
    }

    private bool IsTouchingWallLeft()
    {
        Vector2 boxPosition = wallCheckLeft.position;
        return Physics2D.OverlapBox(boxPosition, _boxSize, 0f, whatIsGround);
    }

    private bool IsTouchingWallRight()
    {
        Vector2 boxPosition = wallCheckRight.position;
        return Physics2D.OverlapBox(boxPosition, _boxSize, 0f, whatIsGround);
    }

    private bool _jumpedWithW;

    
    private bool CanJump()
    {
        bool wPressed = Input.GetKey(KeyCode.W);
        if (wPressed && !_jumpedWithW && !_wallJumping && (_isGrounded || _isTouchingWallLeft || _isTouchingWallRight))
        {
            return true;
        }

        if (!wPressed)
        {
            _jumpedWithW = false;
        }
        return false;
    }

    private void PerformJump()
    {
        ResetVelocities();
        PerformJumpWithDirection();
        _jumpedWithW = true;
    }

    private void PerformConsecutiveJump()
    {
        RegularJump();
        _jumpedWithW = true;
        _consecutiveJumpsMade++;
    }

    private void HandleWallGrabbing()
    {
        if (!_isGrounded && !_wallJumping && _canSlide)
        {
            // log in console
            Vector2 velocity = _rigidbody2D.velocity;
            _rigidbody2D.velocity = new Vector2(velocity.x, grabbingFallSpeed);
        }
    }

    private void UpdateActions()
    {
        _isGrounded = IsTouchingGround();
        _isTouchingWallLeft = IsTouchingWallLeft();
        _isTouchingWallRight = IsTouchingWallRight();
        _isTouchingWall = _isTouchingWallLeft || _isTouchingWallRight;
        _isGrabbingWallLeft = _isTouchingWallLeft && Input.GetKey(KeyCode.A);
        _isGrabbingWallRight = _isTouchingWallRight && Input.GetKey(KeyCode.D);
        _isGrabbingWall = _isGrabbingWallLeft || _isGrabbingWallRight;

        // Check if the player is moving downward
        var velocity = _rigidbody2D.velocity;
        _canSlide = velocity.y < 0 && _isGrabbingWall;
        _isFalling = velocity.y < 0 && !_isGrabbingWall;

        _inTheAir = !_isGrounded && !_isGrabbingWall;

        if (_isGrounded || _isGrabbingWall) _consecutiveJumpsMade = 0;
    }

    private void PerformJumpWithDirection()
    {
        if (_isGrounded)
        {
            RegularJump();
        }
        else if (_isTouchingWallRight)
        {
            WallJump(-wallJumpHorizontalForce);
        }
        else if (_isTouchingWallLeft)
        {
            WallJump(wallJumpHorizontalForce);
        }
    }

    private void ResetVelocities()
    {
        _rigidbody2D.velocity = new Vector2(0f, 0f);
    }

    private void ResetVerticalVelocity()
    {
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
    }

    private void WallJump(float horizontalForce)
    {
        _rigidbody2D.AddForce(new Vector2(horizontalForce, jumpForce * wallJumpVerticalMultiplierForce),
            ForceMode2D.Impulse);
        _wallJumping = true;
        Invoke(nameof(ResetWallJump), jumpHorizontalForceDuration);
    }

    private void RegularJump()
    {
        ResetVerticalVelocity();
        _rigidbody2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    private void ResetWallJump()
    {
        _wallJumping = false;
    }
    
    private void ResetMovementDisabled()
    {
        _movementEnabled = true;
    }
}