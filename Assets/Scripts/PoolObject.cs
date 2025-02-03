using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PoolObject : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool readyToUse = true;


    private void Update()
    {
       // gameObject.SetActive(!readyToUse);    
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(readyToUse);
            gameObject.SetActive(!readyToUse); //corrtutine

        }
        else
        {
            readyToUse = (bool)stream.ReceiveNext();
            gameObject.SetActive(!readyToUse);
        }
    }
}
