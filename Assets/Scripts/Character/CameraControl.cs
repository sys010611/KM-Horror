using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // xRotation �ʵ带 �����մϴ�.
    private float xRotation = 0f;

    public float sensitivity = 100f;

    public Transform playerBody;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ���콺 �Է����� xRotation ���� �����մϴ�.
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -50f, 50f);

        // ī�޶��� ȸ������ �����մϴ�.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
