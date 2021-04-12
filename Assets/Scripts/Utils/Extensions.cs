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
        
        public static T[,] Shuffle<T>(this T[,] matrix)
        {
            int howManyRows = matrix.GetLength(0);
            int howManyColumns = matrix.GetLength(1);
            T[,] randomizedMatrix = new T[howManyRows, howManyColumns];
            //we will use those arrays to randomize indexes
            int[] shuffledRowIndexes = Enumerable.Range(0, howManyRows).ToArray();
            int[] shuffledColumnIndexes = Enumerable.Range(0, howManyColumns).ToArray();
            System.Random rnd = new  System.Random();
            shuffledRowIndexes = shuffledRowIndexes.OrderBy(x => rnd.Next()).ToArray();

            for (int i = 0; i < howManyRows; i++)
            {
                // at every loop we get new randomized column idexes, so every row will be shuffled independently
                shuffledColumnIndexes = shuffledColumnIndexes.OrderBy(x => rnd.Next()).ToArray();
                for (int j = 0; j < howManyColumns; j++)
                    randomizedMatrix[i, j] = matrix[shuffledRowIndexes.ElementAt(i), shuffledColumnIndexes.ElementAt(j)];
            }

            return randomizedMatrix;
        }
    }
}