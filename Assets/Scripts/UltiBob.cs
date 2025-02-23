using Com.MyCompany.MyGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltiBob : MonoBehaviourPun
{
    public PlayerMovement Bob;

    // Start is called before the first frame update
    void Start()
    {
        Bob = FindObjectOfType<PlayerMovement>();
     
        //Bob = FindAnyObjectByType<PlayerMovement>();
        Bob.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Bob.ultimateReady && Input.GetKeyDown(KeyCode.Q))
        {
            Bob.animator.SetBool("IsUlting", true);
            Bob.ulti = 0;
            Bob.ultimateReady = false;
            StartCoroutine(UltiBobRoutine());
        }
    }
   
    IEnumerator UltiBobRoutine()
    {
        Bob.animator.SetBool("IsUlting", true);
        Bob.ulti = 0;
        Bob.ultimateReady = false;
        yield return new WaitForSeconds(Bob.ultiDuration);
        Bob.animator.SetBool("IsUlting", false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
