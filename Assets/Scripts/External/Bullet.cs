using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviourPun
{

    public bool normalAttack;
    public float parabolicSpeed;

    public float speed;
    public float maxTime;

    public float bulletDamage;

    public GameObject owner;
    
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

    public void FixedUpdate()
    {
        
        AttackBullet();

    }

    public void AttackBullet()
    {
        _rb.velocity = speed * _dir;
    }


    public void AttackParabolic()
    {
        _rb.useGravity = true;
        _rb.velocity = Vector3.zero;
        _rb.AddForce(((_rb.transform.forward) + Vector3.up) * parabolicSpeed);
    }

    public void SetDirection(Vector3 value)
    {
        _dir = value;
    }
}
