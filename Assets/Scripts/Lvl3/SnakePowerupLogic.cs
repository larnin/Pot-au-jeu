using UnityEngine;
using System.Collections;

public class SnakePowerupLogic : MonoBehaviour
{
    private void Awake()
    {
        SnakePowerupPopulatorLogic.powerup = this;
    }

    private void OnDestroy()
    {
        SnakePowerupPopulatorLogic.powerup = null;
    }
}
