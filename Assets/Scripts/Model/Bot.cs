using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bot : MonoBehaviour
{
    private double c1, c2, c3, c4;
    private Dictionary<int, Dictionary<int, double>> benifit;
    private Dictionary<int, double> risk, support;
    // private List<int> numOfAttacks = new List<int>(GameSettings.getPlanetsCount());
    private int[] numOfAttacks = new int[GameSettings.getPlanetsCount()];
    private Player player;
    private Thread botThread;

    private List<Tuple<int, int>> currAttacks;
    void Awake()
    {
        benifit = new Dictionary<int, Dictionary<int, double>>();
        for (int i = 0; i < GameSettings.getPlanetsCount(); i++)
        {
            benifit[i] = new Dictionary<int, double>();
        }
        risk = new Dictionary<int, double>();
        support = new Dictionary<int, double>();
        currAttacks = new List<Tuple<int, int>>();
        player = GetComponent<Player>();
    }

    public void strartBotThread()
    {
        botThread = ThreadManager.startThreadFunction(run);
    }

    private void run()
    {
        while (true)
        {
            calcData();
            clearAttacks();
            doAttack();
            // for(int i=0 ; i<numOfAttacks.Length ; i++)
            //    if (numOfAttacks[i] > 0)
            //        doSupport(i);

            Thread.Sleep(300);
        }
    }
    private void clearAttacks()
    {
        foreach (var planetId in player.planets)
        {

            ThreadManager.queueToMainThread(() => LevelManager.cancelAttack(planetId));

        }
        currAttacks.Clear();
    }
    private void doAttack()
    {

        var mx_bsd = new Tuple<double, int, int>(0, 0, 0);
        var src_list = LevelManager.getPlanets().FindAll(p => p.getOwner() == player);
        var dst_list = LevelManager.getPlanets().FindAll(p => p.getOwner() != player);

        foreach (Planet src in src_list)
        {
            foreach (Planet dst in dst_list)
            {
                int i = src.getID();
                int j = dst.getID();
                if (benifit.ContainsKey(i) && benifit[i].ContainsKey(j))
                {
                    var cur = new Tuple<double, int, int>(benifit[i][j], i, j);
                    if (cur.Item1 > mx_bsd.Item1)
                    {
                        mx_bsd = cur;
                    }
                }
            }
        }

        if (mx_bsd.Item1 > 1.0)
        {
            ThreadManager.queueToMainThread(() => LevelManager.startAttack(mx_bsd.Item2, mx_bsd.Item3));
            currAttacks.Add(new Tuple<int, int>(mx_bsd.Item2, mx_bsd.Item3));

        }
    }

    private void doSupport(int target)
    {

        Tuple<double, int> mx = new Tuple<double, int>(0, -1);
        var src_list = LevelManager.getPlanets().FindAll(p => p.getOwner() == player);

        foreach (Planet src in src_list)
        {
            int i = src.getID();
            double surplus = src.getShipsCount() - risk[i];
            if (mx.Item1 < surplus)
            {
                mx = new Tuple<double, int>(surplus, i);
            }
        }

        if (mx.Item2 != -1)
        {
            ThreadManager.queueToMainThread(() => LevelManager.startAttack(mx.Item2, target));
        }
    }

    private void calcData()
    {
        List<Planet> planets = LevelManager.getPlanets();

        // numOfAttacks
        var currAttacks = LevelManager.getCurrentAttacks();
        for (int i = 0; i < numOfAttacks.Length; i++) numOfAttacks[i] = 0;
        foreach (var atk in currAttacks)
        {
            if (LevelManager.getPlanet(atk.Item1).getOwner() != player
            && LevelManager.getPlanet(atk.Item2).getOwner() == player)
            {
                if (atk.Item2 < 0 || atk.Item2 >= numOfAttacks.Length) Debug.Log(atk.Item2);
                numOfAttacks[atk.Item2]++;
            }
        }


        // risk
        foreach (Planet a in planets)
        {
            // if (a.isNeutal())continue;
            double sum = 0.0;
            foreach (Planet b in planets)
            {
                if (a.getOwner() != b.getOwner() && !b.isNeutal())
                {
                    sum += b.getShipsCount() / getDist(a, b);
                }
            }
            risk[a.getID()] = c1 * sum;
        }
        // support, non-neutral
        foreach (Planet a in planets)
        {
            if (a.isNeutal()) continue;
            double sum = 0.0;
            foreach (Planet b in planets)
            {
                if (a.getOwner() == b.getOwner())
                {
                    double surplus = Mathf.Max((float)(b.getShipsCount() - risk[b.getID()]), 0);
                    sum += surplus;
                }
            }
            support[a.getID()] = c2 * sum;
        }

        // support, neautral
        foreach (Planet a in planets)
        {
            if (!a.isNeutal()) continue;
            double sum = 0.0;
            foreach (Planet b in planets)
            {
                if (b.getOwner() != player && !b.isNeutal())
                {
                    double surplus = Mathf.Max((float)(b.getShipsCount() - risk[b.getID()]), 0);
                    sum += surplus;
                }
            }
            support[a.getID()] = c2 * sum;
        }

        // benifit
        foreach (Planet a in planets)
        {
            if (a.isNeutal()) continue;
            foreach (Planet b in planets)
            {
                if (a.getOwner() == b.getOwner()) continue;
                int i = a.getID();
                int j = b.getID();
                // ownership changed while calcData call wait for next call
                if (!risk.ContainsKey(i) || !support.ContainsKey(i) || !support.ContainsKey(j)) continue;
                double res = (a.getShipsCount() - risk[i] + c3 * support[i]) / (b.getShipsCount() + support[j]);
                benifit[i][j] = c4 * b.getRadius() * res;
            }
        }
    }
    public void setDifficulity(GameSettings.PlayerType botType)
    {
        var c = GameSettings.playersConsts[(int)botType];
        this.c1 = c[0];
        this.c2 = c[1];
        this.c3 = c[2];
        this.c4 = c[3];
    }
    private float getDist(Planet a, Planet b)
    {
        return Vector3.Distance(a.getPosition(), b.getPosition());
    }

    private void OnDestroy()
    {
        botThread.Abort();

    }
}