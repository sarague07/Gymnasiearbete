using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Refrences")]
    private CharacterController controller;
    [SerializeField] private Transform MainCamera;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float sprintTransit = 5f;
    [SerializeField] private float turningSpeed = 2f;
    [SerializeField] private float gravity = 9.01f;
    [SerializeField] private float jumpHeight = 2f;

    private float verticalVelocity;
    private float speed;

    [Header("Input")]
    private float moveInput;
    private float turnInput;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Scene / Respawn")]
    [SerializeField] private float respawnDelay = 0f;

    private Vector3 initialSpawnPosition;
    private Quaternion initialSpawnRotation;
    private bool isDead;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider == null) return;

        if (hit.collider.CompareTag("Killplane"))
        {
            Die();
        }
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        initialSpawnPosition = transform.position;
        initialSpawnRotation = transform.rotation;

        RespawnManager.SetCheckpoint(initialSpawnPosition);

        if (currentHealth <= 0f)
            currentHealth = maxHealth;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (isDead) return;

        CheckEscapeKey();
        InputManagement();
        Movement();
    }

    private void Movement()
    {
        GroundMovement();
        Turn();
    }

    private void GroundMovement()
    {
        Vector3 move = new Vector3(turnInput, 0, moveInput);
        move = MainCamera.transform.TransformDirection(move);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = Mathf.Lerp(speed, sprintSpeed, sprintTransit * Time.deltaTime);
        }
        else
        {
            speed = Mathf.Lerp(speed, walkSpeed, sprintTransit * Time.deltaTime);
        }

        move *= speed;

        move.y = VerticalForceCalculation();


        controller.Move(move * Time.deltaTime);
    }

    private void Turn()
    {
        if (Mathf.Abs(turnInput) > 0 || Mathf.Abs(moveInput) > 0)
        {
            Vector3 currentLookDirection = MainCamera.forward;
            currentLookDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(currentLookDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turningSpeed);
        }
    }

    private float VerticalForceCalculation()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        return verticalVelocity;

    }

    private void InputManagement()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }
    private void CheckEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Screen.fullScreen)
                Screen.fullScreen = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log("Escape pressed: exited full screen and locked cursor.");
        }
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"Player took {amount} damage. Current health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    public void ApplyDamage(float amount)
    {
        TakeDamage(amount);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        Debug.Log($"Player healed {amount}. Current health: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died. Starting respawn sequence.");

        StartCoroutine(HandleRespawn(respawnDelay));
    }

    private IEnumerator HandleRespawn(float delay)
    {
        if (controller != null)
            controller.enabled = false;
        enabled = false;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        else
            yield return null;

        RespawnManager.Respawn(gameObject, initialSpawnPosition);

        currentHealth = maxHealth;
        verticalVelocity = 0f;

        transform.rotation = initialSpawnRotation;

        if (controller != null)
            controller.enabled = true;
        enabled = true;

        isDead = false;

        Debug.Log("Player respawned.");
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}