namespace Passer.Humanoid.Tracking {
    public enum FaceBone {
        LeftOuterBrow,
        LeftBrow,
        LeftInnerBrow,
        RightInnerBrow,
        RightBrow,
        RightOuterBrow,

        LeftCheek,
        RightCheek,

        NoseTopLeft,
        NoseTop,
        NoseTopRight,
        NoseTip,
        NoseBottomLeft,
        NoseBottom,
        NoseBottomRight,

        UpperLipLeft,
        UpperLip,
        UpperLipRight,
        LipLeft,
        LipRight,
        LowerLipLeft,
        LowerLip,
        LowerLipRight,
        LastBone
    }

    public class FaceSensor : Sensor {
        public TrackedBrow leftBrow = new();
        public TrackedBrow rightBrow = new();

        public TrackedEye leftEye = new();
        public TrackedEye rightEye = new();

        public TargetData leftCheek = new();
        public TargetData rightCheek = new();

        public TrackedNose nose = new();

        public TrackedMouth mouth = new();

        public TargetData jaw = new();

        public float smile;
        public float pucker;
        public float frown;

        public FaceSensor(DeviceView deviceView) : base(deviceView) { }

        public class TrackedBrow {
            public TargetData inner;
            public TargetData center;
            public TargetData outer;
        }

        public class TrackedEye {
            public float closed;
        }

        public class TrackedNose {
            public TargetData top;
            public TargetData topLeft;
            public TargetData topRight;
            public TargetData tip;
            public TargetData bottom;
            public TargetData bottomLeft;
            public TargetData bottomRight;
        }

        public class TrackedMouth {
            public TargetData upperLipLeft;
            public TargetData upperLip;
            public TargetData upperLipRight;

            public TargetData lipLeft;
            public TargetData lipRight;

            public TargetData lowerLipLeft;
            public TargetData lowerLip;
            public TargetData lowerLipRight;
        }

        public class FaceTargetData : TargetData {
            public new Vector startPosition;
        }
    }
}
