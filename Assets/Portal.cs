using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal mirrorPortal;
    public Transform mirrorPortalTransform;
    public Camera portalCamera;
    //public FPSController player;

    public float cameraOffset = 0.1f;

    public List<Transform> validPoints;

    private void Update()
    {
        /*Vector3 localPosition = mirrorPortalTransform.InverseTransformPoint(player.camera.transform.position);
        portalCamera.transform.position = transform.TransformPoint(localPosition);
        Vector3 localDirection = mirrorPortal.mirrorPortalTransform.InverseTransformDirection(player.camera.transform.forward);
        portalCamera.transform.forward = transform.TransformDirection(localDirection);
        */
        float distanceToPortal = Vector3.Distance(portalCamera.transform.position, transform.position);
        portalCamera.nearClipPlane = distanceToPortal + cameraOffset;

    }

    public bool IsValidPosition(Vector3 position, Vector3 normal)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(normal);

        for (int i = 0; i < validPoints.Count; i++)
        {
            Vector3 direction = validPoints[i].position - player.camera.transform.position;
            direction.Normalize();
            Ray ray = new Ray(PlayerPrefs.camera.trans.position, direction);
            RaycastHit raycasthit;
            if (Physics.Raycast(ray, out raycasthit, maxDistance, PlayerPrefs.shootlayermask.value))
            {
                if (raycasthit.collider.CompareTag("Drawable"))
                {
                    float distanceToHitPosition = Vector3.Distance(position, raycasthit.point);
                    if (distanceToHitPosition >= minDistanceToValidPoint && distanceToHitPosition <= maxDistanceToValidPoint)
                    {
                        float dot = Vector3.Dot(raycasthit.normal, normal);
                        if(dot <= minDot)
                        {
                            return false;
                        }
                    }
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
}
