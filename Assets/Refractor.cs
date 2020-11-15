using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refractor : MonoBehaviour
{
    [Header("LASER")]
    public LineRenderer laser;
    public LayerMask laserLayerMask;
    public float maxDistance = 250.0f;

    Refractor refractionCubeHit;
    public GameObject reflectionEmitter;

    protected virtual void Start()
    {
        laser.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (laser.gameObject.activeSelf)
        {
            UpdateLaserDistance();
        }
    }

    public virtual bool Reflection(GameObject _reflectionEmitter)
    {
        if (reflectionEmitter != null && _reflectionEmitter != reflectionEmitter)
            return false;

        reflectionEmitter = _reflectionEmitter;
        laser.gameObject.SetActive(true);
        UpdateLaserDistance();

        return true;
    }

    protected void UpdateLaserDistance()
    {
        Refractor refractionCube = refractionCubeHit;
        refractionCubeHit = null;


        RaycastHit raycasthit;
        Ray ray = new Ray(laser.transform.position, laser.transform.forward);
        float distance = maxDistance;

        if (Physics.Raycast(ray, out raycasthit, maxDistance, laserLayerMask))
        {
            distance = raycasthit.distance;

            if (raycasthit.collider.CompareTag("RefractionCube"))
            {
                refractionCubeHit = raycasthit.collider.GetComponent<Refractor>();

                if (refractionCubeHit.Reflection(gameObject))
                    refractionCubeHit = null;
            }
            else if (raycasthit.collider.CompareTag("PortalRefractor"))
            {
                Portal portal = raycasthit.collider.GetComponent<ReferenceToPortal>().portal;
                portal.Reflection(raycasthit.point, ray.direction);
            }

            laser.SetPosition(1, new Vector3(0.0f, 0.0f, distance));
            if (refractionCube != refractionCubeHit)
            {
                refractionCube.StopReflection();
            }
        }
    }

    public void StopReflection()
    {
        laser.gameObject.SetActive(false);
        if (refractionCubeHit != null)
        {
            refractionCubeHit.StopReflection();
        }

        refractionCubeHit = null;
    }

    protected virtual void OnDisable()
    {
        laser.gameObject.SetActive(false);
    }
}
