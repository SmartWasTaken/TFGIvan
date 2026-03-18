using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Cancion
{
    public string nombreMostrado;
    public AudioClip clip;
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Lista de Canciones")]
    public List<Cancion> playlist;

    [Header("Interfaz de Usuario (Estilo Spotify)")]
    public TMP_Text textoNombreCancion;

    [Header("Ajustes de Pausa (Segundos)")]
    public float minPausa = 5f;
    public float maxPausa = 15f;

    [HideInInspector]
    public AudioSource audioSource;

    private List<Cancion> cancionesPendientes = new List<Cancion>();

    private Coroutine rutinaMusica;

    void Start()
    {
        Application.runInBackground = true;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        if (playlist.Count > 0)
        {
            rutinaMusica = StartCoroutine(ReproducirMusicaAleatoria());
        }
    }

    IEnumerator ReproducirMusicaAleatoria()
    {
        while (true)
        {
            if (cancionesPendientes.Count == 0)
            {
                cancionesPendientes = new List<Cancion>(playlist);
            }

            int randomIdx = Random.Range(0, cancionesPendientes.Count);
            Cancion cancionElegida = cancionesPendientes[randomIdx];

            cancionesPendientes.RemoveAt(randomIdx);

            if (textoNombreCancion != null)
            {
                textoNombreCancion.text = cancionElegida.nombreMostrado;
            }

            audioSource.clip = cancionElegida.clip;
            audioSource.Play();

            yield return new WaitForSeconds(cancionElegida.clip.length);

            float tiempoPausa = Random.Range(minPausa, maxPausa);
            yield return new WaitForSeconds(tiempoPausa);
        }
    }

    public void SaltarCancion()
    {
        if (rutinaMusica != null) StopCoroutine(rutinaMusica);
        audioSource.Stop();

        rutinaMusica = StartCoroutine(ReproducirMusicaAleatoria());
    }
}