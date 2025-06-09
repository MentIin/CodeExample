using Assets.CodeBase.Infrastructure.Data.Modules;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.StaticData;
using CodeBase.Infrastructure.Data;
using CodeBase.Infrastructure.Data.Modules;
using CodeBase.Logic;
using CodeBase.UI.Services;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Presenters.Machine
{
    public class MachinePresenter : MonoBehaviour
    {
        [SerializeField] private Image _machineBasementImage;
        [SerializeField] private MachineModuleSlotPresenter moduleSlotPresenterPrefab;
        [SerializeField] private ModuleDragDrop _moduleDragDropPrefab;

        private MachineModel _model;
        private IStaticDataService _staticDataService;
        private MachineModuleSlotPresenter[] _modulePresenters;
        private MachineBasementStaticData _machineBasementStaticData;

        private DoubleClickService _doubleClick;
        private FocusManager _gameStopOverFocusManager;
        private IPersistentProgressService _progressService;
        private DragDropManager _dragDropManager;

        public MachineModel Model
        {
            get => _model;
        }


        public void Construct(MachineBasementStaticData basementStaticData, MachineModel model,
            IStaticDataService staticDataService,
            DoubleClickService doubleClickService, FocusManager gameStopOverFocusManager,
            IPersistentProgressService progressService, DragDropManager dragDropManager)
        {
            if (_model != null)
            {
                _model.MachineModulesChanged -= PresentModulesData;
            }
            
            _progressService = progressService;
            _dragDropManager = dragDropManager;
            _machineBasementStaticData = basementStaticData;
            _model = model;
            _staticDataService = staticDataService;
            _doubleClick = doubleClickService;
            _gameStopOverFocusManager = gameStopOverFocusManager;
            
            model.MachineModulesChanged += PresentModulesData;
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy");
            _model.MachineModulesChanged -= PresentModulesData;
        }

        public void Present()
        {
            
            PresentBasement();
            
            int modulesMaxAmount = _machineBasementStaticData.ModulesMaxAmount;
            
            CreateMachineModulePresenters(modulesMaxAmount);

            PresentModulesData();
        }
        
        public Vector2 CalculateModulePosition(int place)
        {
            float k = Mathf.Min((float) Screen.width / 1920f, (float) Screen.height / 1080f);
            float blockDistance = (GameConstants.ModuleSize + GameConstants.ModuleDistance)
                * 100 * k;

            int ModulesMaxAmount = _machineBasementStaticData.ModulesMaxAmount;
            
            
            //Vector2 position = (Vector2) _machineBasementImage.transform.position - (Vector2) size / 2 * blockDistance
            Vector2 position = (Vector2) _machineBasementImage.transform.position;
            //Vector2 offset = (new Vector2(place.x, place.y) + Vector2.one / 2) * blockDistance;
            Vector2 offset = _machineBasementStaticData.ModulePlacementInfos[place].Offset * blockDistance;

            offset = Rotate(offset, -90f);
            Vector2 positionPlusOffset = position + offset;
            
            return positionPlusOffset;
        }
        public Vector2 Rotate(Vector2 v, float angle)
        {
            float rad = angle * Mathf.Deg2Rad; // Преобразуем угол в радианы
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            // Применяем матрицу поворота
            float newX = v.x * cos - v.y * sin;
            float newY = v.x * sin + v.y * cos;

            return new Vector2(newX, newY);
        }

        public void PresentModulesData()
        {
            foreach (var presenter in _modulePresenters)
            {
                presenter.SetBackgroundVisibility(false);
                presenter.SetDragDropActive(false);
                presenter.SetModuleVisual(null, "");
                presenter.DragDrop.ReturnToPosition();
            }
            foreach (ModuleSerializableInfo serializableInfo in _model.MachineModules)
            {
                MachineModuleStaticData staticData = _staticDataService.ForMachineModule(serializableInfo.Id);
                MachineModuleSlotPresenter instance = _modulePresenters[serializableInfo.Place];
                
                
                ModulePresentableInfo modulePresentableInfo = new ModulePresentableInfo
                {
                    ModuleStaticData = _staticDataService.ForMachineModule(serializableInfo.Id),
                    Level = serializableInfo.Level,
                    Place = serializableInfo.Place
                };
                instance.SetData(modulePresentableInfo);
                var FacingDirection = _machineBasementStaticData.ModulePlacementInfos
                    [serializableInfo.Place].Direction;

                instance.SetRotation(FacingDirection);
                instance.SetDragDropActive(true);
                instance.SetModuleVisual(staticData.Icon, staticData.Key);
            }
        }

        public void SetModulePresentersBackgroundActive(bool active)
        {
            foreach (MachineModuleSlotPresenter presenter in _modulePresenters)
            {
                presenter.SetBackgroundVisibility(active);
            }
        }

        private void CreateMachineModulePresenters(int size)
        {
            if (_modulePresenters != null)
            {
                foreach (var VARIABLE in _modulePresenters)
                {
                    Destroy(VARIABLE.gameObject);
                }
                foreach (var VARIABLE in GetComponentsInChildren<ModuleDragDrop>(true))
                {
                    Destroy(VARIABLE.gameObject);
                }
            }
            
            _modulePresenters = new MachineModuleSlotPresenter[size];
            for (int x = 0; x < size; x++)
            {
                MachineModuleSlotPresenter instance = Instantiate(moduleSlotPresenterPrefab, transform);
                instance.transform.SetParent(transform);
                _modulePresenters[x] = instance;
            }

            for (int place = 0; place < size; place++)
            {
                ModuleDragDrop dragDrop = Instantiate(_moduleDragDropPrefab, transform);

                MachineModuleSlotPresenter instance = _modulePresenters[place];

                instance.Construct(_model, place, dragDrop, _doubleClick,
                    _gameStopOverFocusManager, this, _progressService, _dragDropManager,
                    _machineBasementStaticData);
                
                var positionPlusOffset = CalculateModulePosition(place);
                instance.SetPosition(positionPlusOffset);
                RectTransform rectTransform = instance.GetComponent<RectTransform>();
                
                instance.DragDrop.BeginDrag += DragDropOnBeginDrag;
                instance.DragDrop.PointerUp += DragDropOnEndDrag;
                instance.ModulesSwapped += InstanceOnModulesSwapped;
                
            }
        }

        private void InstanceOnModulesSwapped(int to, int from)
        {

            _model.SwapModules(to, from);
            
            _gameStopOverFocusManager.SetFocusOnMachineModule(to, this, 
                _model.GetMachineModuleInfo(to).Level);
            PresentModulesData();
        }


        private void DragDropOnEndDrag()
        {
            //ModuleEndDrag?.Invoke();
            SetModulePresentersBackgroundActive(false);
        }


        private void DragDropOnBeginDrag()
        {
            //ModuleStartDrag?.Invoke();
            SetModulePresentersBackgroundActive(true);
        }

        private void PresentBasement()
        {
            _machineBasementImage.sprite =
                _machineBasementStaticData.Icon;
            _machineBasementImage.SetNativeSize();
        }

        public MachineModuleSlotPresenter GetPresenter(int place)
        {
            return _modulePresenters[place];
        }
    }
}