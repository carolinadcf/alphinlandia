using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Proyecto3.Managers.MenuManager
{
    public class MenuEventSystemHandler : MonoBehaviour
    {
        [Header("References")]
        public List<Selectable> menuSelectables = new List<Selectable>();
        [SerializeField] protected Selectable _firstSelected;

        [Header("Controls")]
        [SerializeField] protected InputActionReference _navigateReference;

        [Header("Animations")]
        [SerializeField] protected float _selectedAnimationScale = 1.1f;
        [SerializeField] protected float _scaleDuration = 0.25f;
        [SerializeField] protected List<GameObject> _animationExclusions = new List<GameObject>();

        [Header("Sounds")]
        [SerializeField] protected UnityEvent SoundEvent;

        protected Dictionary<Selectable, Vector3> _originalScales = new Dictionary<Selectable, Vector3>();

        protected Selectable _lastSelected;

        protected Tween _scaleUpTween;
        protected Tween _scaleDownTween;

        public virtual void Awake()
        {
            // store original scales and add listeners
            foreach (Selectable selectable in menuSelectables)
            {
                _originalScales.Add(selectable, selectable.transform.localScale);
                AddSelectionListeners(selectable);
            }
        }

        public virtual void OnEnable()
        {
            _navigateReference.action.performed += OnNavigate;

            //  ensure all selectables are reset back to original scale
            for (int i = 0; i < menuSelectables.Count; i++)
            {
                Selectable selectable = menuSelectables[i];
                selectable.transform.localScale = _originalScales[selectable];
            }

            StartCoroutine(SelectAfterDelay());
        }

        protected virtual IEnumerator SelectAfterDelay()
        {
            // wait for end of frame to ensure event system is ready
            yield return null;

            // select first selected
            EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
        }

        public virtual void OnDisable()
        {
            _navigateReference.action.performed -= OnNavigate;
            // kill any active tweens
            _scaleUpTween?.Kill(true);
            _scaleDownTween?.Kill(true);
        }

        protected virtual void AddSelectionListeners(Selectable selectable)
        {
            // add listener
            EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = selectable.gameObject.AddComponent<EventTrigger>();
            }

            // add select event
            EventTrigger.Entry SelectEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select
            };
            SelectEntry.callback.AddListener(OnSelect);
            trigger.triggers.Add(SelectEntry);

            // add deselect event
            EventTrigger.Entry DeselectEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Deselect
            };
            DeselectEntry.callback.AddListener(OnDeselect);
            trigger.triggers.Add(DeselectEntry);

            // add pointer enter event
            EventTrigger.Entry PointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            PointerEnterEntry.callback.AddListener(OnPointerEnter);
            trigger.triggers.Add(PointerEnterEntry);

            // add pointer exit event
            EventTrigger.Entry PointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            PointerExitEntry.callback.AddListener(OnPointerExit);
            trigger.triggers.Add(PointerExitEntry);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SoundEvent?.Invoke();
            _lastSelected = eventData.selectedObject.GetComponent<Selectable>();

            if (_animationExclusions.Contains(eventData.selectedObject))
                return;

            Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;
            _scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, _scaleDuration);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (_animationExclusions.Contains(eventData.selectedObject))
                return;

            Selectable sel = eventData.selectedObject.GetComponent<Selectable>();
            _scaleDownTween = eventData.selectedObject.transform.DOScale(_originalScales[sel], _scaleDuration);
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (pointerEventData != null)
            {
                // pointerEventData.selectedObject = pointerEventData.pointerEnter;
                Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
                if (sel == null)
                {
                    sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
                }
                pointerEventData.selectedObject = sel.gameObject;
            }
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (pointerEventData != null)
            {
                pointerEventData.selectedObject = null;
            }
        }

        protected virtual void OnNavigate(InputAction.CallbackContext context)
        {
            if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
            }
        }
    }
}