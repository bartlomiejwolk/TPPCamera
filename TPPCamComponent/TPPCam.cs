#define DEBUG_LOGGER

using FileLogger;
using UnityEngine;

namespace TPPCamera.TPPCamComponent {

    public sealed class TPPCam : MonoBehaviour {

        #region FIELDS
        /// <summary>
        /// Final rotation that the camera will be lerping to.
        /// </summary>
        private Quaternion endRotation;

        // movement
        private Vector3 lerpedCameraOffset;

        // camera follow target
        private Vector3 targetTransPos;

        private float lerpSpeed;

        private Vector3 updatedLookAtPointOffset;

        private Vector3 endCameraOffset;

        private bool targetTransformVisible;

        #endregion FIELDS

        #region INSPECTOR FIELDS
        // limited camera
        [SerializeField]
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

        private Vector3 TargetTransPos {
            get { return targetTransPos; }
            set { targetTransPos = value; }
        }

        /// <summary>
        /// Camera offset while following the target transform.
        /// </summary>
        private Vector3 LerpedCameraOffset {
            get { return lerpedCameraOffset; }
            set { lerpedCameraOffset = value; }
        }

        private float LerpSpeed {
            get { return lerpSpeed; }
            set { lerpSpeed = value; }
        }

        private Vector3 UpdatedLookAtPointOffset {
            get { return updatedLookAtPointOffset; }
            set { updatedLookAtPointOffset = value; }
        }

        /// <summary>
        /// Depending on whether the target transform is occluded or not, 
        /// this will be one of the two camera offsets specified in the
        /// inspector.
        /// </summary>
        private Vector3 EndCameraOffset {
            get { return endCameraOffset; }
            set { endCameraOffset = value; }
        }

        private bool TargetTransformVisible {
            get { return targetTransformVisible; }
            set { targetTransformVisible = value; }
        }

        #endregion
        #region UNITY MESSAGES

        private void FixedUpdate() {
            // todo this should rather be done in LateUpdate()
            FollowTarget();
        }

        private void Start() {
            TargetTransPos = TargetTransform.position;
            LerpedCameraOffset = CameraOffset;
        }

        #endregion UNITY MESSAGES

        #region METHODS

        // detect if player is visible and set offset accordingly
        // todo refactor
        private void CheckTargetTransformOcclusion() {
            // get distance and direction for raycast
            var cameraOffsetPos = TargetTransform.position + CameraOffset;
            // Ray length decreased by 0.1 to not hit the floor.
            var tDist =
                (TargetTransform.position - cameraOffsetPos).magnitude
                - 0.1f;
            var tDir =
                (TargetTransform.position - cameraOffsetPos).normalized;

            // check if player visible
            RaycastHit hit;
            if (Physics.Raycast(
                //cameraOffsetPos + TargetVelocity,
                cameraOffsetPos,
                tDir,
                out hit,
                tDist,
                CameraOcclusionLayerMask)) {

                TargetTransformVisible = false;
                return;
            }

            TargetTransformVisible = true;
        }

        private void FollowTarget() {
            if (TargetTransform == null) return;

            CalculateLerpSpeed();
            UpdateTargetTransformPosition();
            CheckTargetTransformOcclusion();
            HandleTargetTransformVisible();
            HandleTargetTransformNotVisible();
            UpdateSmoothCamOffset();
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
                TargetTransPos + LerpedCameraOffset,
                LerpSpeed);
        }

        private void CalculateEndRotation() {
            var dir1 = transform.position - TargetTransPos;
            var dir2 = (TargetTransPos + UpdatedLookAtPointOffset + dir1)
                - transform.position;

            endRotation.SetLookRotation(
                dir2,
                TargetTransform.up);
        }

        private void UpdateSmoothCamOffset() {
            LerpedCameraOffset = Vector3.MoveTowards(
                LerpedCameraOffset,
                EndCameraOffset,
                CameraOffsetLerpSpeed * Time.fixedDeltaTime);
        }

        private void HandleTargetTransformVisible() {
            if (!TargetTransformVisible) return;

            EndCameraOffset = CameraOffset;

            UpdatedLookAtPointOffset =
            new Vector3(
                LookAtPointOffset.x,
                -LerpedCameraOffset.y,
                LookAtPointOffset.y);               
        }

        private void HandleTargetTransformNotVisible() {
            if (TargetTransformVisible) return;

            EndCameraOffset = OffsetWhenNotVisible;

            UpdatedLookAtPointOffset = new Vector3(
                LookAtPointWhenNotVisible.x,
                -LerpedCameraOffset.y,
                LookAtPointWhenNotVisible.y);
        }

        private void CalculateLerpSpeed() {
            LerpSpeed = FollowSpeed * Time.fixedDeltaTime;
        }

        private void UpdateTargetTransformPosition() {
            switch (Mode) {
                case Mode.Limited:
                    HandleCameraLimits();

                    TargetTransPos = new Vector3(
                        TargetTransPos.x,
                        TargetTransform.position.y,
                        TargetTransPos.z);
                    break;
                case Mode.Instantenous:
                    TargetTransPos = TargetTransform.position;
                    break;
            }
        }

        private void HandleCameraLimits() {
            var exceedLimitX = Mathf.Abs(
                    TargetTransform.position.x - TargetTransPos.x)
                    > CameraLimits.x;

            var exceedLimitZ = Mathf.Abs(
                    TargetTransform.position.z - TargetTransPos.z)
                    > CameraLimits.y;

            if (exceedLimitX || exceedLimitZ) {
                var dir =
                    (TargetTransform.position - TargetTransPos).normalized;
                var magnitude =
                    (TargetTransform.position - TargetTransPos).magnitude;

                TargetTransPos += dir * magnitude * Time.fixedDeltaTime
                    * FollowSpeed;
            }
        }

        #endregion METHODS
    }

}