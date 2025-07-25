using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
    public float taskDuration = 3f;
    private bool isBeingWorked = false;

    private TaskGroup taskGroup;

    void Start()
    {
        // Tim TaskGroup cha
        taskGroup = GetComponentInParent<TaskGroup>();
        if (taskGroup == null)
        {
            Debug.LogWarning("TaskTrigger khong tim thay TaskGroup cha!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBeingWorked) return;

        if (collision.CompareTag("Crewmate"))
        {
            isBeingWorked = true;

            AIMove ai = collision.GetComponent<AIMove>();
            if (ai != null)
                ai.StartTask(taskDuration);

            Invoke(nameof(CompleteTask), taskDuration);
        }
    }

    void CompleteTask()
    {
        taskGroup?.NotifyTaskCompleted();
        Destroy(gameObject);
    }
}
