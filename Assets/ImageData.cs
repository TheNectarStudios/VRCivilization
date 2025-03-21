using System;
using System.Collections.Generic;


[System.Serializable]
class ImageDataWrapper
{
    public List<ImageEntry> data;
}

[System.Serializable]
class ImageEntry
{
    public string imageName;
    public List<string> detectedObjects;

    public ImageEntry(string name, List<string> objects)
    {
        imageName = name;
        detectedObjects = objects;
    }
}
