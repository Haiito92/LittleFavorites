using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HaiitoCorp.LittleFavorites.Editor
{
    public class LittleFavoritesTreeView : TreeView
    {
        private int _nextUId = 0;

        private List<Object> _favorites = new List<Object>();
        
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
    }
}
