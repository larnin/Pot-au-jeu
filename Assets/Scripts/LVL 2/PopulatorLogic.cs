using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using NRand;

public class PopulatorLogic : MonoBehaviour
{
    [SerializeField] RectInt m_bounds;
    [SerializeField] Tilemap m_tilemap;
    [SerializeField] List<TileBase> m_tiles;

    private void Start()
    {
        populate();
    }

    void populate()
    {
        Matrix<int> tiles = new Matrix<int>(-1);

        var rand = new StaticRandomGenerator<DefaultRandomGenerator>();
        var d = new UniformIntDistribution(m_tiles.Count() - 1);

        for (int i = m_bounds.min.x; i <= m_bounds.max.x; i++)
            for (int j = m_bounds.min.y; j < m_bounds.max.y; j++)
                tiles.set(i, j, d.Next(rand));

        for (int i = m_bounds.min.x; i <= m_bounds.max.x; i++)
            for (int j = m_bounds.min.y; j < m_bounds.max.y; j++)
            {
                if (m_tilemap.GetTile(new Vector3Int(i, j, 0)) == null)
                    m_tilemap.SetTile(new Vector3Int(i, j, 0), m_tiles[tiles.get(i, j)]);
            }
    }
}
