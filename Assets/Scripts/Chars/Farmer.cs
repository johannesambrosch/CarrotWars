using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    public float walkSpeed = 0.1f;
    public float attackDuration = 0.5f;

    public Transform rifleAnchor;

    public Animator animator;

    private LookDirections currentLookDirection = LookDirections.right;
    private States currentState = States.idle;

    private enum LookDirections
    {
        left,
        right,
        up,
        down
    }

    private enum States
    {
        idle,
        walking,
        attacking
    }

    private List<LookDirections> stashedLookDirections;

    void Awake()
    {
        stashedLookDirections = new List<LookDirections>();
    }

    private void Update()
    {
        CheckForWalkInputs();
        CheckForActionInputs();
    }


    void FixedUpdate()
    {
        if (currentState == States.walking)
            DoWalkMovement();
    }

    private void CheckForWalkInputs()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            stashedLookDirections.Add(LookDirections.left);
            StartWalking();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            stashedLookDirections.Add(LookDirections.up);
            StartWalking();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            stashedLookDirections.Add(LookDirections.right);
            StartWalking();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            stashedLookDirections.Add(LookDirections.down);
            StartWalking();
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            stashedLookDirections.Remove(LookDirections.left);
            StartWalking();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            stashedLookDirections.Remove(LookDirections.up);
            StartWalking();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            stashedLookDirections.Remove(LookDirections.right);
            StartWalking();
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            stashedLookDirections.Remove(LookDirections.down);
            StartWalking();
        }
    }

    private void StartWalking()
    {
        if (currentState == States.attacking)
            return;

        currentState = States.walking;

        if (stashedLookDirections.Count > 0)
        {
            currentLookDirection = stashedLookDirections[stashedLookDirections.Count - 1];
            switch (currentLookDirection)
            {
                case LookDirections.left:
                    animator.SetTrigger("walkLeft");
                    break;
                case LookDirections.right:
                    animator.SetTrigger("walkRight");
                    break;
                case LookDirections.up:
                    animator.SetTrigger("walkUp");
                    break;
                case LookDirections.down:
                    animator.SetTrigger("walkDown");
                    break;
            }
        }
        else
        {
            currentState = States.idle;
            animator.SetTrigger("idle");
        }
    }

    private void CheckForActionInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAttack();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandlePlayerAction();
        }
    }

    private void TryAttack()
    {
        if (currentState == States.attacking)
            return;

        StartCoroutine(AttackCoroutine(currentLookDirection));
    }

    private void HandlePlayerAction()
    {
        throw new NotImplementedException();
    }

    private void DoWalkMovement()
    {
        switch (currentLookDirection)
        {
            case LookDirections.left:
            case LookDirections.right:
                float horizontalMovement = Input.GetAxis("Horizontal");
                horizontalMovement = horizontalMovement * walkSpeed;
                transform.Translate(new Vector3(horizontalMovement, 0, 0));
                break;
            case LookDirections.up:
            case LookDirections.down:
                float verticalMovement = Input.GetAxis("Vertical");
                verticalMovement = verticalMovement * walkSpeed;
                transform.Translate(new Vector3(0, verticalMovement, 0));
                break;
            default:
                break;
        }
    }

    private IEnumerator AttackCoroutine(LookDirections attackDirection)
    {
        currentState = States.attacking;
        string triggerName;

        switch (attackDirection)
        {
            case LookDirections.left:
                triggerName = "attackLeft";
                break;
            case LookDirections.right:
                triggerName = "attackRight";
                break;
            case LookDirections.up:
                triggerName = "attackUp";
                break;
            case LookDirections.down:
                triggerName = "attackDown";
                break;
            default:
                triggerName = "attackLeft";
                break;
        }
        animator.SetTrigger(triggerName);

        yield return new WaitForSeconds(attackDuration);


        if (stashedLookDirections.Count > 0)
        {
            currentState = States.walking;
            switch (currentLookDirection)
            {
                case LookDirections.left:
                    triggerName = "walkLeft";
                    break;
                case LookDirections.right:
                    triggerName = "walkRight";
                    break;
                case LookDirections.up:
                    triggerName = "walkUp";
                    break;
                case LookDirections.down:
                    triggerName = "walkDown";
                    break;
                default:
                    break;
            }
        }
        else
        {
            triggerName = "idle";
            currentState = States.idle;
        }

        animator.SetTrigger(triggerName);
    }
}
