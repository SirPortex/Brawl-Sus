using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiView : MonoBehaviourPunCallbacks, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}