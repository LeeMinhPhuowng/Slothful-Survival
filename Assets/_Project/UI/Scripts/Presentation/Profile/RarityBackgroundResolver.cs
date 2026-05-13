using Game.UI.Data;
using UnityEngine;

namespace Game.UI.Presentation.Profile
{
    public static class RarityBackgroundResolver
    {
        public static Sprite Resolve(RarityBackgroundEntry[] backgrounds, RarityItem rarity)
        {
            if (backgrounds == null)
            {
                return null;
            }

            foreach (RarityBackgroundEntry entry in backgrounds)
            {
                if (entry != null && entry.Rarity == rarity)
                {
                    return entry.Background;
                }
            }

            return null;
        }
    }
}
