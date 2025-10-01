using System.Collections.Generic;
using System.Linq;
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

            List<TreeViewItem> favoriteTreeViewItems = new List<TreeViewItem>();

            foreach (Object favorite in LittleFavoritesEditorData.Favorites)
            {
                if (!string.IsNullOrEmpty(_searchQuery) && !favorite.name.ToLower().Contains(_searchQuery.ToLower()))
                {
                    continue;
                }
                
                TreeViewItem item = new TreeViewItem
                {
                    id = _nextUId++,
                    depth = 0, 
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

            if(id <= 0) return; // Not take into account the root.
            
            Selection.activeObject = _favoritesDictionary[id];
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);

            if(id <= 0) return; // Not take into account the root.
            
            AssetDatabase.OpenAsset(_favoritesDictionary[id]);
        }
        #endregion

        #region Drag And Drop Handling

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return args.draggedItemIDs.Count > 0;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();

            Object[] draggedObjects = new Object[args.draggedItemIDs.Count];
            for (int i = 0; i < draggedObjects.Length; i++)
            {
                draggedObjects[i] = _favoritesDictionary[args.draggedItemIDs[i]];
            }

            DragAndDrop.objectReferences = draggedObjects;
            DragAndDrop.SetGenericData("LittleFavoritesTreeView", this);
            DragAndDrop.StartDrag("Start Favorites Drag.");
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            //Add logic to check on what we are above and change mouse icon depending on it.
            //Only above this tree.
            
            if (args.performDrop)
            {
                object origin = DragAndDrop.GetGenericData("LittleFavoritesTreeView");
                if (origin == this)
                {
                    //Detect object that were moved
                    //Check whats their new id
                    //Call LittleFavoritesEditorData function to move around asset.
                    switch (args.dragAndDropPosition)
                    {
                        case DragAndDropPosition.BetweenItems:
                            LittleFavoritesEditorData.MoveFavoritesAtIndex(DragAndDrop.objectReferences,args.insertAtIndex);
                            SetSelection(new List<int>(), TreeViewSelectionOptions.None);
                            break;
                    }
                    
                    DragAndDrop.SetGenericData("LittleFavoritesTreeView", null);
                }
                else
                {
                    Debug.Log("Drag from another window");

                    LittleFavoritesEditorData.AddFavorites(DragAndDrop.objectReferences);
                }                
            }
            
            return DragAndDropVisualMode.Move;
        }
        #endregion
    }
}
