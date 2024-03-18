using System.Collections.Generic;
using System;
using UnityEngine;

public class testPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    private float verticalRotation = 0f;
    private CharacterController characterController;
    [SerializeField]
    private Transform cameraTransform;
    public Queue<Action> interActionQueue = new Queue<Action>();

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        InterAction();

        // 수평 축 이동
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * moveVertical + transform.right * moveHorizontal;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 수직 축 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void InterAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
            if (interActionQueue.Count != 0)
                interActionQueue.Dequeue().Invoke();
    }
}
