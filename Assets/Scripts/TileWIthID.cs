using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileWIthID : Tile
{
    [SerializeField] int m_id;
    public int id { get { return m_id; } }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Tile with ID")]
    public static void CreateAnimatedTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save ID Tile", "New ID Tile", "asset", "Save ID Tile", "Assets");
        if (path == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TileWIthID>(), path);
    }
#endif
}
