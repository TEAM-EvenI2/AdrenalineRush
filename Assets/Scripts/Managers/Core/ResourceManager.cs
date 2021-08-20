using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
    Dictionary<string, TextAsset> textAssets = new Dictionary<string, TextAsset>();

    public T Get<T>(string path) where T : Object
    {
        string name = Utils.GetNameFromPath(path);
        if (typeof(T) == typeof(GameObject))
        {

            GameObject r = null;
            if (gameObjects.TryGetValue(name, out r))
                return r as T;
        }
        else if (typeof(T) == typeof(TextAsset))
        {

            TextAsset r = null;
            if (textAssets.TryGetValue(name, out r))
                return r as T;
        }

        return Load<T>(path);
    }

    public T Load<T>(string path) where T : Object
    {
        string name = Utils.GetNameFromPath(path);
        if (typeof(T) == typeof(GameObject))
        {

            GameObject go = null;
            if (!gameObjects.TryGetValue(name, out go))
            {
                go = Resources.Load<GameObject>(path);
                gameObjects.Add(name, go);
            }

            return go as T;
        }
        else if (typeof(T) == typeof(TextAsset))
        {
            TextAsset go = null;
            if (!textAssets.TryGetValue(name, out go))
            {
                go = Resources.Load<TextAsset>(path);
                textAssets.Add(name, go);
            }

            return go as T;
        }

            return Resources.Load<T>(path);
    }

    public T[] LoadAll<T>(string path) where T : Object
    {
        T[] results = Resources.LoadAll<T>(path);

        if (typeof(T) == typeof(GameObject))
        {
            // Caching
        }

        return results;
    }
}
