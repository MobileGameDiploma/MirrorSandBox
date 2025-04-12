using System;
using Mirror;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;

    public GameObject playerCamera;
    public float sensetivity = 1;
    
    public override void OnStartClient()
    {
        if (!isLocalPlayer)
        {
            enabled = false;
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
        CmdCursorLock();
    }
    
    [Client]
    private void Update()
    {
        CmdMove();
        CmdCameraMove();
    }

    [Command]
    private void CmdMove()
    {
        RpcMove();
    }

    [Command]
    private void CmdCameraMove()
    {
        RpcCameraMove();
    }

    [Command]
    private void CmdCursorLock()
    {
        RpcCursorLock();
    }

    [ClientRpc]
    private void RpcMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        
        Vector3 movement = playerCamera.transform.right * x + playerCamera.transform.forward * z;
        movement.Normalize();
        
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }

    [ClientRpc]
    private void RpcCameraMove()
    {
        float x = Input.GetAxis("Mouse Y");
        float y = Input.GetAxis("Mouse X");
        
        Vector3 rotate = new Vector3(-x * sensetivity,Math.Clamp(y * sensetivity, -90f, 90f), 0);
        
        playerCamera.transform.eulerAngles += rotate;
    }

    [ClientRpc]
    private void RpcCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
