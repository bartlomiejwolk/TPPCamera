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
        private Vector3 targetTransformPosition;

        private float lerpSpeed;

        private Vector3 occlusionLookAtPointOffset;

        private Vector3 occlusionOffset;

        private bool playerVisible;

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
        private Vector3 lookAtPointWhenNotVisible;

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

        public Vector3 LookAtPointWhenNotVisible {
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

        private Vector3 TargetTransformPosition {
            get { return targetTransformPosition; }
            set { targetTransformPosition = value; }
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

        private Vector3 OcclusionLookAtPointOffset {
            get { return occlusionLookAtPointOffset; }
            set { occlusionLookAtPointOffset = value; }
        }

        private Vector3 OcclusionOffset {
            get { return occlusionOffset; }
            set { occlusionOffset = value; }
        }

        private bool PlayerVisible {
            get { return playerVisible; }
            set { playerVisible = value; }
        }

        #endregion
        #region UNITY MESSAGES

        private void FixedUpdate() {
            FollowTarget();
        }

        private void Start() {
            TargetTransformPosition = TargetTransform.position;
            SmoothCamOffset = CameraOffset;
        }

        #endregion UNITY MESSAGES

        #region METHODS

        // detect if player is visible and set offset accordingly
        // todo refactor
        private void DetectOccluders() {
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

                PlayerVisible = false;
                return;
            }

            PlayerVisible = true;
        }

        private void FollowTarget() {
            if (TargetTransform == null) return;

            CalculateTargetVelocity();
            CalculateLerpSpeed();
            UpdateTargetTransformPosition();
            DetectOccluders();
            UpdateOcclusionLookAtPointOffset();
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
                TargetTransformPosition + SmoothCamOffset,
                LerpSpeed);
        }

        private void CalculateEndRotation() {
            // save camera rotation
            endRotation = Quaternion.identity;
            endRotation.SetLookRotation(
                (TargetTransformPosition
                 + (OcclusionLookAtPointOffset
                    + (transform.position - TargetTransformPosition)))
                - transform.position,
                TargetTransform.up);
        }

        private void UpdateSmoothCamOffset() {
            // control occlusion offsets
            SmoothCamOffset = Vector3.MoveTowards(
                SmoothCamOffset,
                OcclusionOffset,
                Time.fixedDeltaTime * PerspectiveChangeSpeed);
        }

        // todo replace with two separate handlers
        private void UpdateOcclusionLookAtPointOffset() {
            if (PlayerVisible) {
                OcclusionOffset = CameraOffset;
                OcclusionLookAtPointOffset =
                new Vector3(
                    LookAtPointOffset.x,
                    -SmoothCamOffset.y,
                    LookAtPointOffset.y);               
            }
            else {
                OcclusionOffset = OffsetWhenNotVisible;
                OcclusionLookAtPointOffset = new Vector3(
                    LookAtPointWhenNotVisible.x,
                    -SmoothCamOffset.y,
                    LookAtPointWhenNotVisible.z);
            }

            OcclusionOffset += TargetVelocity;
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

        // todo refactor
        private void UpdateTargetTransformPosition() {
            // control camera in Limited mode
            if (Mode == Mode.Limited) {
                // todo extract
                if (Mathf.Abs(TargetTransform.position.x - TargetTransformPosition.x)
                    > CameraLimits.x
                    ||
                    Mathf.Abs(TargetTransform.position.z - TargetTransformPosition.z)
                    > CameraLimits.y) {

                    TargetTransformPosition +=
                        (TargetTransform.position - TargetTransformPosition).normalized
                        * (TargetTransform.position - TargetTransformPosition).magnitude
                        * Time.fixedDeltaTime * FollowSpeed;
                }
                else {
                    TargetVelocity = Vector3.zero;
                }

                TargetTransformPosition = new Vector3(
                    TargetTransformPosition.x,
                    TargetTransform.position.y,
                    TargetTransformPosition.z);
            }
            else {
                TargetTransformPosition = TargetTransform.position;
            }
        }

        #endregion METHODS
    }

}