using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HaiitoCorp.LittleFavorites.Editor
{
    public class LittleFavoritesTreeView : TreeView
    {
        private int _nextUId = 0;
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

        public void InitializeFavoritesTree()
        {
            LittleFavoritesEditorData.FavoritesChanged += OnFavoritesChanged;
        }
        
        public void FinalizeFavoritesTree()
        {
            LittleFavoritesEditorData.FavoritesChanged -= OnFavoritesChanged;
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

            foreach (Object favorite in LittleFavoritesEditorData.Favorites)
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
                    icon = EditorGUIUtility.ObjectContent(favorite, favorite.GetType()).image as Texture2D
                };
                favoriteTreeViewItems.Add(item);
                
                _favoritesDictionary.Add(item.id, favorite);
            }
            
            SetupParentsAndChildrenFromDepths(root, favoriteTreeViewItems);
            
            return root;
        }
        
        #region Favorites List

        public Object[] GetSelectedObjects()
        {
            IList<int> selectedIDs = GetSelection();
            Object[] selectedFavoriteObjects = new Object[selectedIDs.Count];

            for (int i = 0; i < selectedIDs.Count; i++)
            {
                if(!_favoritesDictionary.ContainsKey(selectedIDs[i]))
                {
                    throw new KeyNotFoundException($"FavoritesDictionary key list doesn't contain this Id");
                }

                selectedFavoriteObjects[i] = _favoritesDictionary[selectedIDs[i]];
            }

            return selectedFavoriteObjects;
        }

        public void RemoveSelection()
        {
            LittleFavoritesEditorData.RemoveFavorites(GetSelectedObjects());
            SetSelection(new List<int>(), TreeViewSelectionOptions.None);
        }
        
        private void OnFavoritesChanged()
        {
            Reload();
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

        #region Drag And Drop Handling
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            //Here add more logic to drop in the future (handling of type of objects, folders, etc...)
            
            if (args.performDrop)
            {
                LittleFavoritesEditorData.AddFavorites(DragAndDrop.objectReferences);
            }
            
            return DragAndDropVisualMode.Move;
        }
        #endregion
    }
}
