using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destructiblePrefab;

    private Dictionary<string, Vector3> partsPos = new Dictionary<string, Vector3>()
    {

    };
    private Dictionary<string, Quaternion> partsRot = new Dictionary<string, Quaternion>()
    {
    };

    private void Start()
    {

    }
    public void ChangeToDestructible()
    {
        Target target = gameObject.GetComponent<Target>();

        GameObject destructible = Instantiate(destructiblePrefab, transform.position, transform.rotation);


        //force to all parts
        foreach (Transform child in destructible.transform)
        {
            Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
            Vector3 force = Vector3.ClampMagnitude(target.hitDir, 1f) * target.hitForceAmount;

            if (target.isFromExplosion)
            {
                rb.AddExplosionForce(target.hitForceAmount, target.explosionPosition, target.explosionRadius);
            } else
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
            
        }




        //// writing source parts
        //foreach (Transform child in transform)
        //{
        //    if (child.name == "metarig") continue;

        //    Debug.Log(child.name);

        //    Vector3 pos = child.gameObject.GetComponent<SkinnedMeshRenderer>().rootBone.localPosition;
        //    Quaternion rot = child.gameObject.GetComponent<SkinnedMeshRenderer>().rootBone.rotation;
        //    if (pos != null)
        //    {
        //        partsPos.Add(child.name, transform.TransformPoint(pos));
        //        partsRot.Add(child.name, rot);
        //    }

        //}
        //// writing part pos and rot
        //foreach (Transform child in destructible.transform)
        //{
        //    child.position = partsPos[child.name];
        //    child.rotation = partsRot[child.name];

        //    child.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //}
        Destroy(gameObject);


    }


}
