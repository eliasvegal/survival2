using UnityEngine;
using System.Collections.Generic;

public class RagdollDragger : MonoBehaviour
{
    [Header("Referencia")]
    public Transform personaje;

    [Header("Drag Settings")]
    public float springForce = 500f;
    public float damper = 25f;
    public float maxDistance = 0.2f;
    public float dragForceMultiplier = 1f;

    private Camera cam;
    private GameObject pointerObj;
    private Rigidbody pointerRb;
    private SpringJoint joint;

    private float currentDistance;
    private Vector3 targetPosition;

    struct BoneTransform
    {
        public Transform bone;
        public Vector3 pos;
        public Quaternion rot;
    }

    private List<BoneTransform> initialBones = new List<BoneTransform>();

    void Start()
    {
        cam = Camera.main;

        pointerObj = new GameObject("MouseAnchor");
        pointerRb = pointerObj.AddComponent<Rigidbody>();
        pointerRb.isKinematic = true;

        SaveInitialBones();
    }

    void SaveInitialBones()
    {
        if (personaje == null)
        {
            Debug.LogError("No asignaste el personaje.");
            return;
        }

        Rigidbody[] rbs = personaje.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rbs)
        {
            BoneTransform bt;
            bt.bone = rb.transform;
            bt.pos = rb.transform.localPosition;
            bt.rot = rb.transform.localRotation;
            initialBones.Add(bt);
        }
    }

    void Update()
    {
        HandleInput();

        if (joint != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = currentDistance;
            targetPosition = cam.ScreenToWorldPoint(mousePos);
        }
    }

    void FixedUpdate()
    {
        if (joint != null)
        {
            pointerRb.MovePosition(targetPosition);
        }
    }

    void HandleInput()
    {
        // Reset con CTRL
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            ResetRagdoll();
        }

        // Click inicial
        if (Input.GetMouseButtonDown(0))
        {
            TryGrab();
        }

        // Soltar
        if (Input.GetMouseButtonUp(0))
        {
            Release();
        }
    }

    void TryGrab()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.rigidbody != null)
            {
                currentDistance = hit.distance;

                pointerObj.transform.position = hit.point;

                joint = hit.rigidbody.gameObject.AddComponent<SpringJoint>();
                joint.connectedBody = pointerRb;

                joint.spring = springForce * dragForceMultiplier;
                joint.damper = damper;
                joint.maxDistance = maxDistance;
                joint.autoConfigureConnectedAnchor = false;
                joint.anchor = hit.rigidbody.transform.InverseTransformPoint(hit.point);
                joint.connectedAnchor = Vector3.zero;
            }
        }
    }

    void Release()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
    }

    void ResetRagdoll()
    {
        Release();

        foreach (BoneTransform bt in initialBones)
        {
            bt.bone.localPosition = bt.pos;
            bt.bone.localRotation = bt.rot;

            Rigidbody rb = bt.bone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.WakeUp(); // 🔥 esto es lo importante
            }
        }
    }
}