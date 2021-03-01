using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****     *****//
// Scene inicial (Aviso, carregamento)
// - Carrega dados usando GlobalPlayerPrefsJMF e aciona FadeOut
// - Aciona carregamento da Scene principal em segundo plano
// Guando estiver tudo carregado, habilita bot�o de Continuar
// Classe baseada na SceneIntroJMF do TemplateSimples, mas sem GoogleSheets.
//*****     *****//

public class SceneIntroARJMF : MonoBehaviour
{
    [Header("Gerenciadores:")]
    [SerializeField] private ManagerLoadSceneJMF LoadSceneManager = default; //Gerenciador de Scenes
    [SerializeField] private ManagerFadeJMF FadeManager = default; // Gerenciador de Fade
    [Header("Carregamento UI:")]
    [SerializeField] private float TempoEspera = 1.5f; // Tempo m�nimo de carregamento (para mostrar anima��o)
    [SerializeField] private Text TextCarregando = default; //Texto carregando...
    [SerializeField] private Image RotatingImage = default; //Imagem de loading rotativa
    [SerializeField] private float RotatingTime = 0.08f; // Tempo para "girar" imagem de carregamento
    [SerializeField] private float RotatingAngle = 45f; // �ngulo para "girar" imagem de carregamento
    [SerializeField] private Button BotaoContinuar = default; // bot�o habilitado quando a cena fica pronta
    private bool TudoCarregado = false; //Indica quando pode mudar de scene

    void Start()
    {
        TudoCarregado = false; //Flag de tudo pronto para acionar bot�o e parar anima��o
        BotaoContinuar.onClick.AddListener(AcaoLoadProximaScene); //Atribui m�todo para onClick do bot�o
        StartCoroutine(FadeCanvas()); //Inicia o Fade
        StartCoroutine(LoadingAnimation()); // Inicia anima��o de carregamento
        GlobalPlayerPrefsJMF.RecuperaPlayerPrefs(); //Manda carregar PlayerPrefs
        IniciaAudio(); //Inicia Audio usando PlayerPrefs
        GlobalPlayerPrefsJMF.AtualizaExecucoes(GlobalPlayerPrefsJMF.Execucoes + 1);//Atualiza n�mero de execu��es
        LoadSceneManager.AcaoCarregarProximaScene(); //Manda carregar cena
        StartCoroutine(VerificaStatus()); //Inicia corotina de verifica��o
    }
    public void AcaoLoadProximaScene() //Resposta para o bot�o de prosseguir 
    {
        LoadSceneManager.AcaoMudarDeScene();
    }
    public IEnumerator FadeCanvas() //Inicia o FadeCanvas.
    {
        FadeManager.ImediatoFullIn(); //Come�a com tela preta 
        yield return null; //Pr�ximo frame
        StartCoroutine(FadeManager.CorotinaFadeOut());// Manda fazer fadeout
    }
    public IEnumerator LoadingAnimation() //Gerencia a anima��o de carregando...
    {
        BotaoContinuar.gameObject.SetActive(false); //Desabilita bot�o de Continuar
        TextCarregando.gameObject.SetActive(true); //Mostra o texto de loading
        RotatingImage.gameObject.SetActive(true); //Mostra a figura de loading
        float SpinAccumulatedTime = 0f; //Inicia cron�metro do loading
        yield return null; // volta no pr�ximo updade
        while (!TudoCarregado) //Enquanto n�o est� tudo pronto
        {
            if (SpinAccumulatedTime >= RotatingTime) //Se o cron�metro passou do valor
            {
                RotatingImage.rectTransform.Rotate(0f, 0f, -RotatingAngle, Space.Self);//Gira de RotatingAngle
                SpinAccumulatedTime -= RotatingTime; //Subtrai Rotating time do cron�metro para pr�ximo giro
            }
            SpinAccumulatedTime += Time.deltaTime;//Acumula cron�metro
            yield return null; // Pr�ximo update
        } //TudoCarregado
        BotaoContinuar.gameObject.SetActive(true); //Reabilita bot�o de Continuar
        TextCarregando.gameObject.SetActive(false); //Esconde o texto de loading
        RotatingImage.gameObject.SetActive(false); //Esconde a figura de loading
    }
    private void IniciaAudio() //Inicia o AudioSource
    {
        AudioSource meuAudio = ManagerSomSingletonJMF.staticSingleton.GetAudioSource(); // Recupera AudioSource
        if (meuAudio.isPlaying) //Se estiver tocando
            meuAudio.Stop(); //Para
        meuAudio.loop = true; // Ativa Looping de �udio
        meuAudio.volume = GlobalPlayerPrefsJMF.Volume; //Ajusta volume
        meuAudio.mute = GlobalPlayerPrefsJMF.Mudo; //Ajusta mudo ou n�o
        meuAudio.clip = ManagerSomSingletonJMF.staticSingleton.GetMusic(GlobalPlayerPrefsJMF.IndiceMusica); //Pega m�sica
        meuAudio.Play(); //toca musica
    }
    public IEnumerator VerificaStatus() //Ap�s tempo de espera verifica se est� tudo carregado.
    {
        yield return new WaitForSeconds(TempoEspera); //Espera tempo de espera
        while (!LoadSceneManager.CenaCarregada) //Enquanto tem alguma coisa carregando
        {
            yield return null; // volta no pr�ximo updade
        }
        TudoCarregado = true; //Tudo pronto
    }
}
