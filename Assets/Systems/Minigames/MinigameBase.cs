using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    public delegate void MinigameCompletedHandler();
    public event MinigameCompletedHandler OnMinigameCompleted;

    public  void StartMinigame()
    {
        var camera = (CameraController)FindObjectOfType(typeof(CameraController));
        if (camera != null)
        {
            camera.DontMoveCamera();
        }
        transform.parent.transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    protected void CompleteMinigame()
    {
        var camera = (CameraController)FindObjectOfType(typeof(CameraController));
        if (camera != null)
        {
            camera.CanMoveCamera();
        }
        transform.parent.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);

        SoundManager.Instance.PlaySFX("FinishObj");
        OnMinigameCompleted?.Invoke();

        
    }
}
