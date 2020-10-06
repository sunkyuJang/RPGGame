using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateViewerHandler
{
    void RefreshState();
    void ShowObj(bool souldShow);
    GameObject GetGameObject();
}
