using System;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 10f;

    public GameObject playerCamera;
    public float sensetivity = 1;
    public Rigidbody rb;
    public LayerMask groundLayer;
    
    public ChatBehavior chat;
    public Canvas localPlayerCanvas;
    
    private bool lockMovement = false;
    private bool isGrounded = false;
    
    public override void OnStartClient()
    {
        if (!isLocalPlayer)
        {
            enabled = false;
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
            localPlayerCanvas.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    [Client]
    private void Update()
    {
        if (isLocalPlayer)
        {
            if (!lockMovement)
            {
                float xAxisRaw = Input.GetAxisRaw("Horizontal");
                float zAxisRaw = Input.GetAxisRaw("Vertical");
        
                float xMouse = Input.GetAxis("Mouse X");
                float yMouse = Input.GetAxis("Mouse Y");
        
                Vector3 movement = playerCamera.transform.right * xAxisRaw + playerCamera.transform.forward * zAxisRaw;
                movement.Normalize();

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.Translate(movement * runSpeed * Time.deltaTime, Space.World);
                }
                else
                {
                    transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
                }
        
                Vector3 rotate = new Vector3(-yMouse * sensetivity,Math.Clamp(xMouse * sensetivity, -90f, 90f), 0);
        
                playerCamera.transform.eulerAngles += rotate;
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                chat.TurnUIOn();
                lockMovement = true;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (isGrounded)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    isGrounded = false;
                }
            }
        }
    }

    public void EnableMovement()
    {
        if (isLocalPlayer)
        {
            lockMovement = false;
            chat.TurnUIOff();
        }
    }
    

    [ClientRpc]
    private void RpcCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
        }
    }
}
