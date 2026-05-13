using System.Collections.Generic;
using Game.UI.Model;
using R3;

namespace Game.UI.Service
{
    public interface IMapSelectionService
    {
        MapModel SelectedMap { get; }
        IReadOnlyList<MapModel> Maps { get; }
        MapModel GetSelectedMap();
        void Select(string mapId);
    }
}
