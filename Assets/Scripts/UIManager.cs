using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Button playBtn;

    private void Start()
    {
        playBtn.onClick.RemoveAllListeners();
        playBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
    }
}
