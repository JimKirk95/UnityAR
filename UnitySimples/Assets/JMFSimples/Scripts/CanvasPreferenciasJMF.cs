using UnityEngine;
using UnityEngine.UI;
// *****           *****//
// Usa StaticPlayerPrefsJMF para acessar e salvar as PlayerPrefs de forma estruturada
// Usa SingletonJMF para acessar AudioSource e AudioClips
// *****           *****//

public class CanvasPreferenciasJMF : MonoBehaviour
{
    [SerializeField] private Text PercentualVolume = default; //Volume para mostrar na tela
    [SerializeField] private Slider Volume = default; //Ajuste de volume
    [SerializeField] private Toggle Mudo = default; //Mudo ou não
    [SerializeField] private Dropdown ListaMusicas = default; //Escolha de fundo musical
    [SerializeField] private Text EntreAtualizacoes = default; //Atualizações para mostrar na tela
    [SerializeField] private Slider Intervalo = default; //Ajuste de intervalo de atualizações
    [SerializeField] private Toggle Nunca = default; //Desliga atualizações
    private AudioSource meuAudio = default; //Guarda referência para o AudioSource global
    void Start()
    {
        meuAudio = ManagerSomSingletonJMF.staticSingleton.GetAudioSource(); // Recupera AudioSource
        GlobalPlayerPrefsJMF.RecuperaPlayerPrefs(); //Manda carregar PlayerPrefs (redundante, o Manager da Intro já deve ter feito isso
        AtualizaTela(); //Atualiza componentes da tela
        AtualizaAudio(); //Redundante, o Manager da Intro já deve ter feito isso    
        Volume.onValueChanged.AddListener(VolumeChanged);
        Mudo.onValueChanged.AddListener(MudoChanged);
        ListaMusicas.onValueChanged.AddListener(MusicaChanged);
        Intervalo.onValueChanged.AddListener(IntervaloChanged);
    }
    public void AtualizaTela() //Atualiza tela no início e após um canelamento de edição
    {
        PercentualVolume.text = "Volume: <b>" + ((int)(100 * GlobalPlayerPrefsJMF.Volume)).ToString() + "%</b>";//Escreve volume como percentual inteiro
        Volume.value = GlobalPlayerPrefsJMF.Volume; // Atualiza slider de volume
        Mudo.isOn = GlobalPlayerPrefsJMF.Mudo; // Atualiza toggle mudo
        ListaMusicas.value = GlobalPlayerPrefsJMF.IndiceMusica;
        EntreAtualizacoes.text = (GlobalPlayerPrefsJMF.Intervalo == 1) ? "1 execução" : GlobalPlayerPrefsJMF.Intervalo.ToString() + " execuções";
        Intervalo.value = GlobalPlayerPrefsJMF.Intervalo; //Atualiza slider de intervalo
        Nunca.isOn = GlobalPlayerPrefsJMF.Nunca; // Atualiza toggle nunca
    }
    public void AtualizaAudio() //Atualiza audio após um cancelamento de edição
    {
        meuAudio.volume = GlobalPlayerPrefsJMF.Volume; //Ajusta volume
        meuAudio.mute = GlobalPlayerPrefsJMF.Mudo; //Ajusta mudo ou não
        AudioClip VerificaMusica = ManagerSomSingletonJMF.staticSingleton.GetMusic(GlobalPlayerPrefsJMF.IndiceMusica); //Pega música
        if (!VerificaMusica.Equals(meuAudio.clip)) //Se a música for diferente
        {
            meuAudio.Stop(); //Para audio
            meuAudio.clip = VerificaMusica; //muda musica
            meuAudio.Play(); //toca musica
        }
    }
    private void VolumeChanged(float novoVolume) //Atualiza volume temporariamente
    {
        PercentualVolume.text = "Volume: <b>" + ((int)(100 * novoVolume)).ToString() + "%</b>";//Escreve volume como percentual inteiro
        meuAudio.volume = novoVolume;
    }
    private void MudoChanged(bool novoMudo) //Atualiza Mudo temporariamente
    {
        meuAudio.mute = novoMudo;
    }
    private void MusicaChanged(int novoIndiceMusica) //Atualiza música temporariamente
    {
        AudioClip VerificaMusica = ManagerSomSingletonJMF.staticSingleton.GetMusic(novoIndiceMusica); //Pega música
        if (!VerificaMusica.Equals(meuAudio.clip)) //Se a música for diferente
        {
            meuAudio.Stop(); //Para audio
            meuAudio.clip = VerificaMusica; //muda musica
            meuAudio.Play(); //toca musica
        }
    }
    private void IntervaloChanged(float novoIntervalo) //atualiza intervalo na tela
    {
        EntreAtualizacoes.text = (novoIntervalo == 1) ? "1 execução" : novoIntervalo.ToString() + " execuções";
    }
    public void AcaoGravarPreferencias() //Resposta ao ação gravar. Atualiza PlayerPrefs
    {
        GlobalPlayerPrefsJMF.AtualizaVolume(Volume.value); // Atualiza slider de volume
        GlobalPlayerPrefsJMF.AtualizaMudo(Mudo.isOn); // Atualiza toggle mudo
        GlobalPlayerPrefsJMF.AtualizaIndiceMusica(ListaMusicas.value);
        GlobalPlayerPrefsJMF.AtualizaIntervalo((int)Intervalo.value); //Atualiza slider de intervalo
        GlobalPlayerPrefsJMF.AtualizaNunca(Nunca.isOn); // Atualiza toggle nunca
    }
    public void AcaoCancelarPreferencias()
    {
        AtualizaAudio(); //Atualiza o Audio
        AtualizaTela(); //Atualiza a tela para quando chamar de novo
    }
}
