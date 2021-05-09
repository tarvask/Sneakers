using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sneakers
{
    public class SneakerController : IDisposable
    {
        public struct Context
        {
            public SneakerView View { get; }
            public SneakerConfig Config { get; }
            public Action<SneakerController> OnLegendarySneakerCollectedAction { get; }

            public Context(SneakerView view, SneakerConfig config,
                Action<SneakerController> onLegendarySneakerCollectedAction)
            {
                View = view;
                Config = config;
                OnLegendarySneakerCollectedAction = onLegendarySneakerCollectedAction;
            }
        }

        private readonly Context _context;
        private SneakerState _state;
        private TransporterType _transporterType;
        private int _currentPoint;
        private bool _isDisposed;

        public string Model => _context.Config.Model;
        public int Id => _context.Config.Id;
        public bool IsLegendary => _context.Config.IsLegendary;
        public SneakerView View => _context.View;
        public TransporterType TransporterType => _transporterType;
        public SneakerState State => _state;
        public DragDropItem DragDropItem => _context.View.DragDropItem;
        public int CurrentPoint => _currentPoint;
        public Vector3 LocalPosition => _context.View.transform.localPosition;
        public bool IsDisposed => _isDisposed;

        public Coroutine CurrentCoroutine { get; set; }

        public SneakerController(Context context, int id)
        {
            _context = context;
            
            _context.View.name = _context.Config.Model + id;
            _context.View.DragDropItem.Init(this);
            _context.View.OnSneakerDropped += action => action(this);
        }

        public void SetState(SneakerState newState)
        {
            _state = newState;
            _context.View.SetState(newState);
        }

        public void SetTransporterType(TransporterType newTransporterType)
        {
            _transporterType = newTransporterType;
        }

        public void SetCurrentPoint(int currentPoint)
        {
            _currentPoint = currentPoint;
        }

        public void SwitchVisibility(bool visible)
        {
            _context.View.SwitchVisibility(_state, visible);
        }

        public void SetPosition(Vector3 newPosition)
        {
            View.transform.localPosition = newPosition;
        }

        public void Move(Vector3 targetPosition, float speed)
        {
            View.transform.localPosition = Vector3.MoveTowards(View.transform.localPosition,
                targetPosition, speed * Time.deltaTime);
        }

        public void CollectLegendary()
        {
            _context.OnLegendarySneakerCollectedAction.Invoke(this);
        }


        public void Dispose()
        {
            _context.View.StopAllCoroutines();
            CurrentCoroutine = null;
            
            Object.Destroy(_context.View.DragDropItem);
            Object.Destroy(_context.View);
            Object.Destroy(_context.View.gameObject);

            _isDisposed = true;
        }
    }
}
