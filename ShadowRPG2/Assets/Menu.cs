using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] Animator ftbImageAnim;

    public void ClickOnPlay()
    {
        StartCoroutine(CoClickOnPlay());
    }

    IEnumerator CoClickOnPlay()
    {
        ftbImageAnim.gameObject.SetActive(true);
        ftbImageAnim.SetBool("IsOut", false);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("BasicScene", LoadSceneMode.Single);
    }
}
