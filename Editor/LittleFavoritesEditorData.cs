using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace HaiitoCorp.LittleFavorites.Editor
{
    internal static class LittleFavoritesEditorData
    {
        private const string c_editorPrefsKey = "LittleFavoritesEditorKey";
        public static List<Object> Favorites { get; private set; } = new List<Object>();

        public static event UnityAction FavoritesChanged;

        #region Favorites
        public static void AddFavorites(Object[] favoriteObjects)
        {
            foreach (Object favoriteObject in favoriteObjects)
            {
                if(Favorites.Contains(favoriteObject)) return;
            
                Favorites.Add(favoriteObject);
            }
            
            Favorites.Sort((a,b) => String.Compare(a.name, b.name, StringComparison.CurrentCulture));
            
            SaveFavoritesToEditorPrefs();
            
            FavoritesChanged?.Invoke();
        }

        public static void RemoveFavorites(Object[] favoriteObjects)
        {
            foreach (Object favoriteObject in favoriteObjects)
            {
                if (!Favorites.Contains(favoriteObject))
                {
                    throw new Exception("Tried to remove a favorite that doesn't exist. This object is not a favorite");
                }
            
                Favorites.Remove(favoriteObject);
            }
            
            SaveFavoritesToEditorPrefs();
            
            FavoritesChanged?.Invoke();
        }
        
        #endregion
        
        #region Save and Load

        private static void SaveFavoritesToEditorPrefs()
        {
            List<string> guids = new List<string>();

            foreach (Object favorite in Favorites)
            {
                string favoritePath = AssetDatabase.GetAssetPath(favorite);
                guids.Add(AssetDatabase.AssetPathToGUID(favoritePath));
            }
            string favoritesGuidsString = JsonConvert.SerializeObject(guids);
            
            EditorPrefs.SetString(c_editorPrefsKey, favoritesGuidsString);
        }

        [DidReloadScripts]
        private static void LoadFavoritesFromEditorPrefs()
        {
            if(!EditorPrefs.HasKey(c_editorPrefsKey)) return;
            
            string favoritesGuidsString = EditorPrefs.GetString(c_editorPrefsKey);

            if(string.IsNullOrEmpty(favoritesGuidsString) || string.IsNullOrWhiteSpace(favoritesGuidsString)) return;

            List<string> guids = JsonConvert.DeserializeObject<List<string>>(favoritesGuidsString);
            
            Favorites.Clear();

            foreach (string guid in guids)
            {
                string favoritePath = AssetDatabase.GUIDToAssetPath(guid);
                Object favorite = AssetDatabase.LoadAssetAtPath<Object>(favoritePath);
                if(favorite == null)
                {
                    Debug.LogWarning("[LittleFavorites] Couldn't load object. Couldn't find object at that path.");
                    continue;
                }
                
                Favorites.Add(favorite);
            }
            
            FavoritesChanged?.Invoke();
        }

        #endregion
    }
}
