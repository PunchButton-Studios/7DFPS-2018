using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CustomAssetTool : Editor
{
    [MenuItem("Assets/Create/Player Base Object")]
    public static void CreateBaseObject() => CreateAsset<BaseObject>();
    [MenuItem("Assets/Create/Worldgen Room")]
    public static void CreateWorldgenRoom() => CreateAsset<Room>();

    public static void CreateAsset<T>() where T:ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrWhiteSpace(path))
            path = "Assets";
        else if (Path.GetExtension(path) != string.Empty)
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), string.Empty);

        path = AssetDatabase.GenerateUniqueAssetPath($"{path}/{typeof(T).ToString()}.asset");
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}