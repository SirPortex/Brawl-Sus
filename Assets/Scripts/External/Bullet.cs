using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{


    public float speed;
    public float maxTime;
    
    private float currentTime;
    private Vector3 _dir;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxTime)
        {
            currentTime = 0;
            gameObject.SetActive(false); //Se "devuelve" a la pool
            GetComponent<PoolObject>().readyToUse = true;
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = speed * _dir;
    }

    public void SetDirection(Vector3 value)
    {
        _dir = value;
    }

    
}
