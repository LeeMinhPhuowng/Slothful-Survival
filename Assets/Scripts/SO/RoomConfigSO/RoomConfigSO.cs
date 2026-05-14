using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Room Config", menuName = "Room Config")]
public class RoomConfigSO : ScriptableObject
{
    [SerializeField] private RoomType roomType;
    [SerializeField] private int roomSize;

    [SerializeField] private Tile ground;
    [SerializeField] private Tile leftWall;
    [SerializeField] private Tile rightWall;
    [SerializeField] private Tile topWall;
    [SerializeField] private Tile bottomWall;
    [SerializeField] private Tile upLeftCornerWall;
    [SerializeField] private Tile upRightCornerWall;
    [SerializeField] private Tile bottomLeftCornerWall;
    [SerializeField] private Tile bottomRightCornerWall;

    public RoomType RoomType => roomType;
    public int RoomSize => roomSize;
    public Tile Ground => ground;
    public Tile LeftWall => leftWall;
    public Tile RightWall => rightWall;
    public Tile TopWall => topWall;
    public Tile BottomWall => bottomWall;
    public Tile UpLeftCornerWall => upLeftCornerWall;
    public Tile UpRightCornerWall => upRightCornerWall;
    public Tile BottomLeftCornerWall => bottomLeftCornerWall;
    public Tile BottomRightCornerWall => bottomRightCornerWall;
}

public enum RoomType
{
    Start,
    Default,
    Chest,
    Shop,
    Boss
}
