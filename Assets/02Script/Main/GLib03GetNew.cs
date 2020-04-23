using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GLip
{
    class Create : StaticManager
    {
        public static T GetNew<T>(Transform parent) { return Instantiate(Resources.Load<GameObject>(typeof(T).Name), parent).GetComponent<T>(); }
        public static T GetNew<T>(string path, Transform parent) { return Instantiate(Resources.Load<GameObject>(path), parent).GetComponent<T>(); }
        public static T GetNew<T>() { return Instantiate(Resources.Load<GameObject>(typeof(T).Name), canvasTrasform).GetComponent<T>(); }
    }

    class Find
    {
        public static T GetFind<T>() { return GameObject.Find(typeof(T).Name).GetComponent<T>(); }
    }
}
