public enum Mode {

    /// <summary>
    /// Camera moves with the target transform instantly.
    /// </summary>
    Instantenous,

    /// <summary>
    /// If target transform moves withing defined dead zone, camera
    /// will not follow.
    /// </summary>
    DeadZone

}