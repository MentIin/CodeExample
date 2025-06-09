using System.Collections.Generic;
using System.Linq;
using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Data.Stats;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Logic;
using Assets.CodeBase.StaticData;
using CodeBase.UI.Services;
using CodeBase.UI.Services.GameStopOver;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Presenters
{
    public class CraftModulePresenter : MonoBehaviour
    {
        public Transform OwningModuleContainer;
        [SerializeField] private Image _moduleIcon;
        [SerializeField] private Image _canBeCraftedIcon;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private ModuleBacklight _moduleBacklight;


        //[SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private PointerEventListener _moduleIconEventListener;

        private RectTransform _rectTransform;
        private MachineModuleStaticData _data;
        private SelectedDialog _selectedDialog;
        private DoubleClickService _doubleClick;
        private int _id;
        private GameStopOverFocusManager _gameStopOverFocusManager;
        private IPersistentProgressService _progressService;
        private int _level;

        public void Construct(MachineModuleStaticData data, int id, SelectedDialog selectedDialog,
            DoubleClickService doubleClickService, GameStopOverFocusManager gameStopOverFocusManager,
            IPersistentProgressService progressService, int level)
        {
            _level = level;
            _progressService = progressService;
            _rectTransform = GetComponent<RectTransform>();
            _moduleIcon.SetSpriteAndPivot(_rectTransform, data.Icon);
            _moduleBacklight.SetLevel(_level, 0.6f);
            _moduleIcon.SetNativeSize();
            _data = data;
            _selectedDialog = selectedDialog;
            _doubleClick = doubleClickService;
            _gameStopOverFocusManager = gameStopOverFocusManager;
            _id = id;

            SetTextVisibility(false);

            UpdateVisual();
            //SetColor();
        }

        private void UpdateVisual()
        {
            int amount=0;

            foreach (var owningModule in _progressService.Progress.OwningModules)
            {
                if (owningModule.Id == _data.Id && owningModule.Level == _level)
                {
                    amount++;
                }
            }
            if (amount == 0) _amountText.text = "";
            else _amountText.text = amount.ToString();

            if (_level == 1)
            {
                if (_progressService.Progress.EnoughMaterials(_data.Craft))
                {
                    if (_progressService.Progress.CountModules(_data.Id, _level) > 0) _canBeCraftedIcon.enabled = false;
                    else _canBeCraftedIcon.enabled = true;
                }else
                {
                    _canBeCraftedIcon.enabled = false;
                }
            }
            else
            {
                if (_progressService.Progress.CountModules(_data.Id, _level - 1) >= 2)
                {
                    if (_progressService.Progress.CountModules(_data.Id, _level) > 0) _canBeCraftedIcon.enabled = false;
                    else _canBeCraftedIcon.enabled = true;
                }else
                {
                    _canBeCraftedIcon.enabled = false;
                }
                
            }


            if (_progressService.Progress.CountModules(_data.Id, _level) > 0)
                _moduleBacklight.gameObject.SetActive(false);
            else _moduleBacklight.gameObject.SetActive(true);
            
            



        }

        private void OnPointerEnter(PointerEventData obj)
        {
            SetTextVisibility(true);
        }

        private void OnEnable()
        {
            //_progressService.Progress.ResourcesChanged += SetColor;
            
            _moduleIconEventListener.PointerDown += OnPointerDown;
            _moduleIconEventListener.PointerEnter += OnPointerEnter;
            _moduleIconEventListener.PointerExit += OnPointerExit;
        }

        private void OnDisable()
        {
            //_progressService.Progress.ResourcesChanged -= SetColor;
        }

        private void OnPointerExit(PointerEventData obj)
        {
            //SetTextVisibility(false);
        }


        private void SetColor()
        {
            if (_progressService.Progress.EnoughMaterials(_data.Craft)) _moduleIcon.color = Color.white;
            else _moduleIcon.color = new Color(0.3f, 0.2f, 0.2f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("PointerDown");
            _gameStopOverFocusManager.SetFocusOnCraftingModule(_id);
            if (_doubleClick.ClickAndCheck(this))
            {
                
            }
            
            _selectedDialog.SetData(_data, _level);
        }

        public List<SerializableStat> GetStats()
        {
            return _data.Stats;
        }

        private void SetTextVisibility(bool to)
        {
            //_text.transform.parent.gameObject.SetActive(to);
        }

        public Transform GetIconTransform()
        {
            return _moduleIcon.GetComponent<RectTransform>();
        }

        public MachineModuleStaticData GetData()
        {
            return _data;
        }
        public int GetLevel()
        {
            return _level;
        }
    }
}