using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Refractor
{
    [Header("PORTAL LOOKING")]
    public Portal mirrorPortal;
    public Transform mirrorPortalTransform;
    public Camera portalCamera;
    public FPS_CharacterController player;

    public float cameraOffset = 0.1f;

    public GameObject portalLooking;

    [Header("VALID POSITIONS")]
    public List<Transform> validPoints;
    public float minDistanceToValidPoint = 0.65f;
    public float maxDistanceToValidPoint = 1.2f;
    public float minDot = 0.9f;

    //  [Header("TELEPORT")]
    [HideInInspector] public bool enteredPortal = false;
    [HideInInspector] public bool leftPortal = false;

    private GameObject objectTeleported;
    private bool teleportObjectAvaiable = true;

    protected override void Start()
    {
        base.Start();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPS_CharacterController>();
        }

        gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (!mirrorPortal.gameObject.activeSelf && portalLooking.activeSelf)
        {
            portalLooking.SetActive(false);
        }
        else if (mirrorPortal.gameObject.activeSelf && !portalLooking.activeSelf)
        {
            portalLooking.SetActive(true);
        }

        Vector3 localPosition = mirrorPortal.mirrorPortalTransform.InverseTransformPoint(player.mainCamera.transform.position);
        portalCamera.transform.position = transform.TransformPoint(localPosition);
        Vector3 localDirection = mirrorPortal.mirrorPortalTransform.InverseTransformDirection(player.mainCamera.transform.forward);
        portalCamera.transform.forward = transform.TransformDirection(localDirection);

        float distanceToPortal = Vector3.Distance(portalCamera.transform.position, transform.position);
        portalCamera.nearClipPlane = distanceToPortal + cameraOffset;

        if (enteredPortal && !leftPortal)
        {
            TeleportPlayer();
        }
    }


    public bool IsValidPosition(Vector3 position, Vector3 normal)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(normal);

        for (int i = 0; i < validPoints.Count; i++)
        {
            Vector3 direction = validPoints[i].position - player.mainCamera.transform.position;
            direction.Normalize();

            Ray ray = new Ray(player.mainCamera.transform.position, direction);
            RaycastHit raycasthit;

            if (Physics.Raycast(ray, out raycasthit, player.shootDistance, player.shootLayerMask.value))
            {
                if (raycasthit.collider.CompareTag("Drawable"))
                {
                    float distanceToHitPosition = Vector3.Distance(position, raycasthit.point);
                    if (distanceToHitPosition >= minDistanceToValidPoint && distanceToHitPosition <= maxDistanceToValidPoint)
                    {
                        float dot = Vector3.Dot(raycasthit.normal, normal);
                        if (dot < minDot)
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }


        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !enteredPortal && !leftPortal)
        {
            if (mirrorPortal.gameObject.activeSelf)
            {
                enteredPortal = true;
                mirrorPortal.enteredPortal = false;
                mirrorPortal.leftPortal = true;
                leftPortal = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !enteredPortal)
        {
            enteredPortal = false;
            leftPortal = false;
        }
    }

    private void TeleportPlayer()
    {
        float pitch = player.pitchController.rotation.eulerAngles.x;

        player.characterController.enabled = false;
        Vector3 localPosition = transform.InverseTransformPoint(transform.position);       //Offset to adjust player height;
        player.transform.position = mirrorPortal.transform.TransformPoint(localPosition) - new Vector3(0, 1f, 0);
        player.characterController.enabled = true;

        Vector3 localDirection = transform.InverseTransformDirection(-player.transform.forward);
        player.transform.forward = mirrorPortal.transform.TransformDirection(localDirection);
        player.yaw = player.transform.rotation.eulerAngles.y;
        player.pitch = pitch;

        enteredPortal = false;
    }

    public bool Reflection(GameObject _reflectorEmitter, Vector3 position, Vector3 direction)
    {
        if (reflectionEmitter != null && _reflectorEmitter != reflectionEmitter)
        {
            return false;
        }

        reflectionEmitter = _reflectorEmitter;


        Vector2 localPosition = mirrorPortalTransform.InverseTransformPoint(position);
        mirrorPortal.laser.transform.localPosition = localPosition;

        Vector3 localDirection = mirrorPortalTransform.InverseTransformDirection(direction);
        mirrorPortal.laser.transform.localRotation = Quaternion.LookRotation(localDirection);

        mirrorPortal.laser.gameObject.SetActive(true);

        base.UpdateLaserDistance();

        return true;
    }

    public override void StopReflection()
    {
        mirrorPortal.laser.gameObject.SetActive(false);

        if (refractionCubeHit != null)
        {
            refractionCubeHit.StopReflection();
        }

        refractionCubeHit = null;
    }
}
