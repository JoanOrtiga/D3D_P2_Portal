using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    [Header("Health")]
    public Text health;
    private string healthText;


    [Header("Portal")]
    public Sprite[] portals;
    public Image portalImage;
    bool placedOrange;
    bool placedBlue;

    void Start()
    {
        healthText = health.text;
    }

    public void UpdateHealth(int newHealth)
    {
        health.text = healthText + newHealth;
    }

    public void UpdatePortals(int portal, bool placed)
    {
        if(portal == 0)
        {
            placedBlue = placed;
        }
        else if (portal == 1)
        {
            placedOrange = placed;
        }

        if (!placedBlue && !placedOrange)
        {
            portalImage.sprite = portals[0];
        }
        else if (!placedBlue && placedOrange)
        {
            portalImage.sprite = portals[1];
        }
        else if (placedBlue && !placedOrange)
        {
            portalImage.sprite = portals[2];
        }
        else if (placedBlue && placedOrange)
        {
            portalImage.sprite = portals[3];
        }
    }
}
