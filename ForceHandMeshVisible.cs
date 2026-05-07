using UnityEngine;

public class ForceHandMeshVisible : MonoBehaviour
{
    SkinnedMeshRenderer smr;

    void Start()
    {
        smr = GetComponent<SkinnedMeshRenderer>();
    }

    void LateUpdate()
    {
        if (smr != null && !smr.enabled)
            smr.enabled = true;
    }
}
