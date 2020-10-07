using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class testing : MonoBehaviour
{
    public GameObject game;
    private void Awake()
    {
        game = Instantiate(game);
        game.SetActive(false);
    }
    private void Start()
    {
        game.SetActive(true);
    }
}
