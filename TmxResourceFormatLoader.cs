using Godot;
using System;

public class TmxResourceFormatLoader : ResourceFormatLoader
{
    override public string[] GetRecognizedExtensions()
    {
        return new string[]{"tmx"};
    }

    public override string GetResourceType(string path)
    {
        return "Resource";
    }

    public override bool HandlesType(string typename)
    {
        return typename == "Resource";
    }

    public override object Load(string path, string originalPath)
    {
        return new Resource();
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
