using Backend.Systems.Component;
using Backend.Systems.Update;

namespace Backend.Systems.Timer
{
    public class TextCountDown : BaseText, IUpdatable
    {
        private Deadline _deadline;

        public void Init(Deadline deadline)
        {
            _deadline = deadline;
        }

        public void Tick(float dt)
        {
            if (_deadline == null) return;
            var ts = _deadline.GetRemainTime();
            SetText($"{(int)ts.TotalMinutes:00}:{ts.Seconds:00}");
        }
    }
}