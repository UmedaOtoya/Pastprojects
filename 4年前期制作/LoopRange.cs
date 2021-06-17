using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create LoopRangeData")]
public class LoopRange : ScriptableObject
{
    [SerializeField, Tooltip("ループ範囲")] private Bounds loopRange = default;
    /// <summary>
    /// ループする範囲のサイズ
    /// </summary>
    public Bounds Bounds => loopRange;
}
