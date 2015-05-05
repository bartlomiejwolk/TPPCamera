using UnityEngine;

public class GameCamera : MonoBehaviour {

    // modes
    public enum Mode {

        Instantenous,
        Limited

    }

    #region FIELDS

    // limited camera
    public Vector2 cameraLimits = new Vector2(5f, 5f);

    public LayerMask cameraOcclusionLayerMask = 1 << 9;
    public Vector3 cameraOffset;
    public float cameraRotationSpeed = 5f;
    public float followSpeed = 5f;
    public Vector2 lookAtPointOffset;

    // see through camera variables
    public Vector3 lookAtPointWhenNotVisible;

    public Mode mode;
    public Vector3 movementVelocityOffset;

    public Vector3 offsetWhenNotVisible;

    public float perspectiveChangeSpeed = 10f;

    // main variables
    public Transform target;

    // camera follow target
    private Vector3 cameraTarget;

    // velocity check
    private Vector3 lastTargetPos;

    // movement
    private Vector3 smoothCamOffset;

    private Vector3 targetVelocity;

    #endregion FIELDS

    #region UNITY MESSAGES

    private void FixedUpdate() {
        FollowTarget();
    }

    private void Start() {
        smoothCamOffset = cameraOffset;
        cameraTarget = target.position;
    }

    #endregion UNITY MESSAGES

    #region METHODS

    public bool DetectOccluders() {
        // get distance and direction for raycast
        var cameraOffsetPos = target.position + cameraOffset;
        // Ray length decreased by 0.1 to not hit the floor.
        var tDist = (target.position - cameraOffsetPos).magnitude - 0.1f;
        var tDir = (target.position - cameraOffsetPos).normalized;

        // check if player visible
        RaycastHit hit;
        if (Physics.Raycast(
            cameraOffsetPos + targetVelocity,
            tDir,
            out hit,
            tDist,
            cameraOcclusionLayerMask)) {
            return false;
        }
        return true;
    }

    protected void FollowTarget() {
        if (target == null) return;

        // target speed check
        targetVelocity = (target.position - lastTargetPos) / Time.fixedDeltaTime;
        targetVelocity = Vector3.Scale(targetVelocity, movementVelocityOffset);
        // camera movement speed
        var avgVelocity = (Mathf.Abs(targetVelocity.x)
                           + Mathf.Abs(targetVelocity.x)
                           + Mathf.Abs(targetVelocity.x)) / 3;
        var lerpSpeed = (avgVelocity + followSpeed) * Time.fixedDeltaTime;

        // save camera rotation
        var tRot = Quaternion.identity;

        // control camera in Limited mode
        if (mode == Mode.Limited) {
            if (Mathf.Abs(target.position.x - cameraTarget.x) > cameraLimits.x
                ||
                Mathf.Abs(target.position.z - cameraTarget.z) > cameraLimits.y) {
                cameraTarget += (target.position - cameraTarget).normalized *
                                (target.position - cameraTarget).magnitude
                                * Time.fixedDeltaTime * followSpeed;
            }
            else {
                targetVelocity = Vector3.zero;
            }
            cameraTarget = new Vector3(
                cameraTarget.x,
                target.position.y,
                cameraTarget.z);
        }
        else {
            cameraTarget = target.position;
        }

        // detect if player is visible and set offset accordingly
        var playerVisible = DetectOccluders();
        var occlusionOffset = cameraOffset;
        var occlusionLookAtPointOffset =
            new Vector3(
                lookAtPointOffset.x,
                -smoothCamOffset.y,
                lookAtPointOffset.y);
        if (!playerVisible) {
            occlusionOffset = offsetWhenNotVisible;
            occlusionLookAtPointOffset = new Vector3(
                lookAtPointWhenNotVisible.x,
                -smoothCamOffset.y,
                lookAtPointWhenNotVisible.z);
        }
        occlusionOffset += targetVelocity;

        // control occlusion offsets
        smoothCamOffset = Vector3.MoveTowards(
            smoothCamOffset,
            occlusionOffset,
            Time.fixedDeltaTime * perspectiveChangeSpeed);
        tRot.SetLookRotation(
            (cameraTarget
             + (occlusionLookAtPointOffset + (transform.position - cameraTarget)))
            -
            transform.position,
            target.up);

        // apply transformations
        transform.position = Vector3.Lerp(
            transform.position,
            cameraTarget + smoothCamOffset,
            lerpSpeed);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            tRot,
            cameraRotationSpeed * Time.fixedDeltaTime);

        // save last target position
        lastTargetPos = target.position;
    }

    #endregion METHODS
}