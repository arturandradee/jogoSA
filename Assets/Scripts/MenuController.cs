using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Jogar()
    {
        SceneManager.LoadScene(1);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(2);
    }

    public void VoltarMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Sair()
    {
        Debug.Log("Saiu do jogo");
        Application.Quit();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene("GameOver");
        }

    }
}