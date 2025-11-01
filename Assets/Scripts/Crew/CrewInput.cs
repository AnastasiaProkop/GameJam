using UnityEngine;

public class CrewInput : MonoBehaviour
{
    private CrewMember selectedCrew;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // Попали в матроса?
                CrewMember crew = hit.collider.GetComponent<CrewMember>();
                if (crew != null)
                {
                    selectedCrew = crew;
                    Debug.Log("Выбран матрос!");
                    return;
                }

                // Попали в задачу и есть выбранный матрос?
                ShipTask task = hit.collider.GetComponent<ShipTask>();
                if (task != null && selectedCrew != null)
                {
                    selectedCrew.AssignToTask(task);
                    Debug.Log("Матрос назначен на задачу!");
                    selectedCrew = null; // Сбрасываем выбор
                }
            }
        }
    }
}