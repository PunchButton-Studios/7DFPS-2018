using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSlot : MonoBehaviour
{
    private BaseObject baseObject;
    public string extraData;

    public BaseObject BaseObject
    {
        get
        {
            return baseObject;
        }
        set
        {
            if(baseObject != null)
                Destroy(transform.GetChild(0).gameObject);

            baseObject = value;
            Instantiate(baseObject.prefab, transform);
        }
    }

    public void ApplyExtraData()
    {
        if(BaseObject != null)
        {
            ActivatableBaseObject baseObject = transform.GetChild(0).GetComponent<ActivatableBaseObject>();
            baseObject?.ApplyExtraData(extraData);
        }
    }
}