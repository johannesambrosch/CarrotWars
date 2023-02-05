using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractor : MonoBehaviour
{
    public static Tractor instance;

    public GameObject dirtSplash, killZone, selectTile, fuelIcon, fuelBar;

    public ProgressBar fuelGauge;

    private Animator animator;

    private bool isActive = false;

    private LookDirections currentLookDirection = LookDirections.up;

    public float fuelConsumptionPerSecond = 0.125f;

    private Rigidbody2D rb;

    Vector2 currentVelocity;

    public float speed = 10f;

    private enum LookDirections
    {
        left,
        right,
        up,
        down
    }
    private List<LookDirections> stashedLookDirections;

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
        stashedLookDirections = new List<LookDirections>();
        rb = GetComponent<Rigidbody2D>();
        currentVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (!isActive) return;

        if (GameManager.instance.currentState == GameManager.States.Game)
        {
            CheckForDriveInputs();
            //CheckForActionInputs();
        }

        Farmer.instance.fuelLeft -= Time.deltaTime * fuelConsumptionPerSecond;
        fuelGauge.SetProgress(Farmer.instance.fuelLeft);
        if (Farmer.instance.fuelLeft < 0)
        {
            Farmer.instance.fuelLeft = 0;
            StopTractor();
        }
    }

    private void CheckForDriveInputs()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            stashedLookDirections.Add(LookDirections.left);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            stashedLookDirections.Add(LookDirections.up);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            stashedLookDirections.Add(LookDirections.right);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            stashedLookDirections.Add(LookDirections.down);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            stashedLookDirections.Remove(LookDirections.left);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            stashedLookDirections.Remove(LookDirections.up);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            stashedLookDirections.Remove(LookDirections.right);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            stashedLookDirections.Remove(LookDirections.down);
        }
        // Sanity check back to idle
        else if (!Input.GetKey(KeyCode.W)
            && !Input.GetKey(KeyCode.A)
            && !Input.GetKey(KeyCode.S)
            && !Input.GetKey(KeyCode.D)
            && stashedLookDirections.Count > 0)
        {
            stashedLookDirections = new List<LookDirections>();
        }
        StartOrStopDriving();
    }

    private void StartOrStopDriving()
    {
        if (stashedLookDirections.Count > 0)
        {
            dirtSplash.SetActive(true);
            currentLookDirection = stashedLookDirections[stashedLookDirections.Count - 1];
            switch (currentLookDirection)
            {
                case LookDirections.left:
                    transform.eulerAngles = new Vector3(0, 0, 90);
                    rb.AddForce(Vector2.left * Time.deltaTime * speed);
                    break;
                case LookDirections.right:
                    transform.eulerAngles = new Vector3(0, 0, -90);
                    rb.AddForce(Vector2.right * Time.deltaTime * speed);
                    break;
                case LookDirections.up:
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    rb.AddForce(Vector2.up * Time.deltaTime * speed);
                    break;
                case LookDirections.down:
                    transform.eulerAngles = new Vector3(0, 0, 180);
                    rb.AddForce(Vector2.down * Time.deltaTime * speed);
                    break;
            }
        }
        else
        {
            dirtSplash.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.instance.currentState != GameManager.States.Game)
            return;

        if (collision.CompareTag("Farmer"))
        {
            if (Farmer.instance.fuelLeft > 0f)
            {
                selectTile.SetActive(true);
                Farmer.instance.tractorSelected = true;
            }
            else
            {
                fuelIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Farmer"))
        {
            DisableSelect();
        }
    }

    private void DisableSelect()
    {
        selectTile.SetActive(false);
        Farmer.instance.tractorSelected = false;
        fuelIcon.SetActive(false);
    }

    public void StartTractor()
    {
        DisableSelect();

        animator.SetTrigger("startEngine");
        fuelBar.SetActive(true);
        killZone.SetActive(true);
        isActive = true;
    }

    public void StopTractor()
    {
        animator.SetTrigger("stopEngine");
        fuelBar.SetActive(false);
        killZone.SetActive(false);
        isActive = false;
        //Farmer.instance.
    }
}
