using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refractor : MonoBehaviour
{
    [Header("LASER")]
    public LineRenderer laser;
    public LayerMask laserLayerMask;
    public float maxDistance = 250.0f;

    public Refractor refractionCubeHit;
    public GameObject reflectionEmitter;

    public int damage = 250;

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
       
        RaycastHit raycasthit;
        Ray ray = new Ray(laser.transform.position, laser.transform.forward);
        float distance = maxDistance;

        //refractionCubeHit = null;

        if (Physics.Raycast(ray, out raycasthit, maxDistance, laserLayerMask))
        {
            distance = raycasthit.distance;

            if (raycasthit.collider.CompareTag("RefractionCube"))
            {
                refractionCubeHit = raycasthit.collider.GetComponent<Refractor>();
                refractionCubeHit.Reflection(gameObject);
            }
            else if (raycasthit.collider.CompareTag("PortalRefractor"))
            {
                Portal portal = raycasthit.collider.GetComponent<ReferenceToPortal>().portal;
                portal.Reflection(gameObject, raycasthit.point, ray.direction);

                refractionCubeHit = portal;
            }
            else if (raycasthit.collider.CompareTag("Player"))
            {
                raycasthit.collider.GetComponent<FPS_CharacterController>().LoseHeal(damage);
            }
            else if (raycasthit.collider.CompareTag("Turret"))
            {
                Destroy(raycasthit.collider.gameObject);
            }
            else if (refractionCube != null)
            {
                refractionCubeHit = null;
                refractionCube.StopReflection();
            }

            laser.SetPosition(1, new Vector3(0.0f, 0.0f, distance));
            
         /*   if (refractionCube != refractionCubeHit)
            {
                refractionCube.StopReflection();
            }*/
        }
        else if(refractionCube != null)
        {
            print("XD");
            refractionCube.StopReflection();
        }
    }

    public virtual void StopReflection()
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
