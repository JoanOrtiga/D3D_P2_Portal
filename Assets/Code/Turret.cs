using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public FPS_CharacterController fps;
    public float rayLength;
    public bool isGrabbed;
    private Rigidbody grabbedObject;
    public GameObject shootingPoint;
    private LineRenderer laser;
    public bool active;
    private int damage = 250;
    public bool broken;
    public LayerMask laserLayerMask;

    private Refractor refractor;

    private void Start()
    {
        broken = true;
        active = true;
        isGrabbed = false;
        laser = GetComponent<LineRenderer>();

        fps = FindObjectOfType<FPS_CharacterController>();
    }

    private void Update()
    {
        if (broken)
        {
            Vector3 fwd = shootingPoint.transform.forward;
            Ray ray = new Ray(shootingPoint.transform.position, fwd);
            RaycastHit rayCastHit;

            Refractor refractorCube = null;

            if (active)
            {
                Debug.DrawRay(shootingPoint.transform.position, fwd * rayLength, Color.green);
                Physics.Raycast(shootingPoint.transform.position, fwd);
                laser.SetPosition(0, shootingPoint.transform.position);

                laser.SetPosition(1, fwd * rayLength + shootingPoint.transform.position);

                if (Physics.Raycast(ray, out rayCastHit, rayLength, laserLayerMask))
                {
                    if (rayCastHit.collider.CompareTag("Player"))
                    {
                        fps.LoseHeal(damage);
                    }
                    else if (rayCastHit.collider.CompareTag("CompanionCube"))
                    {
                        active = false;
                    }
                    else if (rayCastHit.collider.CompareTag("Turret"))
                    {
                        Destroy(rayCastHit.collider.gameObject);
                    }
                    else if (rayCastHit.collider.CompareTag("RefractionCube"))
                    {
                        rayCastHit.collider.GetComponent<Refractor>().Reflection(gameObject);

                        refractorCube = rayCastHit.collider.GetComponent<Refractor>();
                    }
                    else if (rayCastHit.collider.CompareTag("PortalRefractor"))
                    {
                        Portal portal = rayCastHit.collider.GetComponent<ReferenceToPortal>().portal;
                        portal.Reflection(gameObject, rayCastHit.point, ray.direction);
                    }

                    laser.SetPosition(1, rayCastHit.point);
                }


            }

            if (refractorCube != refractor )
            {
                if(refractor != null)
                    refractor.StopReflection();

                refractor = refractorCube;
            }
        }
        else
        {
            gameObject.GetComponent<LineRenderer>().enabled = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Turret"))
        {
            broken = false;
        }
        else if (other.gameObject.CompareTag("CompanionCube"))
        {
            broken = false;
        }
    }

}
