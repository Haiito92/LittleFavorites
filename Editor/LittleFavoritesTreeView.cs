using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace HaiitoCorp.LittleFavorites.Editor
{
    public class LittleFavoritesTreeView : TreeView
    {
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
            TreeViewItem root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            
            
            List<TreeViewItem> baseItems = new List<TreeViewItem>
            {
                new TreeViewItem { id = 1, depth = 0, displayName = "Favorites" },
            };
            
            SetupParentsAndChildrenFromDepths(root, baseItems);
            
            return root;
        }
    }
}
