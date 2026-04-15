using UnityEngine;

public class ActorSelectionView : MonoBehaviour
{
    [SerializeField] private GameObject _sourceHighlightObject;
    [SerializeField] private GameObject _targetHighlightObject;

    private void Awake()
    {
        ClearSelection();
    }

    public void ShowAsSource()
    {
        SetActive(_sourceHighlightObject, true);
        SetActive(_targetHighlightObject, false);
    }

    public void ShowAsTarget()
    {
        SetActive(_sourceHighlightObject, false);
        SetActive(_targetHighlightObject, true);
    }

    public void ClearSelection()
    {
        SetActive(_sourceHighlightObject, false);
        SetActive(_targetHighlightObject, false);
    }

    private void SetActive(GameObject targetObject, bool isActive)
    {
        if (targetObject == null)
        {
            return;
        }

        targetObject.SetActive(isActive);
    }
}