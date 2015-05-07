// Copyright (c) 2015 Bart³omiej Wo³k (bartlomiejwolk@gmail.com)
//  
// This file is part of the TPPCamera extension for Unity.
// Licensed under the MIT license. See LICENSE file in the project root folder.

public enum Mode {

    /// <summary>
    ///     Camera moves with the target transform instantly.
    /// </summary>
    Instantenous,

    /// <summary>
    ///     If target transform moves withing defined dead zone, camera
    ///     will not follow.
    /// </summary>
    DeadZone

}