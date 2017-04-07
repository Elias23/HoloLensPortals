using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCameraManager : MonoBehaviour
{

    public GameObject portal1;
    public GameObject portal2;
    Transform portal1Camera;
    Transform portal2Camera;

    Vector3 portal1RelativePos;
    Vector3 portal2RelativePos;
    Vector3 portal1RelativeDir;
    Vector3 portal2RelativeDir;

    // Use this for initialization
    void Start()
    {

        portal1Camera = portal1.transform.GetChild(0);
        portal2Camera = portal2.transform.GetChild(0);

        portal1RelativePos = portal1.transform.InverseTransformPoint(this.transform.position);
        portal2RelativePos = portal2.transform.InverseTransformPoint(this.transform.position);

        portal1Camera.localPosition = new Vector3(-1 * portal2RelativePos.x, portal2RelativePos.y, -1 * portal2RelativePos.z);
        portal2Camera.localPosition = new Vector3(-1 * portal1RelativePos.x, portal1RelativePos.y, -1 * portal1RelativePos.z);

        portal1Camera.localRotation = Quaternion.AngleAxis(180.0f, new Vector3(0, 1, 0)) * Quaternion.Inverse(portal2.transform.rotation) * (transform.rotation);
        portal2Camera.localRotation = Quaternion.AngleAxis(180.0f, new Vector3(0, 1, 0)) * Quaternion.Inverse(portal1.transform.rotation) * (transform.rotation);


    }

    // Update is called once per frame
    void Update()
    {
        portal1RelativePos = portal1.transform.InverseTransformPoint(this.transform.position);
        portal2RelativePos = portal2.transform.InverseTransformPoint(this.transform.position);

        portal1Camera.localPosition = new Vector3(-1 * portal2RelativePos.x, portal2RelativePos.y, -1 * portal2RelativePos.z);
        portal2Camera.localPosition = new Vector3(-1 * portal1RelativePos.x, portal1RelativePos.y, -1 * portal1RelativePos.z);

        portal1Camera.localRotation = Quaternion.AngleAxis(180.0f, new Vector3(0, 1, 0)) * Quaternion.Inverse(portal2.transform.rotation)*(transform.rotation) ;
        portal2Camera.localRotation = Quaternion.AngleAxis(180.0f, new Vector3(0, 1, 0)) * Quaternion.Inverse(portal1.transform.rotation)*(transform.rotation) ;



    }
}
