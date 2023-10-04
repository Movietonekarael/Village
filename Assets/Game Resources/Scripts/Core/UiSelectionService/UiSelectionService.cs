using UnityEngine;

namespace GameCore
{
    namespace GUI
    {
        public class UiSelectionService : MonoBehaviour 
        {
            private ISelectable _currentSelected;
            public ISelectable CurrentSelected
            {
                get
                {
                    return _currentSelected;
                }
                set
                {
                    _currentSelected?.Deselect();
                    _currentSelected = value;
                    if (_selectionEnabled)
                        _currentSelected?.Select();
                }
            }

            private bool _selectionEnabled = true;
            public bool SelectionEnabled
            {
                get
                {
                    return _selectionEnabled;
                }
                set
                {
                    _selectionEnabled = value;
                    EnableSelection(_selectionEnabled);
                }
            }

            private void EnableSelection(bool enabled)
            {
                if (enabled)
                    EnableSelection();
                else 
                    DisableSelection();
            }

            private void EnableSelection()
            {
                _currentSelected?.Select();
            }

            private void DisableSelection() 
            {
                _currentSelected?.Deselect();
            }

            public void SoftSelect(ISelectable newSubmit)
            {
                _currentSelected = newSubmit;
            }
        }
    }
}
