using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using ExitGames.Client.Photon;
using TMPro;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    [SerializeField] TMP_InputField _inputField; //El input del player
    ChatClient _chatClient; //Nos permite recibir y enviar mensajes
    string _channel; //El canal al cual estoy suscripto
    [SerializeField] TextMeshProUGUI content;
    [SerializeField] ScrollRect _scroll;

    private void Start()
    {
        DontDestroyOnLoad(this);

        if (!PhotonNetwork.IsConnected) Destroy(this);

        //Inicializo el chat client
        _chatClient = new ChatClient(this);

        //Agarro el nombre del channel
        _channel = PhotonNetwork.CurrentRoom.Name;

        //Lo conecto al chat de photon
        _chatClient.Connect(
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, 
            PhotonNetwork.AppVersion, 
            new AuthenticationValues(PhotonNetwork.NickName/* + "#" + PhotonNetwork.LocalPlayer.UserId*/));

        //Ahora se ejecuta el evento OnConnected
    }

    private void Update()
    {
        _chatClient.Service(); //Actualiza la info del chat

        //Cuando el usuario apreta la Y, podrá escribir en el chat
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _inputField.Select();
        }
    }

    //Funcion que envia un mensaje
    public void SendMsg()
    {
        //Agarro el mensaje del jugador
        string msg = _inputField.text;

        //Si esta vacio, no hago nada
        if (string.IsNullOrEmpty(msg) || string.IsNullOrWhiteSpace(msg)) return;

        //Mando el mensaje
        _chatClient.PublishMessage(_channel, msg);

        //Vacio el campo de texto
        _inputField.text = "";
    }

    public void SendSpecialMsg(string msg)
    {
        _chatClient.PublishMessage(_channel, msg);
    }

    IEnumerator WaitToScroll()
    {
        yield return new WaitForEndOfFrame();
        _scroll.verticalNormalizedPosition = 0;
    }

    #region ~~~ INTERFAZ FUNCTION CHAT ~~~
    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnConnected()
    {
        print("Chat conectado");

        //Aca suscribimos o creamos el chat
        _chatClient.Subscribe(_channel);
    }

    public void OnDisconnected()
    {
        print("Chat desconectado");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            if (senders[i] == PhotonNetwork.NickName) content.text += "\n" + "<color=blue>" + senders[i] + "</color>" + ": " + messages[i];
            else content.text += "\n" + "<color=red>" +senders[i] + "</color>" + ": " + messages[i];
        }
        if (_scroll.verticalNormalizedPosition < 0.3f)
            StartCoroutine(WaitToScroll());
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
    #endregion
}