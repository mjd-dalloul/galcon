using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator {
    
    public static float minWidth = -7.5f;
    public static float maxWidth = 7.5f;
    
    public static float minHeight = -2.5f;
    public static float maxHeight = 4.5f;
    
    public static float minRaduis = 0.5f;
    public static float maxRaduis = 0.9f;
    
    public static float minShips = 5;
    public static float maxShips = 50;
    

    public struct PlanetInfo
    {
        public float x, y, r;
        public int s;
    }
    
    public static PlanetInfo[] generate(int planetsCount, int playersCount, int seed)
    {
        int tries = 0;
        UnityEngine.Random.seed = seed;
        PlanetInfo[] planetsInfo = new PlanetInfo[planetsCount];
        for (int i = 0; i < planetsCount; i++)
        {
            PlanetInfo cur = new PlanetInfo();
            cur.x = UnityEngine.Random.Range(minWidth, maxWidth);
            cur.y = UnityEngine.Random.Range(minHeight, maxHeight);
            cur.r = (i < playersCount) ? maxRaduis + 0.15f : UnityEngine.Random.Range(minRaduis, maxRaduis);
            cur.s = (i < playersCount) ? 100 : (int) UnityEngine.Random.Range(minShips, maxShips);

            bool ok = true;

            if(cur.y > 3.6 && cur.x > 6.6)
            {
                i--;
                continue;
            }

            for(int j=0; j<i; j++)
            {
                PlanetInfo pre = planetsInfo[j];
                if (dist(pre.x - cur.x, pre.y - cur.y) <= pre.r + cur.r + 0.6)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                planetsInfo[i] = cur;
            }
            else if (tries > 50000)
            {
                Debug.Log("tries = " + tries + " i = " + i );
                throw new System.Exception("Failed to generate a map");
            }
            else
            {
                tries++;
                i--;
                continue;
            }
        }

        return planetsInfo;
    }
    private static float dist(float x, float y)
    {
        return Mathf.Sqrt((x * x) + (y * y));
    }
    
}