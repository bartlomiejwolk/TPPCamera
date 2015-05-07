#define DEBUG_LOGGER

using FileLogger;
using UnityEngine;

namespace TPPCamera.TPPCamComponent {

    public sealed class TPPCam : MonoBehaviour {

        #region FIELDS
        /// <summary>
        /// Final rotation that the camera will be lerping to.
        /// </summary>
        // todo why it can't be a property?
        private Quaternion endRotation;

        #endregion FIELDS

        #region INSPECTOR FIELDS
        [SerializeField]
        // todo rename to deadZone
        private Vector2 cameraLimits = new Vector2(5f, 5f);

        [SerializeField]
        private LayerMask cameraOcclusionLayerMask = 1 << 9;

        [SerializeField]
        private Vector3 cameraOffset;

        [SerializeField]
        private float cameraRotationSpeed = 5f;

        [SerializeField]
        private float followSpeed = 5f;

        [SerializeField]
        private Vector2 lookAtPointOffset;

        // see through camera variables
        [SerializeField]
        private Vector2 lookAtPointWhenNotVisible;

        [SerializeField]
        private Mode mode;

        [SerializeField]
        private Vector3 offsetWhenNotVisible;

        [SerializeField]
        private float cameraOffsetLerpSpeed = 10f;

        // main variables
        [SerializeField]
        private Transform targetTransform;
        #endregion

        #region PROPERTIES
        public Vector2 CameraLimits {
            get { return cameraLimits; }
            set { cameraLimits = value; }
        }

        public LayerMask CameraOcclusionLayerMask {
            get { return cameraOcclusionLayerMask; }
            set { cameraOcclusionLayerMask = value; }
        }

        public Vector3 CameraOffset {
            get { return cameraOffset; }
            set { cameraOffset = value; }
        }

        public float CameraRotationSpeed {
            get { return cameraRotationSpeed; }
            set { cameraRotationSpeed = value; }
        }

        public float FollowSpeed {
            get { return followSpeed; }
            set { followSpeed = value; }
        }

        public Vector2 LookAtPointOffset {
            get { return lookAtPointOffset; }
            set { lookAtPointOffset = value; }
        }

        public Vector2 LookAtPointWhenNotVisible {
            get { return lookAtPointWhenNotVisible; }
            set { lookAtPointWhenNotVisible = value; }
        }

        public Mode Mode {
            get { return mode; }
            set { mode = value; }
        }

        public Vector3 OffsetWhenNotVisible {
            get { return offsetWhenNotVisible; }
            set { offsetWhenNotVisible = value; }
        }

        public float CameraOffsetLerpSpeed {
            get { return cameraOffsetLerpSpeed; }
            set { cameraOffsetLerpSpeed = value; }
        }

        public Transform TargetTransform {
            get { return targetTransform; }
            set { targetTransform = value; }
        }

        /// <summary>
        /// Holds info about target transform position.
        /// Used in Limited mode.
        /// </summary>
        private Vector3 TargetTransformPos { get; set; }

        /// <summary>
        /// Camera offset while following the target transform.
        /// </summary>
        private Vector3 LerpedCameraOffset { get; set; }

        /// <summary>
        /// Camera position lerp speed.
        /// </summary>
        private float LerpPositionSpeed { get; set; }

        /// <summary>
        /// Depending on whether the target transform is occluded or not, 
        /// this will be one of the two look at point offsets specified in the
        /// inspector.
        /// </summary>
        private Vector3 EndLookAtPointOffset { get; set; }

        /// <summary>
        /// Depending on whether the target transform is occluded or not, 
        /// this will be one of the two camera offsets specified in the
        /// inspector.
        /// </summary>
        private Vector3 EndCameraOffset { get; set; }

        /// <summary>
        /// This is true when target transform is visible to the camera, ie.
        /// is not occluded.
        /// </summary>
        private bool TargetTransformVisible { get; set; }

        #endregion
        #region UNITY MESSAGES

        private void FixedUpdate() {
            // todo this should rather be done in LateUpdate()
            FollowTarget();
        }

        private void Start() {
            TargetTransformPos = TargetTransform.position;
            LerpedCameraOffset = CameraOffset;
        }

        #endregion UNITY MESSAGES

        #region METHODS

        /// <summary>
        /// Detect if player is visible.
        /// </summary>
        private void CheckTargetTransformOcclusion() {
            // This is the default camera position if target object was not
            // moving and wasn't occluded.
            var cameraOffsetPos = TargetTransform.position + CameraOffset;
            // Get distance for raycast.
            var tDist =
                (TargetTransform.position - cameraOffsetPos).magnitude
                // Ray length decreased by 0.1 to not hit the floor.
                - 0.1f;
            // Get direction for raycast.
            var tDir =
                (TargetTransform.position - cameraOffsetPos).normalized;

            RaycastHit hit;

            // Check if player visible.
            TargetTransformVisible = !Physics.Raycast(
                cameraOffsetPos,
                tDir,
                out hit,
                tDist,
                CameraOcclusionLayerMask);

        }

        private void FollowTarget() {
            if (TargetTransform == null) return;

            CalculateLerpSpeed();
            UpdateTargetTransformPosition();
            CheckTargetTransformOcclusion();
            HandleTargetTransformVisible();
            HandleTargetTransformNotVisible();
            LerpCameraOffset();
            CalculateEndRotation();

            ApplyEndPosition();
            ApplyEndRotation();
        }

        private void ApplyEndRotation() {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                endRotation,
                CameraRotationSpeed * Time.fixedDeltaTime);
        }

        private void ApplyEndPosition() {
            // apply transformations
            transform.position = Vector3.Lerp(
                transform.position,
                TargetTransformPos + LerpedCameraOffset,
                LerpPositionSpeed);
        }

        private void CalculateEndRotation() {
            var dir1 = transform.position - TargetTransformPos;
            var dir2 = (TargetTransformPos + EndLookAtPointOffset + dir1)
                - transform.position;

            endRotation.SetLookRotation(
                dir2,
                TargetTransform.up);
        }

        private void LerpCameraOffset() {
            LerpedCameraOffset = Vector3.MoveTowards(
                LerpedCameraOffset,
                EndCameraOffset,
                CameraOffsetLerpSpeed * Time.fixedDeltaTime);
        }

        private void HandleTargetTransformVisible() {
            if (!TargetTransformVisible) return;

            EndCameraOffset = CameraOffset;

            EndLookAtPointOffset =
                new Vector3(
                    LookAtPointOffset.x,
                    -LerpedCameraOffset.y,
                    LookAtPointOffset.y);               
        }

        private void HandleTargetTransformNotVisible() {
            if (TargetTransformVisible) return;

            EndCameraOffset = OffsetWhenNotVisible;

            EndLookAtPointOffset = new Vector3(
                LookAtPointWhenNotVisible.x,
                -LerpedCameraOffset.y,
                LookAtPointWhenNotVisible.y);
        }

        private void CalculateLerpSpeed() {
            LerpPositionSpeed = FollowSpeed * Time.fixedDeltaTime;
        }

        private void UpdateTargetTransformPosition() {
            switch (Mode) {
                case Mode.Limited:
                    HandleCameraLimits();

                    TargetTransformPos = new Vector3(
                        TargetTransformPos.x,
                        TargetTransform.position.y,
                        TargetTransformPos.z);
                    break;
                case Mode.Instantenous:
                    TargetTransformPos = TargetTransform.position;
                    break;
            }
        }

        private void HandleCameraLimits() {
            var exceedLimitX = Mathf.Abs(
                    TargetTransform.position.x - TargetTransformPos.x)
                    > CameraLimits.x;

            var exceedLimitZ = Mathf.Abs(
                    TargetTransform.position.z - TargetTransformPos.z)
                    > CameraLimits.y;

            if (exceedLimitX || exceedLimitZ) {
                var dir =
                    (TargetTransform.position - TargetTransformPos).normalized;
                var magnitude =
                    (TargetTransform.position - TargetTransformPos).magnitude;

                TargetTransformPos += dir * magnitude * Time.fixedDeltaTime
                    * FollowSpeed;
            }
        }

        #endregion METHODS
    }

}