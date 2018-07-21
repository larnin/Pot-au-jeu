using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ExplosionLogic : MonoBehaviour
{
    [SerializeField] float m_lifeTime;

    private void Awake()
    {
        Destroy(gameObject, m_lifeTime);
    }
}