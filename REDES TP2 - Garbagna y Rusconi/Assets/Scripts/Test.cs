using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    float yRot_past;
    public float limit;

    public GameObject controlled;
    public float value;

    bool updateRot;

    float originalPos;
    // Start is called before the first frame update
    void Start()
    {
        //yRot_past = transform.rotation.y; //Pos1


        StartCoroutine(WaitToSyncRot()); //Pos 2
        //updateRot = true; // Pos 2

        originalPos = Mathf.RoundToInt(transform.localEulerAngles.y); //Pos 3
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxis("Mouse X"));
        /*value = transform.rotation.y;

        var rotation = Input.GetAxis("Mouse X") * 75f * Time.deltaTime;

        transform.Rotate(0f, rotation, 0f);
        var yRot_new = transform.rotation.y;

        var diff = yRot_new - yRot_past;

        if (diff > limit / 2 || diff < -1 * limit / 2)
        {
            //Debug.Log(diff);
            //GameServer.Instance.photonView.RPC("RequestAim", GameServer.Instance.Server, PhotonNetwork.LocalPlayer, diff);
            controlled.transform.rotation = transform.rotation;
            yRot_past = yRot_new;
            Debug.LogError("Entre a la rotación");
        }*/

        // ~~~~~~~ POSIBILIDAD 1 ~~~~~~~

        /*var rotation = Input.GetAxis("Mouse X") * 75f * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);

        if (updateRot)
        {
            controlled.transform.rotation = transform.rotation;
            updateRot = false;
            StartCoroutine(WaitToSyncRot());
        }*/

        // ~~~~~~~ POSIBILIDAD 2 ~~~~~~~
        var rotation = Input.GetAxis("Mouse X") * 2f;
        transform.Rotate(0f, rotation, 0f);

        var actualRot = Mathf.RoundToInt(transform.localEulerAngles.y);

        float diff = 2;

        value = transform.localEulerAngles.y;

        float negativeDiff = originalPos - diff;

        float positiveDiff = originalPos + diff;

        if (actualRot < negativeDiff || actualRot > positiveDiff && updateRot)
        {
            controlled.transform.rotation = transform.rotation;
            originalPos = actualRot;
            updateRot = false;
            StartCoroutine(WaitToSyncRot());
        }
    }

    IEnumerator WaitToSyncRot()
    {
        yield return new WaitForSeconds(0.1f);
        updateRot = true;
    }
}
