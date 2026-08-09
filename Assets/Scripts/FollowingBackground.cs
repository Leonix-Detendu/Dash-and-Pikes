using UnityEngine;

public class FollowingBackground : MonoBehaviour
{
    // On n'a plus besoin de nextArrangement, on manipule directement les objets
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private GameObject player;

    [SerializeField] private float tileSizeX;
    [SerializeField] private float tileSizeY;

    [SerializeField] private float nbTilesOnX, nbTilesOnY;

    // Le centre virtuel de ta grille
    private Vector2 gridCenter;

    private void Start()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            tiles[i] = child.gameObject;
            i++;
        }

        transform.position = player.transform.position;
        gridCenter = player.transform.position;

        Debug.Log(gridCenter);
    }

    private void Update()
    {
        Vector2 playerPos = player.transform.position;

        // Si le joueur va trop à DROITE
        if (playerPos.x - gridCenter.x > tileSizeX)
        {
            ShiftTilesX(tileSizeX * nbTilesOnX, true); // On décale la colonne de gauche vers la droite
            gridCenter.x += tileSizeX;         // On met à jour le centre virtuel
            Debug.Log(gridCenter);
        }
        // Si le joueur va trop à GAUCHE
        else if (playerPos.x - gridCenter.x < -tileSizeX)
        {
            ShiftTilesX(-tileSizeX * nbTilesOnX, false);
            gridCenter.x -= tileSizeX;
            Debug.Log(gridCenter);
        }

        // Si le joueur va trop en HAUT
        if (playerPos.y - gridCenter.y >= tileSizeY)
        {
            ShiftTilesY(tileSizeY * nbTilesOnY, true);
            gridCenter.y += tileSizeY;
            Debug.Log(gridCenter);
        }
        // Si le joueur va trop en BAS
        else if (playerPos.y - gridCenter.y <= -tileSizeY)
        {
            ShiftTilesY(-tileSizeY * nbTilesOnY, false);
            gridCenter.y -= tileSizeY;
            Debug.Log(gridCenter);
        }
    }

    // Déplace la colonne appropriée sur l'axe X
    private void ShiftTilesX(float shiftAmount, bool playerMovingRight)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Vector3 pos = tiles[i].transform.position;

            // Si le joueur va à droite, on cherche les tuiles qui sont restées tout à gauche
            if (playerMovingRight && pos.x <= gridCenter.x - tileSizeX)
            {
                pos.x += shiftAmount;
                tiles[i].transform.position = pos;
            }
            // Si le joueur va à gauche, on cherche les tuiles qui sont restées tout à droite
            else if (!playerMovingRight && pos.x >= gridCenter.x + tileSizeX)
            {
                pos.x += shiftAmount;
                tiles[i].transform.position = pos;
            }
        }
    }

    // Déplace la ligne appropriée sur l'axe Y
    private void ShiftTilesY(float shiftAmount, bool playerMovingUp)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Vector3 pos = tiles[i].transform.position;

            if (playerMovingUp && pos.y <= gridCenter.y - tileSizeY)
            {
                pos.y += shiftAmount;
                tiles[i].transform.position = pos;
            }
            else if (!playerMovingUp && pos.y >= gridCenter.y + tileSizeY)
            {
                pos.y += shiftAmount;
                tiles[i].transform.position = pos;
            }
        }
    }
}