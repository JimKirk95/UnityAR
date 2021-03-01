using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****     *****//
// Scene inicial (Aviso, carregamento)
// - Carrega dados usando GlobalPlayerPrefsJMF e aciona FadeOut
// - Aciona carregamento da Scene principal em segundo plano
// Guando estiver tudo carregado, habilita botão de Continuar
// Classe baseada na SceneIntroJMF do TemplateSimples, mas sem GoogleSheets.
//*****     *****//

public class SceneIntroARJMF : MonoBehaviour
{
    [Header("Gerenciadores:")]
    [SerializeField] private ManagerLoadSceneJMF LoadSceneManager = default; //Gerenciador de Scenes
    [SerializeField] private ManagerFadeJMF FadeManager = default; // Gerenciador de Fade
    [Header("Carregamento UI:")]
    [SerializeField] private float TempoEspera = 1.5f; // Tempo mínimo de carregamento (para mostrar animação)
    [SerializeField] private Text TextCarregando = default; //Texto carregando...
    [SerializeField] private Image RotatingImage = default; //Imagem de loading rotativa
    [SerializeField] private float RotatingTime = 0.08f; // Tempo para "girar" imagem de carregamento
    [SerializeField] private float RotatingAngle = 45f; // Ângulo para "girar" imagem de carregamento
    [SerializeField] private Button BotaoContinuar = default; // botão habilitado quando a cena fica pronta
    private bool TudoCarregado = false; //Indica quando pode mudar de scene

    void Start()
    {
        TudoCarregado = false; //Flag de tudo pronto para acionar botão e parar animação
        BotaoContinuar.onClick.AddListener(AcaoLoadProximaScene); //Atribui método para onClick do botão
        StartCoroutine(FadeCanvas()); //Inicia o Fade
        StartCoroutine(LoadingAnimation()); // Inicia animação de carregamento
        GlobalPlayerPrefsJMF.RecuperaPlayerPrefs(); //Manda carregar PlayerPrefs
        IniciaAudio(); //Inicia Audio usando PlayerPrefs
        GlobalPlayerPrefsJMF.AtualizaExecucoes(GlobalPlayerPrefsJMF.Execucoes + 1);//Atualiza número de execuções
        LoadSceneManager.AcaoCarregarProximaScene(); //Manda carregar cena
        StartCoroutine(VerificaStatus()); //Inicia corotina de verificação
    }
    public void AcaoLoadProximaScene() //Resposta para o botão de prosseguir 
    {
        LoadSceneManager.AcaoMudarDeScene();
    }
    public IEnumerator FadeCanvas() //Inicia o FadeCanvas.
    {
        FadeManager.ImediatoFullIn(); //Começa com tela preta 
        yield return null; //Próximo frame
        StartCoroutine(FadeManager.CorotinaFadeOut());// Manda fazer fadeout
    }
    public IEnumerator LoadingAnimation() //Gerencia a animação de carregando...
    {
        BotaoContinuar.gameObject.SetActive(false); //Desabilita botão de Continuar
        TextCarregando.gameObject.SetActive(true); //Mostra o texto de loading
        RotatingImage.gameObject.SetActive(true); //Mostra a figura de loading
        float SpinAccumulatedTime = 0f; //Inicia cronômetro do loading
        yield return null; // volta no próximo updade
        while (!TudoCarregado) //Enquanto não está tudo pronto
        {
            if (SpinAccumulatedTime >= RotatingTime) //Se o cronômetro passou do valor
            {
                RotatingImage.rectTransform.Rotate(0f, 0f, -RotatingAngle, Space.Self);//Gira de RotatingAngle
                SpinAccumulatedTime -= RotatingTime; //Subtrai Rotating time do cronômetro para próximo giro
            }
            SpinAccumulatedTime += Time.deltaTime;//Acumula cronômetro
            yield return null; // Próximo update
        } //TudoCarregado
        BotaoContinuar.gameObject.SetActive(true); //Reabilita botão de Continuar
        TextCarregando.gameObject.SetActive(false); //Esconde o texto de loading
        RotatingImage.gameObject.SetActive(false); //Esconde a figura de loading
    }
    private void IniciaAudio() //Inicia o AudioSource
    {
        AudioSource meuAudio = ManagerSomSingletonJMF.staticSingleton.GetAudioSource(); // Recupera AudioSource
        if (meuAudio.isPlaying) //Se estiver tocando
            meuAudio.Stop(); //Para
        meuAudio.loop = true; // Ativa Looping de áudio
        meuAudio.volume = GlobalPlayerPrefsJMF.Volume; //Ajusta volume
        meuAudio.mute = GlobalPlayerPrefsJMF.Mudo; //Ajusta mudo ou não
        meuAudio.clip = ManagerSomSingletonJMF.staticSingleton.GetMusic(GlobalPlayerPrefsJMF.IndiceMusica); //Pega música
        meuAudio.Play(); //toca musica
    }
    public IEnumerator VerificaStatus() //Após tempo de espera verifica se está tudo carregado.
    {
        yield return new WaitForSeconds(TempoEspera); //Espera tempo de espera
        while (!LoadSceneManager.CenaCarregada) //Enquanto tem alguma coisa carregando
        {
            yield return null; // volta no próximo updade
        }
        TudoCarregado = true; //Tudo pronto
    }
}
