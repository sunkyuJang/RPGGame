using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitBoxScript : MonoBehaviour
{
    private void Awake()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        List<ParticleSystem> psChildren = transform.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>();
        foreach(ParticleSystem nowPs in psChildren)
        {
            nowPs.loop = ps.loop;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
