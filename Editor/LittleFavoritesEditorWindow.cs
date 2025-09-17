using System;
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
        private LittleFavoritesTreeView _favoritesTreeView;
        private TreeViewState _favoritesTreeViewState;
        #endregion
        
        [MenuItem("Tools/HaiitoCorp/LittleFavorites")]
        public static void OpenWindow()
        {
            GetWindow<LittleFavoritesEditorWindow>("Little Favorites");
        }

        private void OnEnable()
        {
            _searchField = new SearchField();

            if (_favoritesTreeViewState == null) _favoritesTreeViewState = new TreeViewState();

            _favoritesTreeView = new LittleFavoritesTreeView(_favoritesTreeViewState);
        }

        private void OnGUI()
        {
            _searchQuery = _searchField.OnGUI(_searchQuery);
            
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 0.0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            _favoritesTreeView.OnGUI(dropArea);

            Event evt = Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if(!dropArea.Contains(evt.mousePosition)) break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.AcceptDrag();
                    evt.Use();
                    break;
                case EventType.DragExited:
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                    }
                    evt.Use();
                    break;
            }
        }
    }
}
