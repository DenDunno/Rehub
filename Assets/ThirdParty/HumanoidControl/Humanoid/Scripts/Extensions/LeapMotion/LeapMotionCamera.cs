using Passer.Humanoid;
using UnityEngine;

namespace Passer.Tracking {

    /// <summary>
    /// Leap Motion Camera
    /// </summary>
    public class LeapMotionCamera : SensorComponent {
#if hLEAP

        #region Manage

        public LeapHandSkeleton FindHand(bool isLeft) {
            if (this == null)
                return null;

            LeapHandSkeleton[] skeletons = GetComponentsInChildren<LeapHandSkeleton>();
            foreach (LeapHandSkeleton skeleton in skeletons) {
                if (skeleton != null && skeleton.isLeft == isLeft) {
                    return skeleton;
                }
            }
            return null;
        }

        public LeapHandSkeleton leftHand {
            get { return FindHand(true);  }
        }
        public LeapHandSkeleton rightHand {
            get { return FindHand(false);  }
        }

        public void ShowSkeleton(bool shown) {
            leftHand.show = shown;
            rightHand.show = shown;
        }

        #endregion Manage

        #region Start

        public Transform deviceOrigin;

        protected override void Awake() {
            base.Awake();
            GameObject deviceOriginObj = new GameObject("Device Origin");
            deviceOrigin = deviceOriginObj.transform;
            deviceOrigin.transform.SetParent(this.transform);
            deviceOrigin.transform.localPosition = Vector3.zero;
            deviceOrigin.transform.localEulerAngles = Vector3.zero;
        }

        #endregion

        #region Update
        public override void UpdateComponent() {
            
        }

        #endregion Update
#endif
    }
}
