using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapInfos : MonoBehaviour
{
    public static TilemapInfos instance;

    [Serializable]
    public class TileID
    {
        public int id;
        public Tile tile;
    }
    
    [SerializeField] int m_defaultID = 0;
    Tilemap m_tilemap;

    void Awake()
    {
        m_tilemap = GetComponent<Tilemap>();
        m_tilemap.CompressBounds();
        instance = this;
    }
    
    public int tileID(int x, int y)
    {
        var tile = m_tilemap.GetTile<TileWIthID>(new Vector3Int(x, y, 0));
        if (tile != null)
            return tile.id;
        return m_defaultID;
    }
}
