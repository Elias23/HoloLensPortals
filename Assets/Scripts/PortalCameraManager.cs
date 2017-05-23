using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCameraManager : MonoBehaviour
{

    public GameObject portal1;
    public GameObject portal2;
    public float m_ClipPlaneOffset = 0.1f;

    Transform portal1Camera;
    Transform portal2Camera;

    Vector3 portal1RelativePos;
    Vector3 portal2RelativePos;
    Vector3 portal1RelativeDir;
    Vector3 portal2RelativeDir;
    Quaternion tempQuat;

    // Use this for initialization
    void Start()
    {

        portal1Camera = portal1.transform.GetChild(0);
        portal2Camera = portal2.transform.GetChild(0);

        portal1Camera.localPosition = Quaternion.Euler(0,180,0) * portal1.transform.InverseTransformPoint(this.transform.position);
        portal2Camera.localPosition = Quaternion.Euler(0,180,0) * portal2.transform.InverseTransformPoint(this.transform.position);


        //TODO remove z axis rotation
        tempQuat = Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(portal2.transform.rotation) * (transform.rotation);
        tempQuat = Quaternion.Euler(tempQuat.eulerAngles.x, tempQuat.eulerAngles.y, 0);
        portal1Camera.localRotation = tempQuat;
        tempQuat = Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(portal1.transform.rotation) * (transform.rotation);
        tempQuat = Quaternion.Euler(tempQuat.eulerAngles.x, tempQuat.eulerAngles.y, 0);
        portal2Camera.localRotation = tempQuat;


        Vector4 clipPlane1 = CameraSpacePlane(portal1Camera.GetComponent<Camera>(), portal1.transform.position, portal1.transform.forward, -1.0f);
        Matrix4x4 projection1 = portal1Camera.GetComponent<Camera>().projectionMatrix;
        CalculateObliqueMatrix(ref projection1, clipPlane1);
        portal1Camera.GetComponent<Camera>().projectionMatrix = projection1;

        Vector4 clipPlane2 = CameraSpacePlane(portal2Camera.GetComponent<Camera>(), portal2.transform.position, portal2.transform.forward, -1.0f);
        Matrix4x4 projection2 = portal2Camera.GetComponent<Camera>().projectionMatrix;
        CalculateObliqueMatrix(ref projection2, clipPlane2);
        portal2Camera.GetComponent<Camera>().projectionMatrix = projection2;



    }

    // Update is called once per frame
    void Update()
    {
        portal1RelativePos = portal1.transform.InverseTransformPoint(this.transform.position);
        portal2RelativePos = portal2.transform.InverseTransformPoint(this.transform.position);

        portal1Camera.localPosition = Quaternion.Euler(0, 180, 0) * portal1.transform.InverseTransformPoint(this.transform.position);
        portal2Camera.localPosition = Quaternion.Euler(0, 180, 0) * portal2.transform.InverseTransformPoint(this.transform.position);

        tempQuat = Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(portal2.transform.rotation) * (transform.rotation);
        tempQuat = Quaternion.Euler(tempQuat.eulerAngles.x, tempQuat.eulerAngles.y, 0);
        portal1Camera.localRotation = tempQuat;
        tempQuat = Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(portal1.transform.rotation) * (transform.rotation);
        tempQuat = Quaternion.Euler(tempQuat.eulerAngles.x, tempQuat.eulerAngles.y, 0);
        portal2Camera.localRotation = tempQuat;


        Vector4 clipPlane1 = CameraSpacePlane(portal1Camera.GetComponent<Camera>(), portal1.transform.position, portal1.transform.forward, -1f);
        Matrix4x4 projection1 = portal1Camera.GetComponent<Camera>().projectionMatrix;
        CalculateObliqueMatrix(ref projection1, clipPlane1);
        portal1Camera.GetComponent<Camera>().projectionMatrix = projection1;

        Vector4 clipPlane2 = CameraSpacePlane(portal2Camera.GetComponent<Camera>(), portal2.transform.position, portal2.transform.forward, -1f);
        Matrix4x4 projection2 = portal2Camera.GetComponent<Camera>().projectionMatrix;
        CalculateObliqueMatrix(ref projection2, clipPlane2);
        portal2Camera.GetComponent<Camera>().projectionMatrix = projection2;



    }
    //following code is from http://wiki.unity3d.com/index.php/MirrorReflection3
    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }
    // Adjusts the given projection matrix so that near plane is the given clipPlane
    // clipPlane is given in camera space. See article in Game Programming Gems 5 and
    // http://aras-p.info/texts/obliqueortho.html
    private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(
            sgn(clipPlane.x),
            sgn(clipPlane.y),
            1.0f,
            1.0f
        );
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        // third row = clip plane - fourth row
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
    }
    // Extended sign: returns -1, 0 or 1 based on sign of a
    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }
}
