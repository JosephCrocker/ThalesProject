using UnityEngine;
using System.Collections;

public static class StringHandler {

    public static class Planes
    {
        public const string Commercial      = "Commercial";
        public const string LightAircraft   = "LightAircraft";
        public const string Priority        = "Priority";
    }

    public static class Events
    {
        public const string InitializeSelection     = "InitializeSelection";
        public const string ReleaseSelection        = "ReleaseSelection";
        public const string RevertToDefaultShader   = "RevertToDefaultShader";
        public const string UpdateMaterial          = "UpdateMaterial";
    }

    public static class CoRoutines
    {
        public const string LerpHeading         = "LerpHeading";
        public const string NewHeading          = "NewHeading";

        public const string RotateCompass       = "RotateCompass";
        public const string UpdateCamera        = "UpdateCamera";
        public const string UpdateHeight        = "UpdateHeight";
        public const string UpdateSpeed         = "UpdateSpeed";
        public const string UpdatePlaneHeading  = "UpdatePlaneHeading";

        public const string RotateArrow         = "RotateArrow";
    }
}
