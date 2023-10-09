using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameCore
{
    namespace GUI
    {
        [RequireComponent(typeof(UiSelecter))]
        public class SelectionListener : MonoBehaviour, ISelectHandler
        {
            [Inject] private readonly UiSelectionService _submitService;
            private UiSelecter _uiSubmitter;

            private void Awake()
            {
                _uiSubmitter = gameObject.GetComponent<UiSelecter>();
            }

            public void OnSelect(BaseEventData eventData)
            {
                _submitService.SoftSelect(_uiSubmitter);
            }
        }
    }
}
