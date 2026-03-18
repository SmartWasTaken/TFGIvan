using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [Serializable]
    public class TextoRecibidoData
    {
        public int id;
        public string titulo;
        public string contenido;
        public int id_capitulo;
        public string nombre_capitulo;
        public int orden_capitulo;
        public string ambientacion;
    }

    [Serializable]
    public class RespuestaEnviadaData
    {
        public int texto;
        public string voto_usuario;
        public string comentario;
        public int nivel_confianza;
        public float tiempo_lectura_segundos;
    }

    [Serializable]
    public class FeedbackRecibidoData
    {
        public string resultado;
        public bool es_acierto;
        public string origen_real;
        public string explicacion_experto;
    }

    [Serializable]
    public class PaginaTutorialData
    {
        public int orden;
        public string titulo;
        public string contenido;
    }

    [Serializable]
    public class TutorialInfoData
    {
        public bool completado;
        public PaginaTutorialData[] paginas;
    }
}
