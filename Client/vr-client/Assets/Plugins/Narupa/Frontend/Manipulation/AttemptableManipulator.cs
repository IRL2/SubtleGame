using Narupa.Core.Math;
using Narupa.Frontend.Input;

namespace Narupa.Frontend.Manipulation
{
    /// <summary>
    /// A function that attempts to start a manipulation given an manipulator pose.
    /// </summary>
    public delegate IActiveManipulation ManipulationAttemptHandler(UnitScaleTransformation manipulatorPose);
    
    public class AttemptableManipulator : Manipulator
    {
        private ManipulationAttemptHandler manipulationAttempt;

        public AttemptableManipulator(IPosedObject posedObject, ManipulationAttemptHandler manipulationAttempt) : base(posedObject)
        {
            this.manipulationAttempt = manipulationAttempt;
        }
        
        public void AttemptManipulation()
        {
            if (Pose is Transformation pose
             && manipulationAttempt(pose.AsUnitTransformWithoutScale()) is IActiveManipulation manipulation)
            {
                SetActiveManipulation(manipulation);
            }
        }
    }
}