using UnityEngine;

namespace TPPCamera.TPPCamComponent {

    public sealed class TPPCam : MonoBehaviour {

        #region FIELDS
        // velocity check
        private Vector3 lastTargetPos;

        // movement
        private Vector3 smoothCamOffset;

        private Vector3 targetVelocity;

        // camera follow target
        private Vector3 targetTransPos;

        private float lerpSpeed;

        private Vector3 updatedLookAtPointOffset;

        private Vector3 updatedCameraOffset;

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
        private Vector3 movementVelocityOffset;

        [SerializeField]
        private Vector3 offsetWhenNotVisible;

        [SerializeField]
        private float perspectiveChangeSpeed = 10f;

        // main variables
        [SerializeField]
        private Transform targetTransform;

        private Quaternion endRotation;

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

        public Vector3 MovementVelocityOffset {
            get { return movementVelocityOffset; }
            set { movementVelocityOffset = value; }
        }

        public Vector3 OffsetWhenNotVisible {
            get { return offsetWhenNotVisible; }
            set { offsetWhenNotVisible = value; }
        }

        public float PerspectiveChangeSpeed {
            get { return perspectiveChangeSpeed; }
            set { perspectiveChangeSpeed = value; }
        }

        public Transform TargetTransform {
            get { return targetTransform; }
            set { targetTransform = value; }
        }

        private Vector3 TargetTransPos {
            get { return targetTransPos; }
            set { targetTransPos = value; }
        }

        private Vector3 TargetVelocity {
            get { return targetVelocity; }
            set { targetVelocity = value; }
        }

        /// <summary>
        /// Helper variable.
        /// </summary>
        private Vector3 LastTargetPos {
            get { return lastTargetPos; }
            set { lastTargetPos = value; }
        }

        private Vector3 SmoothCamOffset {
            get { return smoothCamOffset; }
            set { smoothCamOffset = value; }
        }

        private float LerpSpeed {
            get { return lerpSpeed; }
            set { lerpSpeed = value; }
        }

        private Vector3 UpdatedLookAtPointOffset {
            get { return updatedLookAtPointOffset; }
            set { updatedLookAtPointOffset = value; }
        }

        private Vector3 UpdatedCameraOffset {
            get { return updatedCameraOffset; }
            set { updatedCameraOffset = value; }
        }

        private bool TargetTransformVisible {
            get { return targetTransformVisible; }
            set { targetTransformVisible = value; }
        }

        #endregion
        #region UNITY MESSAGES

        private void FixedUpdate() {
            FollowTarget();
        }

        private void Start() {
            TargetTransPos = TargetTransform.position;
            SmoothCamOffset = CameraOffset;
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
                cameraOffsetPos + TargetVelocity,
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

            CalculateTargetVelocity();
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
                TargetTransPos + SmoothCamOffset,
                LerpSpeed);
        }

        private void CalculateEndRotation() {
            // save camera rotation
            endRotation = Quaternion.identity;

            var dir1 = transform.position - TargetTransPos;
            var dir2 = (TargetTransPos + UpdatedLookAtPointOffset + dir1)
                - transform.position;

            endRotation.SetLookRotation(
                dir2,
                TargetTransform.up);
        }

        private void UpdateSmoothCamOffset() {
            UpdatedCameraOffset += TargetVelocity;

            // control occlusion offsets
            SmoothCamOffset = Vector3.MoveTowards(
                SmoothCamOffset,
                UpdatedCameraOffset,
                Time.fixedDeltaTime * PerspectiveChangeSpeed);
        }

        private void HandleTargetTransformVisible() {
            if (!TargetTransformVisible) return;

            UpdatedCameraOffset = CameraOffset;

            UpdatedLookAtPointOffset =
            new Vector3(
                LookAtPointOffset.x,
                -SmoothCamOffset.y,
                LookAtPointOffset.y);               
        }

        private void HandleTargetTransformNotVisible() {
            if (TargetTransformVisible) return;

            UpdatedCameraOffset = OffsetWhenNotVisible;

            UpdatedLookAtPointOffset = new Vector3(
                LookAtPointWhenNotVisible.x,
                -SmoothCamOffset.y,
                LookAtPointWhenNotVisible.y);
        }

        private void CalculateLerpSpeed() {
            // Increase camera speed along with target speed.
            var averageVelocity = (Mathf.Abs(TargetVelocity.x)
                               + Mathf.Abs(TargetVelocity.y)
                               + Mathf.Abs(TargetVelocity.z)) / 3;

            LerpSpeed = (averageVelocity + FollowSpeed)
                        * Time.fixedDeltaTime;
        }

        private void CalculateTargetVelocity() {
            // target speed check
            TargetVelocity = (TargetTransform.position - LastTargetPos)
                             / Time.fixedDeltaTime;

            TargetVelocity = Vector3.Scale(
                TargetVelocity,
                MovementVelocityOffset);

            // save last target position
            LastTargetPos = TargetTransform.position;
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
            else {
                TargetVelocity = Vector3.zero;
            }
        }

        #endregion METHODS
    }

}