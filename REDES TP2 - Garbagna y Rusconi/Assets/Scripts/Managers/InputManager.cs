using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InputManager : MonoBehaviour
{
    Vector3 movement;
    
    float horizontalRotation = 0;
    float verticalRotation = 0;
    float verticalRotationLimit = 80f;

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

    bool _canSendInfo;


    //rotation vars
    const float diff = 2;
    const float _mouseSensitivity = 75f;

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
        _canSendInfo = true;
    }

    void Update()
    {
        if (PlayerManager.Instance.Spawned)
        {
            ManageMovementInput();
            ManageCameraInput();
        }
    }

    void ManageCameraInput()
    {
        // Muevo el objeto que uso para hacer los calculos localmente (osea este)
        var rotation = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);

        // Agarro la rotacion acutal (en grados)
        var actualRot = Mathf.RoundToInt(transform.localEulerAngles.y);

        // Calculo los diferenciales
        float negativeDiff = originalPos - diff; // Negativo
        float positiveDiff = originalPos + diff; // Positivo


        if (actualRot < negativeDiff || actualRot > positiveDiff)
        {
            //controlled.transform.localEulerAngles = transform.localEulerAngles;
            GameServer.Instance.photonView.RPC("RequestAim", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, transform.localEulerAngles.y);
            originalPos = actualRot;
            //updateRot = false;
            //StartCoroutine(WaitToSyncRot());
        }



        /*
        var diff = yRot_new - yRot_past;
        Debug.Log(diff);
        if (diff > _minValueToRotate / 2 || diff < -1 * _minValueToRotate / 2)
        {
            GameServer.Instance.photonView.RPC("RequestAim", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, diff);
            yRot_past = yRot_new;
        }
        */
    }

    void ManageMovementInput()
    {
        /*
        // Vertical Movement
        if (Input.GetKeyDown(KeyCode.W))
            GameServer.Instance.photonView.RPC("RequestInputPress", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, _moveForward, "");
        else if (Input.GetKeyUp(KeyCode.W))
            GameServer.Instance.photonView.RPC("RequestInputRelease", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, _moveForward, "");

        if (Input.GetKeyDown(KeyCode.S))
            GameServer.Instance.photonView.RPC("RequestInputPress", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, _moveBackward, "");
        else if (Input.GetKeyUp(KeyCode.S))
            GameServer.Instance.photonView.RPC("RequestInputRelease", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, _moveBackward, "");

        // Horizontal Movement
        if (Input.GetKeyDown(KeyCode.A))
            GameServer.Instance.photonView.RPC("RequestInputPress", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, "", _moveLeft);
        else if (Input.GetKeyUp(KeyCode.A))
            GameServer.Instance.photonView.RPC("RequestInputRelease", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, "", _moveLeft);

        if (Input.GetKeyDown(KeyCode.D))
            GameServer.Instance.photonView.RPC("RequestInputPress", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, "", _moveRight);
        else if (Input.GetKeyUp(KeyCode.D))
            GameServer.Instance.photonView.RPC("RequestInputRelease", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, "", _moveRight);
        */


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


        // horizontalMovement

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

        SendInput();
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
}
