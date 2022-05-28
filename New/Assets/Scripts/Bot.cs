using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bot : MonoBehaviour {
    private double c1 = .5, c2 = .3, c3 = .7, c4 = .6;
    // private Dictionary<Tuple<int, int>, double> benifit;
    private Dictionary<int, Dictionary<int, double>> benifit;
    private Dictionary<int, double> risk, support;
    private List<int> numOfAttacks;
    private Player player;
    private LevelManager levelManager;
    private List<Tuple<int, int>> currAttack;
    void Start() {
        benifit = new Dictionary<int, Dictionary<int, double>>();
        for (int i = 0; i < 20; i++) {
            benifit[i] = new Dictionary<int, double>();
        }
        risk = new Dictionary<int, double>();
        support = new Dictionary<int, double>();
        currAttack = new List<Tuple<int, int>>();
        numOfAttacks = new List<int>();
        player = GetComponent<Player>();
        levelManager = FindObjectOfType<LevelManager>();
        ThreadManager.startThreadFunction(run);
    }
    private void run() {
        while (levelManager.getPlanets() == null)Thread.Sleep(1000);
        while (true) {
            clearAll();
            makeDecision();
            //foreach (int i in numOfAttacks)
            //    if (i > 0)
            //        doSupport(i);

            Thread.Sleep(1000); // wait 3s to make the next decision
        }
    }
    private void clearAll() {
        foreach (Tuple<int, int> tp in currAttack) {
            ThreadManager.queueToMainThread(() => player.stopAttacking(tp.Item1, tp.Item2));
        }
        currAttack.Clear();
    }
    private void makeDecision() {
        calcData();
        double mxBen = 0.0;
        Tuple<int, int> hash = new Tuple<int, int>(0, 0);
        foreach (Planet a in levelManager.getPlanets()) {
            if (a.getOwner() != player)continue;

            foreach (Planet b in levelManager.getPlanets()) {
                if (b.getOwner() == player)continue;
                Tuple<int, int> h = new Tuple<int, int>(a.getID(), b.getID());
                // double ben = benifit[h];
                double ben = benifit[a.getID()][b.getID()];
                if (ben > mxBen) {
                    mxBen = ben;
                    hash = h;
                }

            }
        }

        if (mxBen > 1.0) {
            ThreadManager.queueToMainThread(() => player.attack(hash.Item1, hash.Item2));
            currAttack.Add(hash);
        }
    }
    public void onPlanetAttacked(Tuple<int, int> attk, bool isStarted) {

        Planet src = levelManager.getPlanet(attk.Item1);
        Planet tar = levelManager.getPlanet(attk.Item2);
        int idx = tar.getID();

        if (isStarted) {
            numOfAttacks[idx] = numOfAttacks[idx + 1];
        } else {
            numOfAttacks[idx] = numOfAttacks[idx - 1];
        }

        if (tar.getOwner() != player) {
            return;
        }

    }
    private void doSupport(int target) {
        if (numOfAttacks[target] != 0) {
            Tuple<double, int> mx = new Tuple<double, int>(-1, -1);
            foreach (Planet a in levelManager.getPlanets()) {
                if (a.getOwner() != player)continue;
                ///First, second = index
                if (mx.Item1 < ((a.getShipsCount() - risk[a.getID()]))) {
                    mx = new Tuple<double, int>(a.getShipsCount() - risk[a.getID()], a.getID());
                }
            }
            if (mx.Item1 > 5) {
                //todo attack mx.Sc -> target

            }
        }
    }

    private float getDist(Planet a, Planet b) {
        float x = a.getPosition().x;
        float y = a.getPosition().y;
        float xx = b.getPosition().x;
        float yy = b.getPosition().y;
        float diffX = x - xx, diffY = y - yy;
        return Mathf.Sqrt(diffX * diffX + diffY * diffY);
    }

    private void calcData() {
        List<Planet> planets = levelManager.getPlanets();

        foreach (Planet a in planets) {
            if (a.isNeutal())continue;
            int i = a.getID();
            double sum = 0.0;
            foreach (Planet b in planets) {
                int j = b.getID();
                if (b.getOwner() != a.getOwner() && !b.isNeutal() && b != a) {
                    sum += b.getShipsCount() / getDist(a, b);
                }
            }

            double res = c1 * sum;
            risk[i] = res;

        }

        foreach (Planet a in planets) {
            int i = a.getID();
            double sum = 0.0;
            if (a.isNeutal())continue;

            foreach (Planet b in planets) {
                int j = b.getID();
                if (b.getOwner() == a.getOwner() && !b.isNeutal()) {
                    double s = 0;
                    if (b.getShipsCount() - risk[j] > 0)s = b.getShipsCount() - risk[j];
                    else s = 0;
                    sum += s;
                }
            }
            double res = c2 * sum;
            support[i] = res;
        }
        foreach (Planet a in planets) {
            if (!a.isNeutal())continue;
            int i = a.getID();
            double sum = 0.0;
            foreach (Planet b in planets) {
                int j = b.getID();
                if (b.getOwner() != player && !b.isNeutal()) {
                    double s = 0;
                    if (b.getShipsCount() - risk[j] > 0)
                        s = b.getShipsCount() - risk[j];
                    sum += s;
                }
            }
            double res = c2 * sum;
            support[i] = res;
        }
        foreach (Planet a in planets) {
            if (a.isNeutal())continue;
            int i = a.getID();
            foreach (Planet b in planets) {
                if (a.getOwner() == b.getOwner())continue;
                int j = b.getID();
                // ownership changed while calcData call wait for next call
                if (risk[i] == null || support[i] == null || support[j] == null)continue;
                double res = (a.getShipsCount() - risk[i] +
                    c3 * support[i]) / (b.getShipsCount() + support[j]);
                res = c4 * b.getRadius() * res;
                benifit[i][j] = res;
            }
        }
    }

    public void setDifficulty(int c1, int c2, int c3, int c4) {
        this.c1 = c1;
        this.c2 = c2;
        this.c3 = c3;
        this.c4 = c4;
    }
}