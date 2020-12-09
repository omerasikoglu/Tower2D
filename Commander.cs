using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Commander : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 movDir = new Vector2(x, y).normalized;
        float moveSpeed = 5f;
        transform.position += (Vector3)movDir * moveSpeed * Time.deltaTime;//deltatime framerate den bagımsız oldugunu belirtiyor
    }
}
