namespace Game.UI.Data
{
    public readonly struct NormalAttackRequestedPayload
    {
        public readonly string CharacterId;
        public readonly string WeaponId;
        public readonly int RequestFrame;

        public NormalAttackRequestedPayload(string characterId, string weaponId, int requestFrame)
        {
            CharacterId = characterId;
            WeaponId = weaponId;
            RequestFrame = requestFrame;
        }
    }
}