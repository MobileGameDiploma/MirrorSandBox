using System;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;

    public GameObject playerCamera;
    public float sensetivity = 1;
    
    public ChatBehavior chat;
    public Canvas localPlayerCanvas;
    
    private bool LockMovement = false;
    
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
            if (!LockMovement)
            {
                float xAxisRaw = Input.GetAxisRaw("Horizontal");
                float zAxisRaw = Input.GetAxisRaw("Vertical");
        
                float xMouse = Input.GetAxis("Mouse X");
                float yMouse = Input.GetAxis("Mouse Y");
        
                Vector3 movement = playerCamera.transform.right * xAxisRaw + playerCamera.transform.forward * zAxisRaw;
                movement.Normalize();
        
                transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
        
                Vector3 rotate = new Vector3(-yMouse * sensetivity,Math.Clamp(xMouse * sensetivity, -90f, 90f), 0);
        
                playerCamera.transform.eulerAngles += rotate;
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                chat.TurnUIOn();
                LockMovement = true;
            }
            
        }
    }

    public void EnableMovement()
    {
        if (isLocalPlayer)
        {
            LockMovement = false;
            chat.TurnUIOff();
        }
    }
    

    [ClientRpc]
    private void RpcCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
