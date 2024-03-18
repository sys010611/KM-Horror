using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrayscaleFilter : MonoBehaviour
{
    private Volume volume;

    private void Start()
    {
        volume = gameObject.GetComponent<Volume>();
    }


    public void setFilter(float currentHP)
    {
        volume.weight = (100-currentHP) / 100;
    }
}
