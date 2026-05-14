using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomView
{
    /// <summary>
    /// Returns the list of door tile offsets relative to the wall edge.
    /// Even room size → 2-tile door, Odd room size → 3-tile door.
    /// </summary>
    private static int[] GetDoorOffsets(int roomSize)
    {
        int center = roomSize / 2;
        if (roomSize % 2 == 0)
        {
            return new int[] { center - 1, center };
        }
        else
        {
            return new int[] { center - 1, center, center + 1 };
        }
    }

    private static bool IsDoorTile(int localCoord, int[] doorOffsets)
    {
        foreach (int offset in doorOffsets)
        {
            if (localCoord == offset) return true;
        }
        return false;
    }

    public static void Draw(RoomEntity room, Tilemap groundTilemap, Tilemap wallTilemap)
    {
        var config = room.Config;
        if (config == null) return;

        int size = config.RoomSize;
        Vector2Int gridPos = room.GetPosition();
        
        // Offset so rooms don't overlap. Each room takes size x size grid cells.
        int startX = gridPos.x * size;
        int startY = gridPos.y * size;
        int[] doorOffsets = GetDoorOffsets(size);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector3Int tilePos = new Vector3Int(startX + x, startY + y, 0);

                bool isWall = (x == 0 || x == size - 1 || y == 0 || y == size - 1);
                
                // Carve doors if there is a connection in that direction
                if (isWall)
                {
                    // Top wall - door opens when connected up
                    if (y == size - 1 && IsDoorTile(x, doorOffsets) && room.GetNeighbours().ContainsKey(Vector2Int.up))
                        isWall = false;
                    // Bottom wall
                    if (y == 0 && IsDoorTile(x, doorOffsets) && room.GetNeighbours().ContainsKey(Vector2Int.down))
                        isWall = false;
                    // Right wall
                    if (x == size - 1 && IsDoorTile(y, doorOffsets) && room.GetNeighbours().ContainsKey(Vector2Int.right))
                        isWall = false;
                    // Left wall
                    if (x == 0 && IsDoorTile(y, doorOffsets) && room.GetNeighbours().ContainsKey(Vector2Int.left))
                        isWall = false;
                }

                if (isWall)
                {
                    if (wallTilemap != null)
                    {
                        Tile wallTile = null;

                        // Corners
                        if (x == 0 && y == 0) wallTile = config.BottomLeftCornerWall;
                        else if (x == 0 && y == size - 1) wallTile = config.UpLeftCornerWall;
                        else if (x == size - 1 && y == 0) wallTile = config.BottomRightCornerWall;
                        else if (x == size - 1 && y == size - 1) wallTile = config.UpRightCornerWall;
                        // Edges
                        else if (x == 0) wallTile = config.LeftWall;
                        else if (x == size - 1) wallTile = config.RightWall;
                        else if (y == 0) wallTile = config.BottomWall;
                        else if (y == size - 1) wallTile = config.TopWall;

                        if (wallTile != null)
                        {
                            wallTilemap.SetTile(tilePos, wallTile);
                        }
                    }
                }
                else
                {
                    if (groundTilemap != null && config.Ground != null)
                    {
                        groundTilemap.SetTile(tilePos, config.Ground);
                        
                        // Tint the ground color based on room type
                        Color roomColor = Color.white;
                        switch (room.GetRoomType())
                        {
                            case RoomType.Boss: roomColor = new Color(1f, 0.7f, 0.7f); break; // Light Red
                            case RoomType.Chest: roomColor = new Color(1f, 1f, 0.7f); break; // Light Yellow
                            case RoomType.Shop: roomColor = new Color(0.7f, 1f, 0.7f); break; // Light Green
                        }
                        
                        // Start room is Light Cyan
                        if (room.GetRoomType() == RoomType.Start) roomColor = new Color(0.7f, 1f, 1f); 
                        
                        if (roomColor != Color.white)
                        {
                            groundTilemap.SetTileFlags(tilePos, TileFlags.None);
                            groundTilemap.SetColor(tilePos, roomColor);
                        }
                    }
                }
            }
        }
    }
}
