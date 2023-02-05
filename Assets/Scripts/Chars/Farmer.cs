using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    public static Farmer instance;

    public float walkSpeed = 0.1f;
    public float attackDuration = 0.5f;

    public Transform rifleAnchor;

    public Animator animator;

    public ProgressBar staminaBar;
    private float stamina = 1f;
    public float staminaRegenDuration = 3f;

    public GameObject ammoShellsDisplay;
    public List<GameObject> ammoShells;
    private int ammo = 0;

    public GameObject ShovelColliderLeft,
        ShovelColliderUp,
        ShovelColliderRight,
        ShovelColliderDown;

    private LookDirections currentLookDirection = LookDirections.right;
    private States currentState = States.idle;

    private bool hasShovelEquipped = true;

    private Vector3 originalPosition;

    private bool isDriving = false;


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
    internal bool tractorSelected = false;
    internal int carrotCount = 0;
    internal bool gasStationSelected = false;
    internal bool gunStoreSelected = false;
    private bool hasGunInInventory = false;
    private const int maxCarrots = 8;
    internal float fuelLeft = 0f;

    void Awake()
    {
        instance = this;
        stashedLookDirections = new List<LookDirections>();
        originalPosition = transform.position;

        stamina = staminaRegenDuration;
    }

    private void Update()
    {
        if (GameManager.instance.currentState == GameManager.States.Preparation
            || GameManager.instance.currentState == GameManager.States.Game)
        {
            CheckForWalkInputs();
            CheckForActionInputs();
        }
        else
        {
            animator.ResetTrigger("attackLeft");
            animator.ResetTrigger("attackRight");
            animator.ResetTrigger("attackUp");
            animator.ResetTrigger("attackDown");
            animator.ResetTrigger("walkLeft");
            animator.ResetTrigger("walkRight");
            animator.ResetTrigger("walkUp");
            animator.ResetTrigger("walkDown");
            animator.SetTrigger("idle");
        }

        stamina += Time.deltaTime;
        staminaBar.SetProgress(Mathf.InverseLerp(0, staminaRegenDuration, stamina));
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
            StartOrStopWalking();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            stashedLookDirections.Add(LookDirections.up);
            StartOrStopWalking();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            stashedLookDirections.Add(LookDirections.right);
            StartOrStopWalking();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            stashedLookDirections.Add(LookDirections.down);
            StartOrStopWalking();
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            stashedLookDirections.Remove(LookDirections.left);
            StartOrStopWalking();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            stashedLookDirections.Remove(LookDirections.up);
            StartOrStopWalking();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            stashedLookDirections.Remove(LookDirections.right);
            StartOrStopWalking();
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            stashedLookDirections.Remove(LookDirections.down);
            StartOrStopWalking();
        }

        // Sanity check back to idle
        else if (!Input.GetKey(KeyCode.W)
            && !Input.GetKey(KeyCode.A)
            && !Input.GetKey(KeyCode.S)
            && !Input.GetKey(KeyCode.D)
            && stashedLookDirections.Count > 0)
        {
            stashedLookDirections = new List<LookDirections>();
            StartOrStopWalking();
        }
    }

    private void StartOrStopWalking()
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
            if (GameManager.instance.currentState == GameManager.States.Game)
            {
                if (stamina >= staminaRegenDuration)
                {
                    TryAttack();
                }
            }
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


        if (hasShovelEquipped)
        {
            stamina = 0f;
            StartCoroutine(SmackCoroutine(currentLookDirection));
        }
        else
        {
            if (ammo > 0)
            {
                stamina = 0f;
                ammo--;
                ammoShells[ammo].SetActive(false);
                StartCoroutine(ShootCoroutine(currentLookDirection));
            }
        }
    }

    private void HandlePlayerAction()
    {
        if (tractorSelected)
        {
            Tractor.instance.StartTractor();
            StartDriving();
        }
        else if (gasStationSelected)
        {
            if (carrotCount >= GasStation.instance.price)
            {
                fuelLeft = 1f;
                carrotCount -= GasStation.instance.price;
                GameManager.instance.UpdateCarrotDisplay();
                GasStation.instance.DisableShop();
            }
        }
        else
        {
            if (gunStoreSelected)
            {
                hasGunInInventory = true;
                ammo = ammoShells.Count;
                ammoShells.ForEach((shell) => shell.SetActive(true));
                GunStore.instance.DisableShop();
            }
            if (currentState != States.attacking)
            {
                if (!hasGunInInventory && hasShovelEquipped)
                    return;

                hasShovelEquipped = !hasShovelEquipped;
                animator.SetBool("hasShovel", hasShovelEquipped);

                ammoShellsDisplay.SetActive(!hasShovelEquipped);
                GameManager.instance.UpdateWeaponDisplay(hasShovelEquipped, hasGunInInventory);
            }
        }
    }

    private void StartDriving()
    {
        isDriving = true;
        gameObject.SetActive(false);
    }

    public void StopDriving(Transform tractorPosition)
    {
        isDriving = false;
        gameObject.SetActive(true);
        transform.position = tractorPosition.position + Vector3.left;
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

    public void DoReset()
    {
        StopAllCoroutines();
        transform.position = originalPosition;
        stashedLookDirections = new List<LookDirections>();
        currentState = States.idle;
        hasShovelEquipped = true;
        animator.SetBool("hasShovel", true);
        animator.SetTrigger("idle");
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

    internal void GainCarrots(int numCarrots)
    {
        carrotCount += numCarrots;
        if (carrotCount > maxCarrots)
        {
            carrotCount = maxCarrots;
        }
    }
}
