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
    public KeyCode Grab = KeyCode.E;

    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;






    public int maxHp;
    private int currentHp;

    [Header("Ataching Objects")]
    public float throwAttachObjectForce = 5f;
    [SerializeField] private Transform attachObjectTransform;
    public float maxDistanceToAttachObject = 25f;
    public LayerMask attachLayerMask;
    private float attachingObjectCurrentTime = 0.0f;
    [SerializeField] private float attachObjectTime = 0.0f;
    private bool attachingObject;
    private bool attachedObject;
    private GameObject objectAttached;

    [Header("PORTALS")]
    public Portal bluePortal;
    public Portal orangePortal;

    public LayerMask shootLayerMask;
    public float shootDistance;

    [SerializeField] private float maxScale;
    [SerializeField] private float minScale;
    public float maxPercentatge = 200;
    public float minPercentatge = 50;
    public float mouseWheelScaleFactor = 40;


    [Header("References")]
    public Camera mainCamera;
    public UIPlayer uiPlayer;
    public GameManager gameManager;

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

        maxScale = maxPercentatge * bluePortal.transform.localScale.x / 100f;
        minScale = minPercentatge * bluePortal.transform.localScale.x / 100f;
    }

    protected void Update()
    {
        if (gameManager.paused)
            return;

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

        if (Input.GetKey(KeyCode.Mouse0) && !attachedObject && !attachingObject)
        {
            ShootPortal(bluePortal, 0);

            PortalScaling(bluePortal);
        }
        else if (Input.GetKey(KeyCode.Mouse1) && !attachedObject && !attachingObject)
        {
            ShootPortal(orangePortal, 1);

            PortalScaling(orangePortal);
        }



        if (Input.GetKeyDown(Grab) && objectAttached == null)
        {
            TryAttachObj();
        }
        if (objectAttached != null)
        {
            UpdateAttachedObject();
        }
    }

    private void PortalScaling(Portal portal)
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        float scale = mouseWheel * Time.deltaTime * mouseWheelScaleFactor;

        float finalScale = portal.transform.localScale.x;

        if (mouseWheel > 0)
        {
            finalScale = Mathf.Min(portal.transform.localScale.x + scale, maxScale);
        }
        else if (mouseWheel < 0)
        {
            finalScale = Mathf.Max(portal.transform.localScale.x + scale, minScale);
        }

        portal.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
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

    private void ShootPortal(Portal whatPortal, int index)
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit rayCastHit;

        if (Physics.Raycast(ray, out rayCastHit, shootDistance, shootLayerMask.value))
        {
            whatPortal.gameObject.SetActive(true);
            bool validPos = whatPortal.IsValidPosition(rayCastHit.point, rayCastHit.normal);




            if (!validPos)
            {
                whatPortal.gameObject.SetActive(false);
                uiPlayer.UpdatePortals(index, false);
            }
            else
            {
                uiPlayer.UpdatePortals(index, true);
            }
        }

    }

    //private void GetObject()
    //{
    //    Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

    //    RaycastHit raycastHit;

    //    if (Physics.Raycast(ray, out raycastHit, maxDistanceToAttachObject, attachLayerMask))
    //    {
    //        if (raycastHit.collider.tag == "Companion")
    //        {
    //            AttachObject(raycastHit.collider);
    //        }
    //    }
    //}
    public void LoseHeal(int incomingDamage)
    {
        currentHp -= incomingDamage;

        currentHp = Mathf.Max(0, currentHp);

        uiPlayer.UpdateHealth(currentHp);

        if (currentHp <= 0)
        {
            gameManager.GameOver();
        }
    }

    private void TryAttachObj()
    {
        Ray l_Ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, maxDistanceToAttachObject, attachLayerMask))
        {
            if (l_RaycastHit.collider.tag == "CompanionCube")
            {
                AttachObject(l_RaycastHit.collider);
            }
            if (l_RaycastHit.collider.tag == "Turret")
            {
                AttachObject(l_RaycastHit.collider);
            }
        }
    }
    void AttachObject(Collider collider)
    {
        attachingObject = true;
        objectAttached = collider.gameObject;
        collider.enabled = false;
        collider.GetComponent<Rigidbody>().isKinematic = true;
        attachingObjectCurrentTime = 0.0f;

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
                objectAttached.transform.SetParent(attachObjectTransform);
            }
        }
        else if (attachedObject)
        {
            if (Input.GetKeyDown(Grab))
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
        objectAttached.GetComponent<Collider>().enabled = true;
        objectAttached.transform.SetParent(null);
        Rigidbody rigidbody = objectAttached.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(attachObjectTransform.forward * force);

        attachedObject = false;
        objectAttached = null;
    }

    protected override void UpdateCheckPoint()
    {
        base.UpdateCheckPoint();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            UpdateCheckPoint();
        }
    }
    public override void RestartObject()
    {
        characterController.enabled = false;
        transform.position = m_InitialPosition;
        transform.rotation = m_InitialRotation;
        characterController.enabled = true;
    }


    private float pushPower = 2.0F;

    //DEBUG
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) { return; }

        if (hit.moveDirection.y < -0.3) { return; }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        body.velocity = pushDir * pushPower;
    }

    
}

