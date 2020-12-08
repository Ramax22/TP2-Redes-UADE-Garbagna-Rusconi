using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InputManager : MonoBehaviour
{
    //general vars
    bool _canSendInfo;

    //movement vars
    const string _moveForward = "up";
    const string _moveBackward = "down";
    const string _moveLeft = "left";
    const string _moveRight = "right";

    const string _pressAction = "press";
    const string _releaseAction = "release";

    const float _timeToSendInfo = 0.5f;

    bool horizontalMovement, verticalMovement;
    List<string> _inputList = new List<string>();


    //rotation vars
    const float diff = 2;
    const float _mouseSensitivity = 2f;
    bool updateRot;

    float originalPos;

    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
            Destroy(this.gameObject);
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient) return;

        originalPos = Mathf.RoundToInt(transform.localEulerAngles.y);
        StartCoroutine(Wait());
        StartCoroutine(WaitToSyncRot());
        _canSendInfo = true;
        updateRot = true;
    }

    void Update()
    {
        if (PlayerManager.Instance.Spawned)
        {

            if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = CursorLockMode.None;

            if (Input.GetButtonDown("Fire1")) RequestShoot();

            ManageMovementInput();
            ManageCameraInput();
            SendInput();
        }
    }

    void RequestShoot() { GameServer.Instance.photonView.RPC("RequestShoot", GameServer.Instance.Server, PhotonNetwork.LocalPlayer); }

    void ManageCameraInput()
    {
        // Muevo el objeto que uso para hacer los calculos localmente (osea este)
        var rotation = Input.GetAxis("Mouse X") * _mouseSensitivity;
        transform.Rotate(0f, rotation, 0f);

        // Agarro la rotacion acutal (en grados)
        var actualRot = Mathf.RoundToInt(transform.localEulerAngles.y);

        // Calculo los diferenciales
        float negativeDiff = originalPos - diff; // Negativo
        float positiveDiff = originalPos + diff; // Positivo


        if (actualRot < negativeDiff || actualRot > positiveDiff && updateRot)
        {
            //controlled.transform.localEulerAngles = transform.localEulerAngles;
            GameServer.Instance.photonView.RPC("RequestAim", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, transform.localEulerAngles.y);
            originalPos = actualRot;
            updateRot = false;
        }
    }

    void ManageMovementInput()
    {
        // Vertical Movement
        if (Input.GetKeyDown(KeyCode.W) && !verticalMovement)
        {
            _inputList.Add(_moveForward + "-" + _pressAction); //up-press
            verticalMovement = true;
        }
        else if (Input.GetKeyUp(KeyCode.W) && verticalMovement)
        {
            _inputList.Add(_moveForward + "-" + _releaseAction); //up-release
            verticalMovement = false;
        }

        if (Input.GetKeyDown(KeyCode.S) && !verticalMovement)
        {
            _inputList.Add(_moveBackward + "-" + _pressAction); //back-press
            verticalMovement = true;
        }
        else if (Input.GetKeyUp(KeyCode.S) && verticalMovement)
        {
            _inputList.Add(_moveBackward + "-" + _releaseAction); //back-release
            verticalMovement = false;
        }

        // Horizontal Movement
        if (Input.GetKeyDown(KeyCode.A) && !horizontalMovement)
        {
            _inputList.Add(_moveLeft + "-" + _pressAction); //left-press
            horizontalMovement = true;
        }
        else if (Input.GetKeyUp(KeyCode.A) && horizontalMovement)
        {
            _inputList.Add(_moveLeft + "-" + _releaseAction); //left-release
            horizontalMovement = false;
        }

        if (Input.GetKeyDown(KeyCode.D) && !horizontalMovement)
        {
            _inputList.Add(_moveRight + "-" + _pressAction); //right-press
            horizontalMovement = true;
        }
        else if (Input.GetKeyUp(KeyCode.D) && horizontalMovement)
        {
            _inputList.Add(_moveRight + "-" + _releaseAction); //right-release
            horizontalMovement = false;
        }
    }

    void SendInput()
    {
        if (_inputList.Count > 0)
        {
            string input = _inputList[0];

            if (_canSendInfo)
            {
                _canSendInfo = false;

                if (input.Contains(_pressAction))
                {
                    GameServer.Instance.photonView.RPC("RequestInputPress", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, input);
                }
                else if (input.Contains(_releaseAction))
                {
                    GameServer.Instance.photonView.RPC("RequestInputRelease", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, input);
                }

                _inputList.RemoveAt(0);
            }
        }
    }

    IEnumerator Wait()
    {
        while (true)
        {
            _canSendInfo = true;
            yield return new WaitForSeconds(_timeToSendInfo);
        }
    }

    IEnumerator WaitToSyncRot()
    {
        while (true)
        {
            updateRot = true;
            yield return new WaitForSeconds(_timeToSendInfo);
        }
    }
}