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

    public GameObject ShovelColliderLeft,
        ShovelColliderUp,
        ShovelColliderRight,
        ShovelColliderDown;

    private LookDirections currentLookDirection = LookDirections.right;
    private States currentState = States.idle;

    private bool hasShovel = true;

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

        if (hasShovel)
            StartCoroutine(SmackCoroutine(currentLookDirection));
        else
            StartCoroutine(ShootCoroutine(currentLookDirection));
    }

    private void HandlePlayerAction()
    {
        //handle shop proximity etc.

        if (currentState != States.attacking)
        {
            hasShovel = !hasShovel;
            animator.SetBool("hasShovel", hasShovel);
        }
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

    private IEnumerator SmackCoroutine(LookDirections attackDirection)
    {
        currentState = States.attacking;
        string triggerName;

        switch (attackDirection)
        {
            case LookDirections.left:
                triggerName = "attackLeft";
                ShovelColliderLeft.SetActive(true);
                break;
            case LookDirections.right:
                triggerName = "attackRight";
                ShovelColliderRight.SetActive(true);
                break;
            case LookDirections.up:
                triggerName = "attackUp";
                ShovelColliderUp.SetActive(true);
                break;
            case LookDirections.down:
                triggerName = "attackDown";
                ShovelColliderDown.SetActive(true);
                break;
            default:
                triggerName = "attackLeft";
                ShovelColliderLeft.SetActive(true);
                break;
        }
        animator.SetTrigger(triggerName);

        yield return new WaitForSeconds(attackDuration);

        ShovelColliderLeft.SetActive(false);
        ShovelColliderRight.SetActive(false);
        ShovelColliderUp.SetActive(false);
        ShovelColliderDown.SetActive(false);

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

    private IEnumerator ShootCoroutine(LookDirections attackDirection)
    {
        currentState = States.attacking;
        string triggerName;
        Vector2 shootDirection;

        switch (attackDirection)
        {
            case LookDirections.left:
                triggerName = "attackLeft";
                shootDirection = Vector2.left;
                break;
            case LookDirections.right:
                triggerName = "attackRight";
                shootDirection = Vector2.right;
                break;
            case LookDirections.up:
                triggerName = "attackUp";
                shootDirection = Vector2.up;
                break;
            case LookDirections.down:
                triggerName = "attackDown";
                shootDirection = Vector2.down;
                break;
            default:
                triggerName = "attackLeft";
                shootDirection = Vector2.left;
                break;
        }
        animator.SetTrigger(triggerName);

        int layerMask = LayerMask.GetMask("Rabbit");
        RaycastHit2D hit = Physics2D.Raycast(rifleAnchor.transform.position, shootDirection, Mathf.Infinity, layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Rabbit"))
        {
            hit.collider.GetComponent<Rabbit>().GetShot();
        }

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
