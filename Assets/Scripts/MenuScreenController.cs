using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreenController : MonoBehaviour
{
    public GameObject questionPanel;

    public void StartGame()
    {
        questionPanel.SetActive(true);
    }
}
