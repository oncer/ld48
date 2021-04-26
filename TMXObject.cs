using Godot;

public interface TMXObject {
    ObjectType Type {get; set;}
    Vector2 Position {get; set;}
}