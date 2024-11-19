using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats P1Stats;
    bool readyToJump;
    public AudioSource footStepsSounds;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    //Health&Inventory
    PlayerInventoryStats PI;    

    private void Awake()
    {
        PI = GetComponent<PlayerInventoryStats>();

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        //checkforground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        //playerdrag
        if(grounded)
            rb.drag = P1Stats.groundDrag;
        else    
            rb.drag = 0;

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if(grounded)
                footStepsSounds.enabled = true;

        }
        else
                footStepsSounds.enabled = false;

        if (Input.GetKey(KeyCode.Q))
        {
            UnlockMouse();
        }

        if (Input.GetKey(KeyCode.P))
        {
            PI.TakeDamage(2);
        }

        if (Input.GetKey(KeyCode.O))
        {
            RestoreHealth();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //jump:3
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            SoundManager.PlaySound(SoundType.Jump);

            Invoke(nameof(ResetJump), P1Stats.jumpCooldown);
        }
    }
    
    void RestoreHealth()
    {
        if(Inventory.inventory.consumableItemsController.GetItem("HealthPill").GetOwnedQuantity() != 0)
        {
            Inventory.inventory.consumableItemsController.UseItem("HealthPill");
        }
    }

    private void MovePlayer()
    {
        //movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded) //onground
            rb.AddForce(moveDirection.normalized * P1Stats.moveSpeed * 10f, ForceMode.Force);
        else if (!grounded) //inair
            rb.AddForce(moveDirection.normalized * P1Stats.moveSpeed * 10f * P1Stats.airMultiplier, ForceMode.Force);
            
    }

    private void SpeedControl() 
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limitspeed
        if(flatVel.magnitude > P1Stats.moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * P1Stats.moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //makesureverticalvelociyis0
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * P1Stats.jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
