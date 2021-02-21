using UnityEngine;
//*****    *****//
// Singleton simples
// Uma única cópia persiste entre cenas
// Pode ser usado para Gameobjects com MonoBehaviour simples
// Para scrips com corotinas pode ser necessária uma implementação mais complexa
// Neste Template o GameObject persistente tem um AudioSource (para música de fundo)
// E tem também array de AudioClip, com algumas músicas para trocar no AudioSource
// O Fade Manager e o Load Scene Manager foram colocado como Child para completar o FadeOut na Scene Principal
// Se quiser usar eles na Scene Principal, ou em outras Scenes, basta acrescentar eles como atributos
//*****     *****//
[RequireComponent(typeof(AudioSource))] //AudioSource obrigatório para o GameObject
public class ManagerSomSingletonJMF : MonoBehaviour
{
    [SerializeField] private AudioSource MusicPlayer = default; //Audio Sorce atrelado ao GameObject
    [SerializeField] private AudioClip[] Musicas; //Lista de músicas para ser usada depois
    public static ManagerSomSingletonJMF staticSingleton { get; private set; } //Só pode haver um!
    void Awake()
    {
        if (staticSingleton == null)//não tem ninguém ainda
        {
            staticSingleton = this; //Eu sou o highlander!
            DontDestroyOnLoad(this.gameObject);//E vivo para sempre!
        }
        else if (staticSingleton != this)//Não! Já tem outro :-(
        {
            Destroy(this.gameObject); //Só pode haver um e não sou eu :-( bye bye!
        }
    }
    void Start()
    {
        if (MusicPlayer == null) //Não atribuiu o AudioSource
        {
            MusicPlayer = GetComponent<AudioSource>(); //Pega o Audiosorce
        }
    }
    public AudioSource GetAudioSource()
    {
        if (MusicPlayer == null) //Não deve acontecer, mas por precaução
        {
            MusicPlayer = gameObject.AddComponent<AudioSource>();
        }
        return MusicPlayer;
    }
    public AudioClip GetMusic(int i) //Retorna um Clip de música
    {
        if (Musicas.Length > i) //Se tem músicas suficientes na lista
        {
            return Musicas[i]; //Retorna música do indice passado
        }
        else //Indice fora da lista
        {
            if (Musicas.Length > 0) //Se tem pelo menos uma música
            {
                return Musicas[Musicas.Length - 1]; //Retorna última música
            }
            else //Não tem nenhuma música na lista
            {
                //Debug.Log("Esqueceu de colocar músicas na lista?");
                return null; //Retorna null
            }
        }
    }
}