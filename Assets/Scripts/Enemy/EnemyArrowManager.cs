using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyArrowManager : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private GameObject arrowPrefab;

    private Dictionary<GameObject, GameObject> enemyArrows = new Dictionary<GameObject, GameObject>();

    private void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    private void Update()
    {
        var enemies = GameManager.instance.allEnemies;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            // If arrow not created for this enemy create
            if (!enemyArrows.ContainsKey(enemy))
            {
                GameObject arrow = Instantiate(arrowPrefab, canvasRect);
                enemyArrows[enemy] = arrow;
            }

            UpdateArrow(enemy, enemyArrows[enemy]);
        }
    }

    private void UpdateArrow(GameObject enemy, GameObject arrowObj)
    {
        Vector3 screenPos = mainCam.WorldToScreenPoint(enemy.transform.position);

        // Hide arrow if enemy is visible
        if (screenPos.z > 0 &&
            screenPos.x > 0 && screenPos.x < Screen.width &&
            screenPos.y > 0 && screenPos.y < Screen.height)
        {
            arrowObj.SetActive(false);
            return;
        }

        arrowObj.SetActive(true);

        // Clamp to screen edges (safe zone 50px)
        screenPos.x = Mathf.Clamp(screenPos.x, 50, Screen.width - 50);
        screenPos.y = Mathf.Clamp(screenPos.y, 50, Screen.height - 50);

        // Convert to canvas local position
        RectTransform arrowRect = arrowObj.GetComponent<RectTransform>();
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, mainCam, out canvasPos);

        arrowRect.localPosition = canvasPos;

        // Removed rotation so arrows never rotate
    }



    public void RemoveArrow(GameObject enemy)
    {
        if (enemyArrows.ContainsKey(enemy))
        {
            Destroy(enemyArrows[enemy]);
            enemyArrows.Remove(enemy);
        }
    }
}
