using Godot;
using System.Xml;
using System.Collections.Generic;

public class Map : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Dictionary<string,TileMap> layers;

    public Resource tmx;
    int tileWidth;
    int tileHeight;
    Vector2 tileSize;
    int width;
    int height;

    private PackedScene shovelItemScene;
    private PackedScene coinItemScene;
    private PackedScene enemyScene;
    private PackedScene spikeScene;

    static string ResolvePath(string path)
    {
        List<string> partsList = new List<string>(path.Split(new char[]{'/'}, System.StringSplitOptions.None));
        for (int i = 1; i < partsList.Count; i++) {
            if (partsList[i] == "..") {
                i--;
                partsList.RemoveAt(i);
                partsList.RemoveAt(i);
                i--;
            }
        }
        return string.Join("/", partsList);
    }

    public override string ToString() {
        return $"Tilemap {width}x{height} with {layers.Count} layers.";
    }

    public int GetTileAt(Vector2 pos)
    {
        TileMap fgMap = layers["FG"];
        return fgMap.GetCellv(new Vector2(pos.x / tileWidth, pos.y / tileHeight));
    }
    public EarthTileType GetEarthTileAt(Vector2 pos)
    {
        HashSet<int> easyIndices = new HashSet<int>(new int[]{32, 33, 34, 35, 36});
        HashSet<int> mediumIndices = new HashSet<int>(new int[]{48, 49, 50, 51, 52});
        HashSet<int> hardIndices = new HashSet<int>(new int[]{64, 65, 66, 67, 68});
        HashSet<int> ultraIndices = new HashSet<int>(new int[]{80, 81, 82, 83, 84});

        int tileIndex = GetTileAt(pos);
        if (tileIndex == -1) {
            return EarthTileType.Empty;
        }
        if (easyIndices.Contains(tileIndex)) {
            return EarthTileType.Easy;
        }
        if (mediumIndices.Contains(tileIndex)) {
            return EarthTileType.Medium;
        }
        if (hardIndices.Contains(tileIndex)) {
            return EarthTileType.Hard;
        }
        if (hardIndices.Contains(tileIndex)) {
            return EarthTileType.Ultra;
        }
        return EarthTileType.Unknown;
    }

    public void ClearEarthTileAt(Vector2 pos)
    {
        TileMap fgMap = layers["FG"];
        fgMap.SetCellv(new Vector2(pos.x / tileWidth, pos.y / tileHeight), -1);
    }

    public void LoadTMX(string filename)
    {
        layers = new Dictionary<string, TileMap>();
        File f = new File();
        string mapPath = "res://map/";
        string mapFilename = mapPath + filename;
        if (f.Open(mapFilename, File.ModeFlags.Read) == Error.Ok)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(f.GetAsText());
            XmlElement root = xmlDoc.DocumentElement;
            tileWidth = int.Parse(root.Attributes["tilewidth"].Value);
            tileHeight = int.Parse(root.Attributes["tileheight"].Value);
            tileSize = new Vector2(tileWidth, tileHeight);
            width = int.Parse(root.Attributes["width"].Value);
            height = int.Parse(root.Attributes["height"].Value);
            XmlNode tilesetNode = root.SelectSingleNode("tileset");
            string tilesetPath = tilesetNode["image"].Attributes["source"].Value;
            int firstgid = int.Parse(tilesetNode.Attributes["firstgid"].Value);
            int tilesetColumns = int.Parse(tilesetNode.Attributes["columns"].Value);
            int tilesetCount = int.Parse(tilesetNode.Attributes["tilecount"].Value);
            int tilesetWidth = int.Parse(tilesetNode["image"].Attributes["width"].Value);
            int tilesetHeight = int.Parse(tilesetNode["image"].Attributes["height"].Value);

            tilesetPath = System.IO.Path.Combine(mapPath, tilesetPath);
            GD.Print(tilesetPath);
            tilesetPath = ResolvePath(tilesetPath);
            GD.Print(tilesetPath);
            Texture tilesetTexture = GD.Load<Texture>(tilesetPath);
            GD.Print($"Loaded Tileset texture {tilesetPath} {tilesetTexture.GetWidth()}x{tilesetTexture.GetHeight()}");
            TileSet tileset = new TileSet();
            for (int i = 0; i < tilesetCount; i++) {
                tileset.CreateTile(i);
                tileset.TileSetTexture(i, tilesetTexture);
                tileset.TileSetTileMode(i, TileSet.TileMode.SingleTile);
                tileset.TileSetRegion(i, new Rect2((i % tilesetColumns) * tileWidth, (i / tilesetColumns) * tileHeight, tileWidth, tileHeight));
                //tileset.TileSetTextureOffset(i, new Vector2((i % tilesetColumns) * tileWidth, (i / tilesetColumns) * tileHeight));
                //tileset.AutotileSetSize(0, new Vector2(tileWidth, tileHeight));
                ConvexPolygonShape2D shape = new ConvexPolygonShape2D();
                shape.Points = new Vector2[]{
                    new Vector2(0, 0), new Vector2(16, 0),
                    new Vector2(16, 16), new Vector2(0, 16)
                };
                tileset.TileSetShape(i, 0, shape);
            }

            foreach (XmlNode layer in root.SelectNodes("layer"))
            {
                int layerWidth = int.Parse(layer.Attributes["width"].Value);
                int layerHeight = int.Parse(layer.Attributes["height"].Value);
                string layerName = layer.Attributes["name"].Value;
                string layerEncoding = layer["data"].Attributes["encoding"].Value;
                if (layerEncoding != "csv") {
                    GD.Print($"{mapFilename} layer {layerName} invalid encoding {layerEncoding}!");
                    continue;
                }
                TileMap map = new TileMap();//GetNode<TileMap>(layerName);
                map.CollisionLayer = 1<<1;
                map.CollisionMask = 0;
                map.CellSize = new Vector2(tileWidth, tileHeight);
                //if (map == null) continue;
                GD.Print($"Loading layer {layerName}.");
                string dataStr = layer["data"].InnerText;
                dataStr.Replace("\n", "");
                string[] dataStrCells = dataStr.Split(",");
                map.Name = layerName;
                map.TileSet = tileset;

                int tileCount = 0;
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        string dataStrCell = dataStrCells[y * width + x];
                        long tileId = long.Parse(dataStrCell);
                        tileId &= 0xffff;
                        tileId -= firstgid;
                        if (tileId >= 0) {
                            map.SetCell(x, y, (int)tileId, false, false, false);
                            tileCount++;
                        }
                    }
                }
                GD.Print($"Added {tileCount} tiles to layer {layerName}.");
                layers.Add(layerName, map);
                AddChild(map);
            }
        }
        else
        {
            GD.Print("Error loading map.tmx!");
        }
    }

    public void SpawnObjects(TileMap layer, ObjectType type, PackedScene objectScene)
    {
        Godot.Collections.Array cells = layer.GetUsedCellsById((int)type);
        foreach (object cell in cells) {
            Vector2 pos = (Vector2) cell;
            layer.SetCellv(pos, -1);
            var instance = objectScene.Instance<TMXObject>();
            instance.Position = pos * tileSize + tileSize * 0.5f;
            instance.Type = type;
            AddChild(instance as Node);
        }
    }

    public void SpawnPlayer(TileMap layer, ObjectType type)
    {
        Godot.Collections.Array cells = layer.GetUsedCellsById((int)type);
        Player ply = GetNode<Player>("Player");
        foreach (object cell in cells) {
            GD.Print("player cell found!");
            layer.SetCellv((Vector2) cell, -1);
            ply.Position = (Vector2) cell * tileSize + tileSize * 0.5f;
            ply.SpawnPosition = ply.Position;
            break;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        shovelItemScene = GD.Load<PackedScene>("res://ShovelItem.tscn");
        coinItemScene = GD.Load<PackedScene>("res://CoinItem.tscn");
        enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");
        spikeScene = GD.Load<PackedScene>("res://Spike.tscn");

        LoadTMX("map.tmx");
        SpawnObjects(layers["FG"], ObjectType.Shovel1, shovelItemScene);
        SpawnObjects(layers["FG"], ObjectType.Shovel2, shovelItemScene);
        SpawnObjects(layers["FG"], ObjectType.Shovel3, shovelItemScene);
        SpawnObjects(layers["FG"], ObjectType.Coin1, coinItemScene);
        SpawnObjects(layers["FG"], ObjectType.Coin2, coinItemScene);
        SpawnObjects(layers["FG"], ObjectType.Coin3, coinItemScene);
        SpawnObjects(layers["FG"], ObjectType.Coin4, coinItemScene);
        SpawnObjects(layers["FG"], ObjectType.Coin5, coinItemScene);
        SpawnObjects(layers["FG"], ObjectType.Coin6, coinItemScene);
        SpawnObjects(layers["FG"], ObjectType.Enemy1, enemyScene);
        SpawnObjects(layers["FG"], ObjectType.Enemy2, enemyScene);
        SpawnObjects(layers["FG"], ObjectType.Enemy3, enemyScene);
        SpawnObjects(layers["FG"], ObjectType.Spike, spikeScene);


        SpawnPlayer(layers["FG"], ObjectType.Player);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
