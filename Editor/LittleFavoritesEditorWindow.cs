using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HaiitoCorp.LittleFavorites.Editor
{
    public class LittleFavoritesEditorWindow : EditorWindow
    {
        #region Fields
        // Search
        private SearchField _searchField;
        private string _searchQuery = "";
        
        // Tree View
        private TreeViewState _favoritesTreeViewState;
        private LittleFavoritesTreeView _favoritesTreeView;
        #endregion
        
        [MenuItem("Tools/HaiitoCorp/LittleFavorites")]
        public static void OpenWindow()
        {
            GetWindow<LittleFavoritesEditorWindow>("Little Favorites");
        }

        private void OnEnable()
        {
            _searchField = new SearchField();

            _favoritesTreeViewState ??= new TreeViewState();

            _favoritesTreeView = new LittleFavoritesTreeView(_favoritesTreeViewState);
        }

        private void OnGUI()
        {
            _searchQuery = _searchField.OnGUI(_searchQuery);
            _favoritesTreeView.SetSearchQuery(_searchQuery);
            
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 0.0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            _favoritesTreeView.OnGUI(dropArea);

            Event evt = Event.current;
            
            switch (evt.type)
            {
                case EventType.KeyDown:
                    switch (evt.keyCode)
                    {
                        case KeyCode.Delete:
                            _favoritesTreeView.RemoveSelection();
                            evt.Use();
                            break;
                    }
                    break;
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                    if(!dropArea.Contains(evt.mousePosition)) break;

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        _favoritesTreeView.AddFavorite(draggedObject);
                    }
                    evt.Use();
                    break;
            }
        }
    }
}
