using System.Collections.Generic;
using UnityEngine;


namespace Backend.Systems.Update
{
    public sealed class UpdateService : MonoBehaviour, IUpdateService
    {
        private readonly List<IUpdatable> _update = new(256);
        private readonly List<ILateUpdatable> _lateUpdate = new(128);
        private readonly List<IFixedUpdatable> _fixedUpdate = new(128);

        private readonly List<IUpdatable> _updateAdd = new(64);
        private readonly List<IUpdatable> _updateRemove = new(64);

        private readonly List<ILateUpdatable> _lateAdd = new(32);
        private readonly List<ILateUpdatable> _lateRemove = new(32);

        private readonly List<IFixedUpdatable> _fixedAdd = new(32);
        private readonly List<IFixedUpdatable> _fixedRemove = new(32);


        public void Add(IUpdatable updatable)
        {
            if (updatable == null) return;
            _updateAdd.Add(updatable);
        }

        public void Remove(IUpdatable updatable)
        {
            if (updatable == null) return;
            _updateRemove.Add(updatable);
        }

        public void Add(ILateUpdatable updatable)
        {
            if (updatable == null) return;
            _lateAdd.Add(updatable);
        }

        public void Remove(ILateUpdatable updatable)
        {
            if (updatable == null) return;
            _lateRemove.Add(updatable);
        }

        public void Add(IFixedUpdatable updatable)
        {
            if (updatable == null) return;
            _fixedAdd.Add(updatable);
        }

        public void Remove(IFixedUpdatable updatable)
        {
            if (updatable == null) return;
            _fixedRemove.Add(updatable);
        }

        private void Update()
        {
            ApplyPending(_update, _updateAdd, _updateRemove);

            var dt = Time.deltaTime;
            for (int i = 0; i < _update.Count; i++)
                _update[i].Tick(dt);
        }

        private void LateUpdate()
        {
            ApplyPending(_lateUpdate, _lateAdd, _lateRemove);

            var dt = Time.deltaTime;
            for (int i = 0; i < _lateUpdate.Count; i++)
                _lateUpdate[i].LateTick(dt);
        }

        private void FixedUpdate()
        {
            ApplyPending(_fixedUpdate, _fixedAdd, _fixedRemove);

            var fdt = Time.fixedDeltaTime;
            for (int i = 0; i < _fixedUpdate.Count; i++)
                _fixedUpdate[i].FixedTick(fdt);
        }

        private static void ApplyPending<T>(List<T> main, List<T> add, List<T> remove)
        {
            if (remove.Count > 0)
            {
                for (int r = 0; r < remove.Count; r++)
                {
                    var item = remove[r];
                    for (int i = main.Count - 1; i >= 0; i--)
                    {
                        if (ReferenceEquals(main[i], item))
                        {
                            main.RemoveAt(i);
                            break;
                        }
                    }
                }

                remove.Clear();
            }

            if (add.Count > 0)
            {
                for (int a = 0; a < add.Count; a++)
                {
                    var item = add[a];
                    var exists = false;

                    for (int i = 0; i < main.Count; i++)
                    {
                        if (ReferenceEquals(main[i], item))
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                        main.Add(item);
                }

                add.Clear();
            }
        }
    }
}