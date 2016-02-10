using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class PlayerSyncInput : NetworkBehaviour
{
    public Transform headTransform;
    public Transform bodyTransform;
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public float smoothTime = 5f;
    public bool lockCursor = true;

    private bool m_cursorIsLocked = true;

    //movement input
    private Vector2 lastMoveInputSent = Vector2.zero;
    private Vector2 lastMoveInputRecieved = Vector2.zero;
    private Vector2 interploatedMoveInput = Vector2.zero;

    //mouse look input
    private Quaternion lastRotationSent = Quaternion.identity;
    //private Quaternion lastRotationRecieved = Quaternion.identity;
    private Quaternion localRotation;

    public Vector2 getMovementInput()
    {
        return interploatedMoveInput;
    }

    public Quaternion getRotation()
    {
        return localRotation;
    }

    void Start()
    {
        localRotation = bodyTransform.localRotation;

        CursorLocker.onCursorChanged += cursorStateChanged;
        m_cursorIsLocked = CursorLocker.isLocked();
        CursorLocker.lockCursor();
    }

    void Update()
    {
        if (isLocalPlayer)
            updateBodyRotation();
    }

    void FixedUpdate()
    {
        if (isServer)
        {
            lerpInput();
        }

        if (isLocalPlayer)
        {
            transmitInput();
        }
    }

    private void cursorStateChanged(bool locked)
    {
        m_cursorIsLocked = locked;
    }

    private void lerpInput()
    {
        interploatedMoveInput = lastMoveInputRecieved;
    }

    private void updateBodyRotation()
    {
        if (m_cursorIsLocked)
        {
            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            localRotation *= Quaternion.Euler(0f, yRot, 0f);
        }
    }

    private Quaternion getHeadRotation()
    {
        Quaternion rot = bodyTransform.localRotation;

        if (m_cursorIsLocked)
        {
            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            headTransform.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                headTransform.localRotation = ClampRotationAroundXAxis(headTransform.localRotation);
        }

        return rot;
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    [Command]
    void CmdProvideInputToServer(Vector2 latestInput, Quaternion latestRot)
    {
        lastMoveInputRecieved = latestInput;
        localRotation = latestRot;
    }

    [ClientCallback]
    void transmitInput()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (isLocalPlayer && (!inputVector.Equals(lastMoveInputSent) || !localRotation.Equals(lastRotationSent)))
        {        
            CmdProvideInputToServer(inputVector, localRotation);
            lastMoveInputSent = inputVector;
            lastRotationSent = localRotation;
        }
    }
}
