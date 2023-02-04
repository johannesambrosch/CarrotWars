using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    public float walkSpeed = 0.1f;

    public Transform rifleAnchor;

    private enum walkDirection
    {
        none,
        left,
        right,
        up,
        down
    }

    private List<walkDirection> stashedWalkDirections;

    void Awake()
    {
        stashedWalkDirections = new List<walkDirection>();
    }

    private void Update()
    {
        CheckForWalkInputs();
        CheckForActionInputs();
    }


    void FixedUpdate()
    {
        DoWalk();
    }

    private void CheckForWalkInputs()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            stashedWalkDirections.Add(walkDirection.left);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            stashedWalkDirections.Add(walkDirection.up);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            stashedWalkDirections.Add(walkDirection.right);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            stashedWalkDirections.Add(walkDirection.down);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            stashedWalkDirections.Remove(walkDirection.left);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            stashedWalkDirections.Remove(walkDirection.up);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            stashedWalkDirections.Remove(walkDirection.right);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            stashedWalkDirections.Remove(walkDirection.down);
        }
    }

    private void CheckForActionInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryShoot();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandlePlayerAction();
        }
    }

    private void TryShoot()
    {
        throw new NotImplementedException();
    }

    private void HandlePlayerAction()
    {
        throw new NotImplementedException();
    }

    private void DoWalk()
    {
        if (stashedWalkDirections.Count > 0)
        {
            walkDirection curDirection = stashedWalkDirections[stashedWalkDirections.Count - 1];

            switch (curDirection)
            {
                case walkDirection.left:
                case walkDirection.right:
                    float horizontalMovement = Input.GetAxis("Horizontal");
                    horizontalMovement = horizontalMovement * walkSpeed;
                    transform.Translate(new Vector3(horizontalMovement, 0, 0));
                    break;
                case walkDirection.up:
                case walkDirection.down:
                    float verticalMovement = Input.GetAxis("Vertical");
                    verticalMovement = verticalMovement * walkSpeed;
                    transform.Translate(new Vector3(0, verticalMovement, 0));
                    break;
                default:
                    break;
            }
        }
    }
}
