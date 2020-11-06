using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageArea : MonoBehaviour
{
    public Transform responeZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            var character = other.GetComponent<Character>();
            character.IsinField = false;
            character.LastSafeZone.Clear();
            character.LastSafeZone.Add(SceneManager.GetActiveScene().name, responeZone.position);
            other.GetComponent<Character>().IsinField = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
            other.GetComponent<Character>().IsinField = true;
    }
}
