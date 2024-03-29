﻿namespace FPacker.P3D.Math; 

[Flags]
public enum PointFlags : uint {
    NONE = 0U,
    ONLAND = 1U,
    UNDERLAND = 2U,
    ABOVELAND = 4U,
    KEEPLAND = 8U,
    LAND_MASK = 15U,
    DECAL = 256U,
    VDECAL = 512U,
    DECAL_MASK = 768U,
    NOLIGHT = 16U,
    AMBIENT = 32U,
    FULLLIGHT = 64U,
    HALFLIGHT = 128U,
    LIGHT_MASK = 240U,
    NOFOG = 4096U,
    SKYFOG = 8192U,
    FOG_MASK = 12288U,
    USER_MASK = 16711680U,
    USER_STEP = 65536U,
    SPECIAL_MASK = 251658240U,
    SPECIAL_HIDDEN = 16777216U,
    ALL_FLAGS = 268383231U
}