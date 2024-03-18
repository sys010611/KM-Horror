using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowTooltipEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(SceneManager.GetActiveScene().name == "PlayScene_1F_WayOut" && other.CompareTag("Player"))
        {
            UIManager.Instance.SetToolTip("Press SHIFT to run \n\nRunning makes louder sound");
        }

        if(SceneManager.GetActiveScene().name == "PlayScene_1F_Room V1 (2)" && other.CompareTag("Player"))
        {
            UIManager.Instance.SetToolTip("Hold CTRL to crouch \n\nCrouching reduces your sound");
        }

        Destroy(gameObject);
    }
}
