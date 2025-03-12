using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Rigidbody2D rb;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI keyText;
    public GameObject door;
    public Collider2D doorCollider;
    public SpriteRenderer doorSpriteRenderer;
    public Sprite openDoorSprite;
    private int keyCount = 0;
    private float timeLeft = 90f;
    private bool isGrounded;

    void Update()
    {
        Move();
        Jump();
        UpdateTimer();
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            isGrounded = false; 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) != null;
    }

    void UpdateTimer()
    {
        timeLeft -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.Ceil(timeLeft);
        
        if (timeLeft <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            keyCount++;
            keyText.text = "Keys: " + keyCount;
            Destroy(other.gameObject);
            doorCollider.isTrigger = keyCount > 0; // Enable trigger only if key count > 0
        }
        else if (other.CompareTag("Door") && keyCount > 0)
        {
            if (doorSpriteRenderer != null && openDoorSprite != null)
            {
                doorSpriteRenderer.sprite = openDoorSprite; // Change door sprite
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Door") && keyCount > 0)
        {
            keyCount--;
            keyText.text = "Keys: " + keyCount;
            doorCollider.isTrigger = keyCount > 0; // Keep trigger enabled only if keys remain
        }
    }
}
