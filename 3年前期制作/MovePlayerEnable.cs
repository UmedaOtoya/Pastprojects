using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerEnable : MonoBehaviour, IMoveObjectEnable
{
    /// <summary>
    /// 動かせるか
    /// </summary>
    public bool IsMove => true;
}
