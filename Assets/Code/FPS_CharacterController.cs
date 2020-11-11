using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FPS_CharacterController : RestartableObject
{
    private float yaw;
    private float pitch;

    [Header("MOVEMENT OPTIONS")]
    public float min_Pitch = -35f;
    public float max_Pitch = 105f;

    public float yawRotationSpeed = 1f;
    public float pitchRotationSpeed = 1f;

    public Transform pitchController;
    private CharacterController characterController;

    public float movementSpeed = 10f;
    public float movementSpeedSprinting = 14f;
    private float currentMovementSpeed = 0f;
    private float verticalSpeed;
    public float jumpSpeed = 2.5f;

    private bool onGround = true;
    private CollisionFlags collisionFlags;

    [Header("CONTROLS")]
    public KeyCode leftMovement = KeyCode.A;
    public KeyCode rightMovement = KeyCode.D;
    public KeyCode frontMovement = KeyCode.W;
    public KeyCode backMovement = KeyCode.S;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("PORTALS")]
    public Portal bluePortal;
    public Portal orangePortal;

    public LayerMask shootLayerMask;
    public float shootDistance;

    [Header("References")]
    public Camera mainCamera;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        mainCamera = Camera.main;
    }

    protected override void Start()
    {
        base.Start();

        Cursor.lockState = CursorLockMode.Locked;

        yaw = transform.rotation.eulerAngles.y;
        pitch = pitchController.localRotation.eulerAngles.x;

        verticalSpeed = 0;
        currentMovementSpeed = movementSpeed;
    }

    private void Update()
    {
        CameraUpdate();

        Vector3 l_Movement = Vector3.zero;
        Vector3 l_Right = transform.right;
        Vector3 l_Forward = transform.forward;

        if (Input.GetKey(rightMovement))
            l_Movement += l_Right;
        if (Input.GetKey(leftMovement))
            l_Movement += -l_Right;
        if (Input.GetKey(frontMovement))
            l_Movement += l_Forward;
        if (Input.GetKey(backMovement))
            l_Movement += -l_Forward;

        if (Input.GetKeyDown(jumpKey) && onGround)
        {
            verticalSpeed = jumpSpeed;
        }

        if (Input.GetKey(sprintKey))
        {
            currentMovementSpeed = movementSpeedSprinting;
        }
        else
        {
            currentMovementSpeed = movementSpeed;
        }

        l_Movement.Normalize();

        verticalSpeed = verticalSpeed + Physics.gravity.y * Time.deltaTime;

        l_Movement = l_Movement * currentMovementSpeed * Time.deltaTime;
        l_Movement.y = verticalSpeed * Time.deltaTime;

        collisionFlags = characterController.Move(l_Movement);

        GravityUpdate();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootPortal(bluePortal);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ShootPortal(orangePortal);
        }
    }

    private void CameraUpdate()
    {
        float l_MouseAxisX = Input.GetAxis("Mouse X");
        float l_MouseAxisY = Input.GetAxis("Mouse Y");

        yaw = yaw + l_MouseAxisX * yawRotationSpeed * Time.deltaTime;
        pitch = pitch + l_MouseAxisY * pitchRotationSpeed * Time.deltaTime * -1f; // *-1 to invert mouse.

        pitch = Mathf.Clamp(pitch, min_Pitch, max_Pitch);

        transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
        pitchController.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
    }

    private void GravityUpdate()
    {
        onGround = (collisionFlags & CollisionFlags.CollidedBelow) != 0;
        if (onGround || ((collisionFlags & CollisionFlags.CollidedAbove) != 0 && verticalSpeed > 0.0f))
        {
            verticalSpeed = 0.0f;
        }
    }


    private void ShootPortal(Portal whatPortal)
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit rayCastHit;

        if (Physics.Raycast(ray, out rayCastHit, shootDistance, shootLayerMask.value))
        {

            whatPortal.gameObject.SetActive(true);
            bool validPos = whatPortal.IsValidPosition(rayCastHit.point, rayCastHit.normal);

            print(validPos);
            if (!validPos)
                whatPortal.gameObject.SetActive(false);
        }
    }
}

