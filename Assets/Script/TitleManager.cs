using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // 비동기 로딩을 위한 변수
    AsyncOperation asyncLoad;

    // Start is called before the first frame update
    void Start()
    {
        // 비동기로 씬을 로드하기 위해 LoadSceneAsync 함수 호출
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // 씬 로드를 시작하고 AsyncOperation 객체를 받음
        asyncLoad = SceneManager.LoadSceneAsync("Main");

        // 씬 로드가 완료될 때까지 기다림
        while (!asyncLoad.isDone)
        {
            // 로딩 진행 상황을 표시할 수 있음
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0에서 1 사이의 값으로 정규화
            Debug.Log("로딩 진행도: " + (progress * 100) + "%");

            yield return null; // 한 프레임을 기다림
        }
    }
}