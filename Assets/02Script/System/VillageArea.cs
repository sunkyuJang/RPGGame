using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageArea : MonoBehaviour
{
    public Transform responeZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
            other.GetComponent<Character>().IsinField = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
            other.GetComponent<Character>().IsinField = true;
    }
}
