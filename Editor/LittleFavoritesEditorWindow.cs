using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

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
            LittleFavoritesEditorWindow window = GetWindow<LittleFavoritesEditorWindow>("Little Favorites");
        }

        private void OnEnable()
        {
            Texture2D windowIconTexture = Resources.Load("StarIcon") as Texture2D;
            titleContent = new GUIContent("Little Favorites", windowIconTexture);
            
            _searchField = new SearchField();

            _favoritesTreeViewState ??= new TreeViewState();
            _favoritesTreeView = new LittleFavoritesTreeView(_favoritesTreeViewState);
            _favoritesTreeView.InitializeFavoritesTree();
        }

        private void OnDestroy()
        {
            _favoritesTreeView.FinalizeFavoritesTree();
        }

        private void OnGUI()
        {
            _searchQuery = _searchField.OnGUI(_searchQuery);
            _favoritesTreeView.SetSearchQuery(_searchQuery);
            
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 0.0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            _favoritesTreeView.OnGUI(dropArea);
            
            Event evt = Event.current;
            if(evt == null) return;
            
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
            }
        }
    }
}
