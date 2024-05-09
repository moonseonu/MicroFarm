using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // �񵿱� �ε��� ���� ����
    AsyncOperation asyncLoad;

    // Start is called before the first frame update
    void Start()
    {
        // �񵿱�� ���� �ε��ϱ� ���� LoadSceneAsync �Լ� ȣ��
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        // �� �ε带 �����ϰ� AsyncOperation ��ü�� ����
        asyncLoad = SceneManager.LoadSceneAsync("Main");

        // �� �ε尡 �Ϸ�� ������ ��ٸ�
        while (!asyncLoad.isDone)
        {
            // �ε� ���� ��Ȳ�� ǥ���� �� ����
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0���� 1 ������ ������ ����ȭ
            Debug.Log("�ε� ���൵: " + (progress * 100) + "%");

            yield return null; // �� �������� ��ٸ�
        }
    }
}