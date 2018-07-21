using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileLogic : MonoBehaviour
{
    public static Matrix<bool> tiles = new Matrix<bool>(false);

    private void Awake()
    {
        var pos = transform.position;
        tiles.set(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), true);
    }

    private void OnDestroy()
    {
        var pos = transform.position;
        tiles.reset(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }
}