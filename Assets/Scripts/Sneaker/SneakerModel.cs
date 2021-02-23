using UnityEngine;

namespace Sneakers
{
    public class SneakerModel : MonoBehaviour
    {
        private string _model;
        private int _id;
        private SneakerState _state;

        public Coroutine route;
        public int currentPoint;
        private TransporterType _transporterType;

        [SerializeField] private SneakerView view;

        public string Model { get => _model; set => _model = value; }
        public int Id { get => _id; set => _id = value; }
        public TransporterType TransporterType => _transporterType;
        public SneakerState State => _state;
        public DragDropItem DragDropItem => view.DragDropItem;

        public void SetState(SneakerState newState)
        {
            _state = newState;
            view.SetState(newState);
        }

        public void SetTransporterType(TransporterType newTransporterType)
        {
            _transporterType = newTransporterType;
        }

        public void SwitchVisibility(bool visible)
        {
            view.SwitchVisibility(_state, visible);
        }
    }
}
