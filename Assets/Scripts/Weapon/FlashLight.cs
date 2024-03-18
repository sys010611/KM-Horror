using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public GameObject Light;

    private void OnEnable()
    {
        Light.SetActive(true);
    }

    private void OnDisable()
    {
        try
        {
            Light.SetActive(false);
        }
        catch (MissingReferenceException e)
        {
            return;
        }
    }
}
