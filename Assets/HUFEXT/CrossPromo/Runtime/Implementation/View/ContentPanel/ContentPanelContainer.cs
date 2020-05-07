using System.Collections.Generic;
using System.Linq;
using HUFEXT.CrossPromo.Runtime.Implementation.Model;
using HUFEXT.CrossPromo.Runtime.Implementation.View.CrossPromoTile;
using UnityEngine;

namespace HUFEXT.CrossPromo.Runtime.Implementation.View.ContentPanel
{
    public class ContentPanelContainer : MonoBehaviour
    {
        [SerializeField] RectTransform self = default;
        [SerializeField] ContentPanelScrollView content = default;

        readonly Dictionary<string, TileModel> tiles = new Dictionary<string, TileModel>();
        readonly List<TileViewModel> tileViews = new List<TileViewModel>();

        public void AddOrUpdateGame(TileModel tileModel)
        {
            if (tiles.ContainsKey(tileModel.Uuid))
            {
                tiles[tileModel.Uuid] = tileModel;
            }
            else
            {
                var newTile = new TileViewModel(tileModel, content.transform);
                tiles.Add(tileModel.Uuid, tileModel);
                tileViews.Add(newTile);
            }
        }

        public void UpdateOrder()
        {
            var sortedModels = tiles
                .Select(a => a.Value)
                .OrderBy(a => a.Priority)
                .ToList();
            
            for (int i = 0; i < tileViews.Count; i++)
            {
                tileViews[i].UpdateView(sortedModels[i]);
            }
        }

        public void RemoveObsoleteTiles(List<TileModel> newTiles)
        {
            foreach (var tileKey in tiles.Keys.ToList())
            {
                var found = newTiles.Any(newTile => newTile.Uuid == tileKey);
                if (!found)
                {
                    tiles.Remove(tileKey);
                }
            }

            var tilesCount = tiles.Count;
            for (int i = tilesCount; i < tileViews.Count; i++)
            {
                tileViews[i].DestroyView();
            }

            tileViews.RemoveRange(tilesCount, tileViews.Count - tilesCount);
        }

        public void UpdateTexts()
        {
            foreach (var tileView in tileViews)
            {
                tileView.UpdateTexts();
            }
        }
    }
}