using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("PORTAL LOOKING")]
    public Portal mirrorPortal;
    public Transform mirrorPortalTransform;
    public Camera portalCamera;
    public FPS_CharacterController player;

    public float cameraOffset = 0.1f;

    [Header("VALID POSITIONS")]
    public List<Transform> validPoints;
    public float minDistanceToValidPoint = 0.65f;
    public float maxDistanceToValidPoint = 1.2f;
    public float minDot = 0.9f;



    private Collider collider;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<FPS_CharacterController>();
        }

        collider = GetComponent<Collider>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        /*Vector3 localPosition = mirrorPortalTransform.InverseTransformPoint(player.mainCamera.transform.position);
        portalCamera.transform.position = transform.TransformPoint(localPosition);
        Vector3 localDirection = mirrorPortal.mirrorPortalTransform.InverseTransformDirection(player.mainCamera.transform.forward);
        portalCamera.transform.forward = transform.TransformDirection(localDirection);
        
        float distanceToPortal = Vector3.Distance(portalCamera.transform.position, transform.position);
        portalCamera.nearClipPlane = distanceToPortal + cameraOffset;
        */

        /*  Vector3 localPosition = mirrorPortal.mirrorPortalTransform.InverseTransformPoint(player.mainCamera.transform.position);
          portalCamera.transform.localPosition = transform.TransformPoint(localPosition);
          Vector3 localDirection = mirrorPortal.mirrorPortalTransform.InverseTransformDirection(player.mainCamera.transform.forward);
          portalCamera.transform.forward = transform.TransformDirection(localDirection);

          float distanceToPortal = Vector3.Distance(portalCamera.transform.position, transform.position);
          portalCamera.nearClipPlane = distanceToPortal + cameraOffset;*/

        Vector3 localPosition = mirrorPortal.mirrorPortalTransform.InverseTransformPoint(player.mainCamera.transform.position);
        portalCamera.transform.position = transform.TransformPoint(localPosition);
        Vector3 localDirection = mirrorPortal.mirrorPortalTransform.InverseTransformDirection(player.mainCamera.transform.forward);
        portalCamera.transform.forward = transform.TransformDirection(localDirection);

        float distanceToPortal = Vector3.Distance(portalCamera.transform.position, transform.position);
        portalCamera.nearClipPlane = distanceToPortal + cameraOffset;
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
        if (other.CompareTag("Player"))
        {
            Teleport();
        }
    }

    private void Teleport()
    {
        player.characterController.enabled = false;

        Vector3 localPosition = transform.InverseTransformPoint(transform.position);
        player.transform.position = mirrorPortal.transform.TransformPoint(localPosition);

        player.characterController.enabled = true;

        Vector3 localDirection = transform.InverseTransformDirection(transform.forward);
        player.transform.forward = mirrorPortal.transform.TransformDirection(localDirection);
        player.yaw = player.transform.rotation.eulerAngles.y;
        player.pitch = player.pitchController.rotation.eulerAngles.x;

        StartCoroutine(DisableTeleport());
    }

    IEnumerator DisableTeleport()
    {
        collider.enabled = false;

        yield return new WaitForSeconds(1f);
        
        collider.enabled = true;
    }
}
