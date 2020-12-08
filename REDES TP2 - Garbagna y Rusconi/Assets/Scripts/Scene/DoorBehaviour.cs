using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorBehaviour : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Vector3 closePosition;
    [SerializeField] Vector3 openPosition;
    [SerializeField] bool open;
    [SerializeField] bool close;
    [SerializeField] bool moving = false;
    [SerializeField] bool isOpen;
    [SerializeField] bool isClosed;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        closePosition = transform.position;
        openPosition = transform.position;
        openPosition.y -= 6;

        isClosed = true;

        InvokeRepeating("Open", Random.Range(5, 10), Random.Range(5, 10));
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (moving == true)
        { 
            if(open == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, openPosition, Time.deltaTime * speed);
                if (transform.position == openPosition)
                {
                    moving = false;
                    open = false;
                    isOpen = true;
                }
            }
            else if (close == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, closePosition, Time.deltaTime * speed);
                if (transform.position == closePosition)
                {
                    moving = false;
                    close = false;
                    isClosed = true;
                }
            }
        }
    }

    public void Open()
    {
        if (isClosed == true)
        {
            isClosed = false;
            moving = true;
            open = true;

            Invoke("Close", Random.Range(4, 8));
        }
    }

    public void Close()
    {
        if (isOpen == true)
        {
            isOpen = false;
            moving = true;
            close = true;
        }
    }
}
