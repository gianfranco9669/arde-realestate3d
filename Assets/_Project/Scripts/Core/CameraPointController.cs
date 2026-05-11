using System;
using ARDE.RealEstate3D.Hotspots;
using UnityEngine;

namespace ARDE.RealEstate3D.Core
{
    [Serializable]
    public class CameraPoint
    {
        [SerializeField] private string label;
        [SerializeField] private Transform point;

        public string Label => label;
        public Transform Point => point;

        public CameraPoint(string label, Transform point)
        {
            this.label = label;
            this.point = point;
        }
    }

    public class CameraPointController : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private CameraPoint[] cameraPoints;
        [SerializeField] private float moveSpeed = 3.8f;
        [SerializeField] private float rotateSpeed = 4.5f;
        [SerializeField] private LayerMask hotspotLayerMask = ~0;

        private int currentIndex;
        private Transform activePoint;

        public event Action<int> CameraPointChanged;
        public int CurrentIndex => currentIndex;
        public CameraPoint[] CameraPoints => cameraPoints;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (cameraPoints != null && cameraPoints.Length > 0)
            {
                MoveToPoint(0, true);
            }
        }

        private void Update()
        {
            HandleHotspotInput();
            SmoothCameraMovement();
        }

        public void MoveToPoint(int index)
        {
            MoveToPoint(index, false);
        }

        public void MoveToPoint(int index, bool instant)
        {
            if (cameraPoints == null || index < 0 || index >= cameraPoints.Length || cameraPoints[index].Point == null)
            {
                return;
            }

            currentIndex = index;
            activePoint = cameraPoints[currentIndex].Point;
            CameraPointChanged?.Invoke(currentIndex);

            if (instant && targetCamera != null)
            {
                targetCamera.transform.SetPositionAndRotation(activePoint.position, activePoint.rotation);
            }
        }

        public void NextPoint()
        {
            if (cameraPoints == null || cameraPoints.Length == 0)
            {
                return;
            }

            MoveToPoint((currentIndex + 1) % cameraPoints.Length);
        }

        private void SmoothCameraMovement()
        {
            if (targetCamera == null || activePoint == null)
            {
                return;
            }

            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, activePoint.position, Time.deltaTime * moveSpeed);
            targetCamera.transform.rotation = Quaternion.Slerp(targetCamera.transform.rotation, activePoint.rotation, Time.deltaTime * rotateSpeed);
        }

        private void HandleHotspotInput()
        {
            bool pressed = Input.GetMouseButtonDown(0);
            Vector2 screenPosition = Input.mousePosition;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                pressed = true;
                screenPosition = Input.GetTouch(0).position;
            }

            if (!pressed || targetCamera == null)
            {
                return;
            }

            Ray ray = targetCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f, hotspotLayerMask, QueryTriggerInteraction.Collide))
            {
                Hotspot hotspot = hit.collider.GetComponentInParent<Hotspot>();
                if (hotspot != null)
                {
                    hotspot.Select();
                }
            }
        }
    }
}
