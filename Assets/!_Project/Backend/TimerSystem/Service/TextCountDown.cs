using Backend.Systems.Component;
using Backend.Systems.Update;
using Reflex.Attributes;

namespace Backend.Systems.Timer
{
    public class TextCountDown : BaseText, IUpdatable
    {
        private Deadline _deadline;

        [Inject] private IUpdateService _updateService;

        public void Init(Deadline deadline)
        {
            _deadline = deadline;
            _updateService.Add(this);
        }

        public void Stop()
        {
            _deadline = null;
            _updateService.Remove(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _updateService?.Remove(this);
        }

        public void Tick(float dt)
        {
            if (_deadline == null) return;
            var ts = _deadline.GetRemainTime();
            SetText($"{(int)ts.TotalMinutes:00}:{ts.Seconds:00}");
            if (_deadline.IsExpired) Stop();
        }
    }
}