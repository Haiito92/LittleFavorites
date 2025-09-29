using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HaiitoCorp.LittleFavorites.Editor
{
    public class LittleFavoritesTreeView : TreeView
    {
        private int _nextUId = 0;

        private List<Object> _favorites = new List<Object>();

        //int correspond to the id of the tree item "holding" the favorite.
        private Dictionary<int, Object> _favoritesDictionary = new Dictionary<int, Object>();
        
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

        public void AddFavorite(Object favorite)
        {
            if(_favorites.Contains(favorite)) return;
            
            _favorites.Add(favorite);
            
            Reload();
        }

        public void RemoveFavorite(Object favorite)
        {
            if(!_favorites.Contains(favorite)) return;
            
            _favorites.Remove(favorite);
            
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
        
        
    }
}
