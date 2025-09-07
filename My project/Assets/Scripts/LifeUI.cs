using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    public Image lifePrefab;
    public Sprite fullLifeSprite;
    public Sprite emptyLifeSprite;

    private List<Image> lifes = new List<Image>();

    public void SetMaxLifes(int maxLifes)
    {
        foreach (Image life in lifes)
        {
            Destroy(life.gameObject);
        }

        lifes.Clear();

        for (int i = 0; i < maxLifes; i++)
        {
            Image newLife = Instantiate(lifePrefab, transform);
            newLife.sprite = fullLifeSprite;
            newLife.color = Color.white;
            lifes.Add(newLife);
        }
    }

    public void UpdateLifes(int currentLife)
    {
        for (int i = 0; i < lifes.Count; i++)
        {
            if (i < currentLife)
            {
                lifes[i].sprite = fullLifeSprite;
                lifes[i].color = Color.white;
            }
            else
            {
                lifes[i].sprite = emptyLifeSprite;
                lifes[i].color = Color.red;
            }
        }
    }
}
