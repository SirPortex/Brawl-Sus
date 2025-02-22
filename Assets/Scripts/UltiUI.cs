using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Com.MyCompany.MyGame;

public class UltiUI : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerUltiSlider;

    private PlayerMovement target;

    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

    }

    // Update is called once per frame
    void Update()
    {
        if(playerUltiSlider != null)
        {
            playerUltiSlider.value = target.ulti;
        }

        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }
    void LateUpdate()
    {
        
    }
    public void SetTarget(PlayerMovement _target)
    {
        if (_target == null)
        {
            return;
        }

        target = _target;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
