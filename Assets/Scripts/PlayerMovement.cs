using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkingSpeed, rotationSpeed, aceleration, sphereRadius;//,gravityScale;

    public float life = 100;


    public string groundName;

    public Animator animator;

    private Vector3 movementVector;

    private Rigidbody rb;
    private float x, z, mouseX; //input
 
    private float currentSpeed;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        //gravityScale = -Mathf.Abs(gravityScale); //Valor Absoluto

    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxisRaw("Mouse X");



        InterpolationSpeed();

        RotatePlayer();
    }



    public void InterpolationSpeed()
    {

        if (x != 0 || z != 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, walkingSpeed, aceleration * Time.deltaTime); // velocidad de caminar
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, aceleration * Time.deltaTime); // velocidad de idle
        }
    }

    void RotatePlayer()
    {
        Vector3 rotation = new Vector3(0, mouseX, 0) * rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation); // Se aplica la rotacion, tiene numero imaginarios
    }
    private void FixedUpdate()
    {
        ApplySpeed();


    }

    void ApplySpeed()
    {
        rb.velocity = (transform.forward * currentSpeed * z) + (transform.right * currentSpeed * x) +
            new Vector3(0, rb.velocity.y, 0); //Gravedad base de Unity.
        //+ (transform.up * gravityScale); //Gravedad constante no realista
        //rb.AddForce(transform.up * gravityScale);
    }

    private bool IsGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x,
            transform.position.y - transform.localScale.y / 20, transform.position.z), sphereRadius);

        for (int i = 0; i < colliders.Length; i++) //recorremos elemento a elemento.
        {
            // y comprobamos si el elemento es suelo o no.
            if (colliders[i].gameObject.layer == LayerMask.NameToLayer(groundName)) //Recorre cada elemento del array para ver si tocamos suelo
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos() //Raycast de la esfera.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x,
            transform.position.y - transform.localScale.y / 20, transform.position.z), sphereRadius);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;

    }
}
