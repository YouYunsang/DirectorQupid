using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSceneBootstrapper : MonoBehaviour
{
    [SerializeField] private StagePerformanceDirector _stagePerformanceDirector;
    [SerializeField] private string _fallbackSceneName = "RehearsalScene";

    private void Start()
    {
        if (StageFlowRuntime.Instance == null)
        {
            Debug.LogError("StageFlowRuntime instance is missing.");
            SceneManager.LoadScene(_fallbackSceneName);
            return;
        }

        if (!StageFlowRuntime.Instance.HasRuntimeData())
        {
            Debug.LogError("StageRuntimeData is missing.");
            SceneManager.LoadScene(_fallbackSceneName);
            return;
        }

        if (_stagePerformanceDirector == null)
        {
            Debug.LogError("StagePerformanceDirector is not assigned.");
            return;
        }

        _stagePerformanceDirector.Initialize(StageFlowRuntime.Instance.RuntimeData);
    }
}