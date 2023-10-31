using Narupa.Frame;
using Narupa.Frame.Event;

namespace Narupa.Visualisation.Tests.Node.Adaptor
{
    internal class FrameSnapshot : ITrajectorySnapshot
    {
        public void Update(Frame.Frame frame, FrameChanges changes)
        {
            CurrentFrame = frame;
            FrameChanged?.Invoke(frame, changes);
        }

        public Frame.Frame CurrentFrame { get; private set; }
        public event FrameChanged FrameChanged;
    }
}