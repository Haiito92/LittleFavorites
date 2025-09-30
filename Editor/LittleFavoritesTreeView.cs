using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HaiitoCorp.LittleFavorites.Editor
{
    public class LittleFavoritesTreeView : TreeView
    {
        private const string c_editorPrefsKey = "LittleFavoritesEditorKey";
        
        private int _nextUId = 0;

        private static List<Object> _favorites = new List<Object>();

        //int correspond to the id of the tree item "holding" the favorite.
        private Dictionary<int, Object> _favoritesDictionary = new Dictionary<int, Object>();

        private string _searchQuery;
        
        
        public LittleFavoritesTreeView(TreeViewState state) : base(state)
        {
            Reload();
        }

        public LittleFavoritesTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            Reload();
        }

        public void InitializeTree()
        {
            _favorites = new List<Object>();
            _favoritesDictionary = new Dictionary<int, Object>();
            
            LoadFavoritesFromEditorPrefs();
            
            Reload();
        }
        
        protected override TreeViewItem BuildRoot()
        {
            _nextUId = 0;
            
            TreeViewItem root = new TreeViewItem
            {
                id = _nextUId++, 
                depth = -1, 
                displayName = "Root", 
            };

            _favoritesDictionary.Clear();
            
            List<TreeViewItem> favoriteTreeViewItems = new List<TreeViewItem>
            {
                new TreeViewItem
                {
                    id = _nextUId++, 
                    depth = 0, 
                    displayName = "Favorites", 
                    icon = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D
                },
            };

            foreach (Object favorite in _favorites)
            {
                if (!string.IsNullOrEmpty(_searchQuery) && !favorite.name.ToLower().Contains(_searchQuery.ToLower()))
                {
                    continue;
                }
                
                TreeViewItem item = new TreeViewItem
                {
                    id = _nextUId++,
                    depth = 1, 
                    displayName = favorite.name,
                    icon = EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D
                };
                favoriteTreeViewItems.Add(item);
                
                _favoritesDictionary.Add(item.id, favorite);
            }
            
            SetupParentsAndChildrenFromDepths(root, favoriteTreeViewItems);
            
            return root;
        }
        
        #region Favorites List

        public void AddDraggedObjects(Object[] draggedObjects)
        {
            foreach (Object draggedObject in draggedObjects)
            {
                if(_favorites.Contains(draggedObject)) return;
            
                _favorites.Add(draggedObject);
            }
            
            
            _favorites.Sort((a,b) => String.Compare(a.name, b.name, StringComparison.CurrentCulture));
            
            SaveFavoritesToEditorPrefs();
            
            Reload();
        }

        public void RemoveSelection()
        {
            IList<int> selectedIDs = GetSelection();

            foreach (int selectedID in selectedIDs)
            {
                if(!_favoritesDictionary.ContainsKey(selectedID))
                {
                    throw new KeyNotFoundException($"FavoritesDictionary key list doesn't contain this Id");
                }
            
                _favorites.Remove(_favoritesDictionary[selectedID]);
            }
            
            SaveFavoritesToEditorPrefs();
            
            Reload();
        }
        #endregion

        #region User Input Handling

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);

            if(id <= 1) return; // Not take into account the root and the favorites "folder".
            
            Selection.activeObject = _favoritesDictionary[id];
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);

            if(id <= 1) return; // Not take into account the root and the favorites "folder".
            
            AssetDatabase.OpenAsset(_favoritesDictionary[id]);
        }

        #endregion

        #region Search Query

        public void SetSearchQuery(string searchQuery)
        {
            if(_searchQuery == searchQuery) return;
            
            _searchQuery = searchQuery;
            
            Reload();
        }

        #endregion

        #region Save and Load

        private void SaveFavoritesToEditorPrefs()
        {
            List<string> guids = new List<string>();

            foreach (Object favorite in _favorites)
            {
                string favoritePath = AssetDatabase.GetAssetPath(favorite);
                guids.Add(AssetDatabase.AssetPathToGUID(favoritePath));
            }
            string favoritesGuidsString = JsonConvert.SerializeObject(guids);
            
            EditorPrefs.SetString(c_editorPrefsKey, favoritesGuidsString);
        }

        private void LoadFavoritesFromEditorPrefs()
        {
            if(!EditorPrefs.HasKey(c_editorPrefsKey)) return;
            
            string favoritesGuidsString = EditorPrefs.GetString(c_editorPrefsKey);

            if(string.IsNullOrEmpty(favoritesGuidsString) || string.IsNullOrWhiteSpace(favoritesGuidsString)) return;

            List<string> guids = JsonConvert.DeserializeObject<List<string>>(favoritesGuidsString);
            
            _favorites.Clear();

            foreach (string guid in guids)
            {
                string favoritePath = AssetDatabase.GUIDToAssetPath(guid);
                Object favorite = AssetDatabase.LoadAssetAtPath<Object>(favoritePath);
                if(favorite == null)
                {
                    Debug.LogWarning("[LittleFavorites] Couldn't load object. Couldn't find object at that path.");
                    continue;
                }
                
                _favorites.Add(favorite);
            }
        }

        #endregion
    }
}
