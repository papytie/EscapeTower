using UnityEngine;

public class ForegroundParallax : MonoBehaviour {

    [SerializeField] float baseOrthoSize = 5.4f;
    [SerializeField] float zoomFactor = 0.9f;
    [SerializeField] float horizontalParallaxFactor = 0.9f;
    [SerializeField] float verticalParallaxFactor = 0.9f;

    private Camera cam;
    private Vector3 basePosition;

    private void Awake() {
        cam = Camera.main;
        basePosition = transform.position;
    }

    void LateUpdate() {
        float scale = cam.orthographicSize / baseOrthoSize;
        scale = 1 + (scale - 1) * zoomFactor;
        transform.localScale = scale * Vector3.one;

        Vector3 cameraDistance = cam.transform.position - basePosition;
        Vector3 position = basePosition;
        position.x += cameraDistance.x * horizontalParallaxFactor;
        position.y += cameraDistance.y * verticalParallaxFactor;
        transform.position = position;
    }
}
