using System;
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

        private List<Object> _favorites = new List<Object>();

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

        protected override TreeViewItem BuildRoot()
        {
            _nextUId = 0;
            
            TreeViewItem root = new TreeViewItem { id = _nextUId++, depth = -1, displayName = "Root" };

            _favoritesDictionary.Clear();
            
            List<TreeViewItem> favoriteTreeViewItems = new List<TreeViewItem>
            {
                new TreeViewItem { id = _nextUId++, depth = 0, displayName = "Favorites"},
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
            Reload();
        }

        public void RemoveSelection()
        {
            IList<int> selectedIDs = GetSelection();

            foreach (int selectedID in selectedIDs)
            {
                if(!_favoritesDictionary.ContainsKey(selectedID))
                {
                    throw new IndexOutOfRangeException($"FavoritesDictionary key list doesn't contain this Id");
                }
            
                _favorites.Remove(_favoritesDictionary[selectedID]);
            }
            
            Reload();
        }
        #endregion

        #region User Input Handling

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);

            Selection.activeObject = _favoritesDictionary[id];
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);

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
    }
}
