using UnityEngine;

public class StageFlowRuntime : MonoBehaviour
{
    public static StageFlowRuntime Instance { get; private set; }

    public StageRuntimeData RuntimeData => _runtimeData;

    [SerializeField] private StageRuntimeData _runtimeData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetRuntimeData(StageRuntimeData runtimeData)
    {
        if (runtimeData == null)
        {
            Debug.LogError("RuntimeData is null.");
            return;
        }

        _runtimeData = runtimeData;
    }

    public void ClearRuntimeData()
    {
        _runtimeData = null;
    }

    public bool HasRuntimeData()
    {
        return _runtimeData != null;
    }
}