using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    public static List<Carrot> allCarrots;

    public Vector2Int Location { get; private set; }

    void Start()
    {
        if (allCarrots == null) allCarrots = new List<Carrot>();

        allCarrots.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetLocation(int x, int y)
    {
        Location = new Vector2Int(x, y);
    }

    public void Remove(bool roundEnded = false)
    {
        allCarrots.Remove(this);
        GameManager.instance.carrotGrid[Location.x, Location.y] = null;

        if(!roundEnded && allCarrots.Count == 0)
        {
            GameManager.instance.OnRabbitRoundWin();
        }

        Destroy(gameObject);
    }
}
