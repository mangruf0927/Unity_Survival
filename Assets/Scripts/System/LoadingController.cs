using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private LoadingUI loadingUI;

    private async void Start()
    {
        try
        {
            await LoadGameAsync();
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            if (loadingUI != null) loadingUI.ShowError();
        }
    }

    private async UniTask LoadGameAsync()
    {
        var ct = this.GetCancellationTokenOnDestroy();
        Scene loadingScene = gameObject.scene;

        if (loadingUI == null) throw new InvalidOperationException("LoadingUI 연결이 필요합니다.");

        loadingUI.SetProgress(0f);
        await UniTask.NextFrame(ct);
        DataManager.Instance.LoadAll();
        loadingUI.SetProgress(0.1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync("InGame", LoadSceneMode.Additive);
        if (operation == null) throw new InvalidOperationException("InGame 씬 로드 실패");

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingUI.SetProgress(Mathf.Lerp(0.1f, 0.3f, progress));
            await UniTask.NextFrame(ct);
        }

        Scene gameScene = SceneManager.GetSceneByName("InGame");
        SceneManager.SetActiveScene(gameScene);
        await UniTask.NextFrame(ct);

        MapGenerator mapGenerator = FindFirstObjectByType<MapGenerator>();
        TimeSystem timeSystem = FindFirstObjectByType<TimeSystem>();
        if (mapGenerator == null || timeSystem == null)
            throw new InvalidOperationException("MapGenerator 또는 TimeSystem이 없습니다.");

        await mapGenerator.GenerateMapAsync(loadingUI.SetProgress, ct);
        loadingUI.SetProgress(1f);

        while (!loadingUI.IsComplete)
        {
            await UniTask.NextFrame(ct);
        }

        await UniTask.NextFrame(ct);

        mapGenerator.ActivateWorld();
        timeSystem.StartTimer();
        loadingUI.Hide();

        await SceneManager.UnloadSceneAsync(loadingScene).ToUniTask();
    }
}
