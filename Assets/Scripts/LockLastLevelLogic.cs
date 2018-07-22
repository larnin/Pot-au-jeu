using UnityEngine;
using System.Collections;

public class LockLastLevelLogic : MonoBehaviour
{
    [SerializeField] int m_unlockCount = 2;
    
    void Start()
    {
        if(States.instance.finishedSceneCount() < m_unlockCount)
        {
            GetComponent<SelectableLogic>().enabled = false;
        }
        else
        {
            transform.Find("black").gameObject.SetActive(false);
        }
    }
}
