using System;
using Mirror;
using TMPro;
using UnityEngine;

public class ChatBehavior : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;

    private static event Action<string> OnMessage;

    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);

        OnMessage += HandleMessage;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!isOwned)
        {
            return;
        }
        OnMessage -= HandleMessage;
    }

    private void HandleMessage(string message)
    {
        chatText.text += message;
    }

    [Client]
    public void Send(string message)
    {
        if(!Input.GetKeyDown(KeyCode.Return)) {return;}
        
        if(string.IsNullOrWhiteSpace(message)) {return;}

        CmdSendMessage(inputField.text);
        
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}\n");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke(message);
    }
}
