using CodeBase.Infrastructure.Data.Modules;
using CodeBase.UI.Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeBase.UI.Elements.Presenters
{
    public class OwningModulePresenter : MonoBehaviour
    {
        public ModuleDragDrop ModuleDragDrop;
        [HideInInspector] public ModulePresentableInfo modulePresentableInfo;
        
        private int _id;
        private DoubleClickService _doubleClick;
        private GameStopOverFocusManager _gameStopOverFocusManager;
        private SelectedDialog _selectedDialog;
        private ModulePresentableInfo _moduleInfo;

        public void Construct(ModulePresentableInfo moduleInfo, int id, 
            DoubleClickService doubleClickService,
            GameStopOverFocusManager gameStopOverFocusManager,
            DragDropManager dragDropmanager,
            SelectedDialog selectedDialog)
        {
            _moduleInfo = moduleInfo;
            _selectedDialog = selectedDialog;
            ModuleDragDrop.OwningModule = true;
            ModuleDragDrop.ModulePresentableInfo = moduleInfo;
            ModuleDragDrop.Construct(dragDropmanager);
            ModuleDragDrop.SetVisual(moduleInfo.ModuleStaticData.Icon, moduleInfo.ModuleStaticData.Key);
            ModuleDragDrop.SetPosition(transform);
            ModuleDragDrop.PointerDown += ModuleDragDropOnPointerDown;
            ModuleDragDrop.Place = id;
            
            modulePresentableInfo = moduleInfo;
            _id = id;
            _doubleClick = doubleClickService;
            _gameStopOverFocusManager = gameStopOverFocusManager;
        }

        private void ModuleDragDropOnPointerDown()
        {
            _doubleClick.ClickAndCheck(this);
            _selectedDialog.SetData(_moduleInfo.ModuleStaticData, _moduleInfo.Level);
            _gameStopOverFocusManager.SetFocusOnOwningModule(_id);
        }
    }
}