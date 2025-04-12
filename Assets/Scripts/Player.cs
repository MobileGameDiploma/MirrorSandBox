using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Client]
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdMovement();
        }
    }

    [Command]
    public void CmdMovement()
    {
        RpcMovement();
    }

    [ClientRpc]
    public void RpcMovement()
    {
        transform.Translate(Vector3.forward);
    }
}
