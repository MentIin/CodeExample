using System;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.Modules;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.StaticData;
using CodeBase.Infrastructure.Data;
using CodeBase.Infrastructure.Data.Modules;
using CodeBase.UI.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Presenters.Machine
{
    public class MachineModuleSlotPresenter : MonoBehaviour, IDropHandler
    {
        public ModuleDragDrop DragDrop;
        [SerializeField] private Image _background;
        [SerializeField] private Image _rotation;
        
        private RectTransform _rectTransform;
        private int _place;
        private DoubleClickService _doubleClick;
        private FocusManager _gameStopOverFocusManager;
        private MachineModel _machineModel;
        private MachinePresenter _machinePresenter;
        
        private ModulePresentableInfo _moduleInfo;
        private IPersistentProgressService _progressService;
        private MachineBasementStaticData _basementStaticData;

        public event Action<int, int> ModulesSwapped;
        
        public ModulePresentableInfo ModuleSerializableInfo
        {
            get => _moduleInfo;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Construct(MachineModel model, int place, ModuleDragDrop dragDrop,
            DoubleClickService doubleClickService, FocusManager gameStopOverFocusManager,
            MachinePresenter machinePresenter, IPersistentProgressService progressService,
            DragDropManager dragDropManager, MachineBasementStaticData basementStaticData)
        {
            _basementStaticData = basementStaticData;
            _progressService = progressService;
            
            _machineModel = model;
            
            _doubleClick = doubleClickService;
            _machinePresenter = machinePresenter;
            
            DragDrop = dragDrop;
            dragDrop.Construct(dragDropManager);
            dragDrop.SetPosition(_rectTransform);

            _place = place;
            DragDrop.Place = place;
            
            DragDrop.BeginDrag += DragDropOnBeginDrag;
            DragDrop.EndDrag += DragDropOnEndDrag;
            DragDrop.PointerDown += DragDropOnPointerDown;
            _gameStopOverFocusManager = gameStopOverFocusManager;



            _rotation.transform.rotation = basementStaticData.ModulePlacementInfos[_place].Direction.ToQuaternion();
        }

        private void DragDropOnPointerDown()
        {
            _gameStopOverFocusManager.SetFocusOnMachineModule(_place, _machinePresenter,
                _moduleInfo.Level);
            _doubleClick.ClickAndCheck(this);
        }

        private void DragDropOnEndDrag()
        {
            
        }

        private void DragDropOnBeginDrag()
        {
            
        }

        public void SetPosition(Vector2 position)
        {
            _rectTransform.position = position;
            DragDrop.SetPosition(transform);
        }

        public void SetModuleVisual(Sprite icon, string nameKey)
        {
            DragDrop.SetVisual(icon, nameKey);
        }

        public void SetRotation(Direction direction)
        {
            DragDrop.SetRotation(direction);
        }

        public void SetBackgroundVisibility(bool active)
        {
            _background.enabled = active;

        }

        public void SetDragDropActive(bool active)
        {
            DragDrop.gameObject.SetActive(active);
        }

        public void SetData(ModulePresentableInfo presentableInfo)
        {
            _moduleInfo = presentableInfo;
            DragDrop.ModulePresentableInfo = presentableInfo;
            
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                ModuleDragDrop dragDrop;
                if (eventData.pointerDrag.TryGetComponent(out dragDrop))
                {
                    if (!dragDrop.OwningModule)
                    {
                        ModulesSwapped?.Invoke(_place, dragDrop.Place);
                    }
                    else
                    {
                        //OwningModuleInstalled?.Invoke(_place, dragDrop.
                        //Place.x); //dragDrop.Place.x is id
                        InstallOwningModule(_place, dragDrop.ModulePresentableInfo);
                        
                    }
                }
            }
        }
        private void InstallOwningModule(int place, ModulePresentableInfo presentableInfo)
        {
            if (_machineModel.HaveFreePlace(place))
            {
                _progressService.Progress.InstallOwningModule(presentableInfo.ModuleStaticData.Id,
                    presentableInfo.Level, place);
                _gameStopOverFocusManager.SetFocusOnMachineModule(place, _machinePresenter, presentableInfo.Level);
            }
        }
    }
}