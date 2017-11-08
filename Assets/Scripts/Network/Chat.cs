using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Chat : NetworkBehaviour {

    public Text chatText;
    public Text playerNameText;
    public InputField inputField, nameInput;
    public static List<string> messageList = new List<string>();

    public int maxChatlogCount = 100;
    private DataService ds;

    public PlayerProfile currentProfile;
    PlayerProfileController playerProfileController;
    [SyncVar]
    public string pName = "";

    private void Start()
    {
        chatText = GameObject.Find("chatText").GetComponent<Text>();
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
        nameInput = GameObject.Find("nameInputField").GetComponent<InputField>();
        if (!isLocalPlayer)
        {
            enabled = false;
            this.gameObject.name = pName + this.GetComponent<NetworkIdentity>().netId;
            playerNameText.text = pName;
        }
        if (isLocalPlayer)
        {
            CmdGetPlayerProfile(nameInput.text);
            UserInterfaceController.TransitionToGameUI();
        }
    }

    [Command]
    void CmdGetPlayerProfile(string name)
    {
        playerProfileController = new PlayerProfileController();
        PlayerProfile _profile = playerProfileController.GetPlayerProfile(name);
        RpcSetPlayerProfile(_profile.Id, _profile.Name, _profile.Level);
    }

    [ClientRpc]
    void RpcSetPlayerProfile(int id, string name, int level)
    {
        currentProfile = new PlayerProfile();
        currentProfile.Id = id;
        currentProfile.Name = name;
        currentProfile.Level = level;
        this.pName = name;
        playerNameText.text = pName;
        this.gameObject.name = pName + this.GetComponent<NetworkIdentity>().netId;
    }

    [Command]
    void CmdListPlayers()
    {
        foreach (var person in playerProfileController.GetPlayers())
        {
            print(person.ToString());
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string _message = inputField.text;
            if (_message.Length == 0) return;

            inputField.text = "";
            CmdSendChatMessage( currentProfile.Name + "(Level " + currentProfile.Level + "): " + _message);

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inputField.text = "";
            CmdListPlayers();
        }

    }

    [Command]
    public void CmdSendChatMessage(string message)
    {
        RpcSendChatMessage(message);
    }


    [ClientRpc]
    void RpcSendChatMessage(string message)
    {
        if(messageList.Count > maxChatlogCount)
        {
            messageList.RemoveRange(0, maxChatlogCount/2);
        }
        messageList.Add(message);
        //GameObject.Find("Scroll View").GetComponent<ScrollRect>().verticalNormalizedPosition = 0.5f;
        chatText.text = "";
        foreach (string s in messageList)
        {
            chatText.text += s + "\n";
        }
    }

}
