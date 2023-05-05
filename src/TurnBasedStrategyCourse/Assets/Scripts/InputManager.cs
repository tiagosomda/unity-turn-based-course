using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one {nameof(InputManager)}. {this.gameObject.name} and {Instance.gameObject.name}");
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    public Vector2 GetMouseScreenPosition()
    {
        return Input.mousePosition;
    }

    public bool IsMouseButtonDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public Vector2 GetCameraMoveVector()
    {
        Vector2 inputMoveDir = new Vector3(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1;
        }

        return inputMoveDir;
    }

    public float GetCameraRotateAmount()
    {
        float rotateAmount = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1;
        }
        return rotateAmount;
    }

    public float GetCameraZoomAmount()
    {
        float zoomAmount = 0f;
        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = +1;
        }

        return zoomAmount;
    }
}
