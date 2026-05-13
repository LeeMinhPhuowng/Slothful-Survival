using System;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Model;
using R3;

namespace Game.UI.Service
{
    public sealed class MapSelectionService : IMapSelectionService
    {
        private readonly IPlayerProgressService _progressService;
        private MapModel _selectedMap;

        public MapModel SelectedMap => _selectedMap;
        public IReadOnlyList<MapModel> Maps { get; }

        public MapSelectionService(IGameCatalog gameCatalog, IPlayerProgressService progressService)
        {
            _progressService = progressService;
            Maps = gameCatalog.GetAllMaps();

            foreach (MapModel map in Maps)
            {
                map.IsUnlocked = _progressService.IsMapUnlocked(map.MapId, map.IsUnlocked);
            }

            string selectedMapId = _progressService.GetSelectedMapId();
            _selectedMap = Maps.FirstOrDefault(map => map.MapId == selectedMapId && map.IsUnlocked)
                           ?? Maps.FirstOrDefault(map => map.IsUnlocked)
                           ?? Maps.FirstOrDefault();
        }

        public void Select(string mapId)
        {
            MapModel map = Maps.FirstOrDefault(item => item.MapId == mapId);
            if (map != null && map.IsUnlocked)
            {
                _selectedMap = map;
                _progressService.SetSelectedMap(map.MapId);
            }
        }

        public MapModel GetSelectedMap()
        {
            return _selectedMap;
        }
    }
}
