using System;
using UnityEngine;
using UnityEditor;

//設定したフィールドのサイズに合わせて、
//ループするフィールドに配置必須のオブジェクトを
//生成するエディター拡張
public class LoopAreaCreator : EditorWindow
{
    [SerializeField, Tooltip("親オブジェクト")]
    private GameObject parent = default;
    [SerializeField, Tooltip("ループ範囲")]
    private LoopRange loopRange = default;
    [SerializeField, Tooltip("フィールド")]
    private GameObject field = default;
    [SerializeField, Tooltip("ループカメラ")]
    private GameObject loopCamera = default;
    [SerializeField, Tooltip("parentの子オブジェクトを削除するか")]
    private bool isRegenerate = false;

    [Tooltip("loopRange.Bounds.size.x")]private float rangeX = 0.0f;
    [Tooltip("loopRange.Bounds.size.y")] private float rangeY = 0.0f;
    [Tooltip("loopRange.Bounds.size.z")] private float rangeZ = 0.0f;

    private readonly int stageLayer = 8;

    /// <summary>
    /// ウィンドウのGUI
    /// </summary>
    private void OnGUI()
    {
       EditorGUIUtility.labelWidth = 400;
        //入力欄生成
        parent = EditorGUILayout.ObjectField("ステージの親になるオブジェクトを入れてください", parent, typeof(GameObject), true) as GameObject;
        loopRange = EditorGUILayout.ObjectField("スクリタブルオブジェクトのLoopRangeを入れてください", loopRange, typeof(LoopRange), false) as LoopRange;
        field = EditorGUILayout.ObjectField("足場となるフィールドのプレファブを入れてください", field, typeof(GameObject), false) as GameObject;
        loopCamera = EditorGUILayout.ObjectField("ループに見せるためのカメラグループのプレファブを入れてください", loopCamera, typeof(GameObject), false) as GameObject;
        GUILayout.Label("設定した親オブジェクトの子を削除します", EditorStyles.boldLabel);
        isRegenerate = EditorGUILayout.Toggle(isRegenerate);

        if (GUILayout.Button("生成する")) Generate();
    }

    /// <summary>
    /// エリアの生成
    /// </summary>
    private void Generate()
    {
        // 設定した親オブジェクトの子をすべて削除する
        if (isRegenerate) { Delete(); }

        rangeX = loopRange.Bounds.size.x;
        rangeY = loopRange.Bounds.size.y;
        rangeZ = loopRange.Bounds.size.z;

        //足場となるフィールドの実体化
        var stage = Instantiate(field, Vector3.zero, Quaternion.identity, parent.transform);
        stage.layer = stageLayer;

        //ループカメラグループの生成
        CameraGenerate(loopCamera.name + "Up", rangeY, true);
        CameraGenerate(loopCamera.name + "Middle", 0.0f, false);
        CameraGenerate(loopCamera.name + "Down", -rangeY, true);
        BackGroundGenerate();

        //6方面にトリガーコリジョンを生成
        GameObject walls = new GameObject("WarpWalls");
        walls.transform.parent = parent.transform;
        for(int i = 0;  i < 6; i++)
        {
            WarpWallGenerate(walls, (Drection)Enum.ToObject(typeof(Drection), i));
        }
    }

    /// <summary>
    /// 設定した親オブジェクトの子をすべて削除する
    /// </summary>
    private void Delete()
    {
        while (0 < parent.transform.childCount)
        {
            DestroyImmediate(parent.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// ループカメラを生成する
    /// </summary>
    /// <param name="name"></param>
    /// <param name="hight"></param>
    private void CameraGenerate(string name, float hight, bool isNeedCenter)
    {
        //実体化
        var cameras = Instantiate(loopCamera, Vector3.zero, Quaternion.identity, parent.transform);
        cameras.name = name;
        cameras.GetComponent<MebiusCameraController>().offset = new Vector3(0.0f, hight, 0.0f);

        if (isNeedCenter)
        {
            GameObject center = new GameObject("CameraCenter");
            center.transform.parent = cameras.transform;
            var centerCamera = center.AddComponent<Camera>();
            centerCamera.clearFlags = CameraClearFlags.Nothing;
            centerCamera.depth = -10;
        }

        //全てのカメラ位置とファークリップをループ範囲の大きさに合わせる
        foreach (Transform child in cameras.transform)
        {
            //カメラ位置
            child.position = new Vector3(child.position.x * rangeX, 0.0f, child.position.z * rangeZ);
            //ファークリップを短い方に合わせる
            var camera = child.GetComponent<Camera>();
            camera.farClipPlane = rangeX >= rangeZ ? rangeX : rangeZ;
        }
    }

    /// <summary>
    /// 背景を表示するためのカメラ生成
    /// </summary>
    private void BackGroundGenerate()
    {
        GameObject backGround = new GameObject("BGCamera");
        backGround.transform.parent = parent.transform;
        var bgCamera = backGround.AddComponent<Camera>();
        bgCamera.clearFlags = CameraClearFlags.SolidColor;
        bgCamera.backgroundColor = Color.black;
        bgCamera.cullingMask = ~-1; //Nosing
        bgCamera.depth = -100;
    }

    private void WarpWallGenerate(GameObject parent, Drection drection)
    {
        float thickness = 5.0f;
        GameObject wall = new GameObject();
        wall.transform.parent = parent.transform;
        BoxCollider collider = wall.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        TestWarp warp = wall.AddComponent<TestWarp>();
        warp.loopRange = loopRange;

        switch (drection)
        {
            case Drection.forword:
                wall.name = "ForwardWall";
                warp.drection = Drection.forword;
                collider.size = new Vector3(rangeX, rangeY, thickness);
                collider.center = new Vector3(0.0f, 0.0f, loopRange.Bounds.max.z + thickness / 2);
                break;
             case Drection.back:
                wall.name = "backWall";
                warp.drection = Drection.back;
                collider.size = new Vector3(rangeX, rangeY, thickness);
                collider.center = new Vector3(0.0f, 0.0f, loopRange.Bounds.min.z - thickness / 2);
                break;
            case Drection.right:
                wall.name = "RightWall";
                warp.drection = Drection.right;
                collider.size = new Vector3(thickness, rangeY,rangeZ);
                collider.center = new Vector3(loopRange.Bounds.max.x + thickness / 2, 0.0f, 0.0f);
                break;
            case Drection.left:
                wall.name = "LeftWall";
                warp.drection = Drection.left;
                collider.size = new Vector3(thickness, rangeY, rangeZ);
                collider.center = new Vector3(loopRange.Bounds.min.x - thickness / 2, 0.0f, 0.0f);
                break;
            case Drection.top:
                wall.name = "TopWall";
                warp.drection = Drection.top;
                collider.size = new Vector3(rangeX, thickness, rangeZ);
                collider.center = new Vector3(0.0f, loopRange.Bounds.max.y + thickness / 2, 0.0f);
                break;
            case Drection.bottom:
                wall.name = "bottomWall";
                warp.drection = Drection.bottom;
                collider.size = new Vector3(rangeX, thickness,rangeZ);
                collider.center = new Vector3(0.0f, loopRange.Bounds.min.y - thickness / 2, 0.0f);
                break;
            default:
                return;
        }
    }

    /// <summary>
    /// unityのメニュー欄に追加
    /// </summary>
    [MenuItem("Editor/CreateLoopArea")]
    private static void Init()
    {
        //Editorに項目を追加
        GetWindow<LoopAreaCreator>("CreateLoopArea");
    }
}
