using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoe : Instrument
{
    private GameObject patchObj;
    private PatchChecking patchCheck;

    private bool IsPatchObjNull() => patchObj == null;

    public override GameObject CreateObj(GameObject obj = null, GameObject prefab = null)
    {
        patchObj = obj;
        if(!IsPatchObjNull())
        {
            patchCheck = obj.GetComponent<Patch>().Checker;
        }

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, InstrumentConstants.MAX_DISTANCE_TO_EARTH) && 
            hit.collider.CompareTag(TagConstants.EARTH))
        {
            if (IsPatchObjNull())
            {
                patchObj = MonoBehaviour.Instantiate(prefab);
                patchCheck = patchObj.GetComponent<Patch>().Checker;
            }

            Vector3 point = Vector3.zero;
            point = new Vector3(patchCheck.IsVerFasten ? patchCheck.FastenPos.x : hit.point.x,
                                0,
                                patchCheck.IsHorFasten ? patchCheck.FastenPos.z : hit.point.z);
            Debug.Log(Vector3.Distance(point, hit.point));
            if(Vector3.Distance(point, hit.point) >= MechConstants.MAX_DISTANCE_FOR_FASTEN_PATCH)
            {
                patchCheck.ResetFastenChecking();
                point = hit.point;
            }
            patchObj.transform.position = new Vector3(point.x,
                                                      0,
                                                      point.z);

            if (patchCheck && patchCheck.IsOnObject)
            {
                patchObj.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.5f);
            }
            else if(patchCheck)
            {
                patchObj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            patchObj = DestroyObj(patchObj);
        }

        return patchObj;
    }

    public override GameObject DestroyObj(GameObject obj = null)
    {
        if (!IsPatchObjNull()) MonoBehaviour.Destroy(patchObj);
        patchObj = null;

        return patchObj;
    }

    public override void Use()
    {
        if (IsPatchObjNull()) return;
        if (patchCheck.IsOnObject) return;

        patchObj.layer = LayerMask.NameToLayer(LayerConstants.DEFAULT);
        patchObj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        patchObj.GetComponent<Patch>().DestroyChecker();
        MonoBehaviour.Instantiate(patchObj);
        patchCheck = null;
        patchObj = null;
    }
}
