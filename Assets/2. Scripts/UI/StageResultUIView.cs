using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageResultUIView : MonoBehaviour
{
    public event Action RetryButtonClicked;
    public event Action BackToRehearsalButtonClicked;

    [SerializeField] private GameObject _rootObject;
    [SerializeField] private TMP_Text _resultTitleText;
    [SerializeField] private TMP_Text _resultDetailText;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _backButton;

    private void Awake()
    {
        BindButtons();
        Hide();
    }

    private void OnDestroy()
    {
        UnbindButtons();
    }

    public void ShowResult(bool isSuccess, string detailText)
    {
        if (_rootObject != null)
        {
            _rootObject.SetActive(true);
        }

        if (_resultTitleText != null)
        {
            _resultTitleText.text = isSuccess ? "공연 성공" : "공연 실패";
        }

        if (_resultDetailText != null)
        {
            _resultDetailText.text = detailText;
        }
    }

    public void Hide()
    {
        if (_rootObject != null)
        {
            _rootObject.SetActive(false);
        }
    }

    private void BindButtons()
    {
        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(OnRetryButtonClicked);
        }

        if (_backButton != null)
        {
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    private void UnbindButtons()
    {
        if (_retryButton != null)
        {
            _retryButton.onClick.RemoveListener(OnRetryButtonClicked);
        }

        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }

    private void OnRetryButtonClicked()
    {
        RetryButtonClicked?.Invoke();
    }

    private void OnBackButtonClicked()
    {
        BackToRehearsalButtonClicked?.Invoke();
    }
}