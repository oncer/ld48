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
    int width;
    int height;

    private string ResolvePath(string path)
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
    public EarthTileType GetEarthTileAt(Vector2 pos)
    {
        TileMap fgMap = layers["FG"];
        HashSet<int> easyIndices = new HashSet<int>(new int[]{32, 33, 34, 35});
        HashSet<int> mediumIndices = new HashSet<int>(new int[]{48, 49, 50, 51});
        HashSet<int> hardIndices = new HashSet<int>(new int[]{64, 65, 66, 67, 68});
        HashSet<int> ultraIndices = new HashSet<int>(new int[]{80, 81, 82, 83, 84});

        int tileIndex = fgMap.GetCellv(new Vector2(pos.x / tileWidth, pos.y / tileHeight));
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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        layers = new Dictionary<string, TileMap>();
        File f = new File();
        string mapPath = "res://map/";
        string mapFilename = mapPath + "map.tmx";
        if (f.Open(mapFilename, File.ModeFlags.Read) == Error.Ok)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(f.GetAsText());
            XmlElement root = xmlDoc.DocumentElement;
            tileWidth = int.Parse(root.Attributes["tilewidth"].Value);
            tileHeight = int.Parse(root.Attributes["tileheight"].Value);
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
                        int tileId = int.Parse(dataStrCell) - firstgid;
                        if (tileId >= 0) {
                            map.SetCell(x, y, tileId, false, false, false);
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

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
