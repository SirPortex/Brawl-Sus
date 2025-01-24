using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void LateUpdate()
    {
        animator.SetFloat("Speed", playerMovement.GetCurrentSpeed() / playerMovement.walkingSpeed); //Variable de Speed del BlendTree que vaya relacionado con nuestra velocidada

    }
}
