using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class Extensions
    {
        public static Bounds OrthographicBounds(this Camera camera)
        {
            float screenAspect = Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            Bounds bounds = new Bounds(
                camera.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }

        public static T GetRandom<T>(this IList<T> collection)
        {
            int index = Random.Range(0, collection.Count);
            return collection[index];
        }
    }
}