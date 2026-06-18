using UnityEngine;

public class ClickSom : MonoBehaviour
{
    public AudioSource audioSource;

    void OnMouseDown()
    {
        audioSource.Play();
    }
}