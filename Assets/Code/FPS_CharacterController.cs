using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FPS_CharacterController : RestartableObject
{
    [HideInInspector] public float yaw;
    [HideInInspector] public float pitch;

    [Header("MOVEMENT OPTIONS")]
    public float min_Pitch = -35f;
    public float max_Pitch = 105f;

    public float yawRotationSpeed = 1f;
    public float pitchRotationSpeed = 1f;

    public Transform pitchController;
    [HideInInspector] public CharacterController characterController;

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

    public KeyCode getObjectKey = KeyCode.F;

    private Transform attachObjectTransform;

    public float maxDistanceToAttachObject = 25f;
    public LayerMask attachLayerMask;

    private float attachingObjectCurrentTime = 0.0f;
    private float attachObjectTime = 0.0f;
    private bool attachingObject;
    private bool attachedObject;
    private GameObject objectAttached;

    public float throwAttachObjectForce = 5f;
    public int maxHp;
    private int currentHp;

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
        currentHp = maxHp;
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && !attachedObject && !attachingObject)
        {
            ShootPortal(bluePortal);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1) && !attachedObject && !attachingObject)
        {
            ShootPortal(orangePortal);
        }

        if (Input.GetKeyDown(getObjectKey) && attachedObject == null)
        {
            GetObject();
        }

        if (attachedObject != null)
        {
            UpdateAttachedObject();
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

            if (!validPos)
                whatPortal.gameObject.SetActive(false);
        }
    }

    private void GetObject()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, maxDistanceToAttachObject, attachLayerMask))
        {
            if (raycastHit.collider.tag == "Companion")
            {
                AttachObject(raycastHit.collider);
            }
        }
    }
    public void LoseHeal(int incomingDamage)
    {
        currentHp -= incomingDamage;
       
    }
    private void AttachObject(Collider collider)
    {
        attachingObject = true;
        attachedObject = collider.gameObject;
        collider.enabled = false;
        collider.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void UpdateAttachedObject()
    {
        attachingObjectCurrentTime += Time.deltaTime;

        if (attachingObject)
        {
            float pct = Mathf.Min(attachingObjectCurrentTime / attachObjectTime, 1f);

            objectAttached.transform.position = Vector3.Lerp(objectAttached.transform.position, attachObjectTransform.position, pct);
            objectAttached.transform.rotation = Quaternion.Lerp(objectAttached.transform.rotation, attachObjectTransform.rotation, pct);


            if (pct == 1.0f)
            {
                attachingObject = false;
                attachedObject = true;
            }
        }
        else if (attachedObject)
        {
            if (Input.GetKeyDown(getObjectKey))
            {
                ThrowAttachObject(0.0f);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                ThrowAttachObject(throwAttachObjectForce);
            }
        }
    }

    private void ThrowAttachObject(float force)
    {
        objectAttached.GetComponent<Collider>().enabled = false;
        Rigidbody rigidbody = objectAttached.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(attachObjectTransform.up * force);

        attachedObject = false;
        objectAttached = null;
        attachingObjectCurrentTime = 0.0f;
    }
}

