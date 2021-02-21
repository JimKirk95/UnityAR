using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
//*****     *****//
// Scene inicial (Aviso, carregamento)
// - Carrega dados usando GlobalPlayerPrefsJMF e aciona FadeOut
// - Aciona carregamento da Scene principal e do GSheet em segundo plano
// Guando estiver tudo carregado, habilita botão de Continuar
//*****     *****//
public class SceneIntroJMF : MonoBehaviour
{
    [Header("Gerenciadores:")]
    [SerializeField] private ManagerLoadSceneJMF LoadSceneManager = default; //Gerenciador de Scenes
    [SerializeField] private ManagerFadeJMF FadeManager = default; // Gerenciador de Fade
    [SerializeField] private ManagerGSheetJMF GSheetManager = default;  //Carregador de GSheet
    [Header("Carregamento UI:")]
    [SerializeField] private float TempoEspera = 1.5f; // Tempo mínimo de carregamento (para mostrar animação)
    [SerializeField] private Text TextCarregando = default; //Texto carregando...
    [SerializeField] private Image RotatingImage = default; //Imagem de loading rotativa
    [SerializeField] private float RotatingTime = 0.08f; // Tempo para "girar" imagem de carregamento
    [SerializeField] private float RotatingAngle = 45f; // Ângulo para "girar" imagem de carregamento
    [SerializeField] private Button BotaoContinuar = default; // botão habilitado quando a cena fica pronta
    [Header("Textos para PlayerPrefs")]
    [SerializeField] private Text PlayerPrefVolume = default; //Texto para PlayerPref
    [SerializeField] private Text PlayerPrefAtualiza = default; //Texto para PlayerPref
    [SerializeField] private Text PlayerPrefExecucoes = default; //Texto para PlayerPref
    [Header("Textos para CVS load")]
    [SerializeField] private Text CSVLoadStatus = default; //Texto para loadCSV
    [SerializeField] private Text CSVDateLoad = default; //Texto para loadCCSV 
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
        TxtPlayerPrefs(); //Atualiza PlayerPrefs
        LoadSceneManager.AcaoCarregarProximaScene(); //Manda carregar cena
        StartCoroutine(LoadingCSV()); //Inicia carregamento do Google Sheets/CSV
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
    private void TxtPlayerPrefs() //Atualiza txts com informações do PlayerPrefs
    {
        PlayerPrefVolume.text = "Volume: <b>" + ((int)(100 * GlobalPlayerPrefsJMF.Volume)).ToString() + "%</b>";
        if (GlobalPlayerPrefsJMF.Mudo)
        {
            PlayerPrefVolume.text += " - MUDO";
        }
        if (GlobalPlayerPrefsJMF.Nunca)
        {
            PlayerPrefAtualiza.text = "Nunca atualiza";
        }
        else
        {
            if (GlobalPlayerPrefsJMF.Intervalo == 1)
            {
                PlayerPrefAtualiza.text = "Atualiza em <b>todas</b> as execuções";
            }
            else
            {
                PlayerPrefAtualiza.text = "Atualiza a cada: <b> " + GlobalPlayerPrefsJMF.Intervalo.ToString() + " execuções </b>";
            }
        }
        PlayerPrefExecucoes.text = "Execuções desde a última atualização: <b>" + GlobalPlayerPrefsJMF.Execucoes + "</b>";
    }
    public IEnumerator LoadingCSV() //Gerencia o carregamento da planilha
    {
        yield return null; // volta no próximo updade
        if ((!GlobalPlayerPrefsJMF.Nunca) && (GlobalPlayerPrefsJMF.Execucoes >= GlobalPlayerPrefsJMF.Intervalo)) // Vai tentar atualizar o .csv
        {
            GSheetManager.CarregaCSV(); //Manda carregar Google Sheets
        }
        else
        { //Não é para atualizar via download. Usados dados salvos                 
            GSheetManager.CarregaCSV(false); //Manda carregar .csv de arquivo local
        }
        yield return null; // volta no próximo updade        
        while (!GSheetManager.Finalizado)
        {
            CSVLoadStatus.text = "Status CSV: " + GlobalDownloadGSheetJMF.Status;
            yield return null; //Próximo update
        }
        CSVLoadStatus.text = "Status csv: " + GlobalDownloadGSheetJMF.Status;
        CSVDateLoad.text = "Última atualização: " + GlobalDownloadGSheetJMF.DataDownload;
        PlayerPrefExecucoes.text = "Execuções desde a última atualização: <b>" + GlobalPlayerPrefsJMF.Execucoes + "</b>";
    }
    public IEnumerator VerificaStatus() //Após tempo de espera verifica se está tudo carregado.
    {
        yield return new WaitForSeconds(TempoEspera); //Espera tempo de espera
        while ((!GSheetManager.Finalizado) || (!LoadSceneManager.CenaCarregada)) //Enquanto tem alguma coisa carregando
        {
            yield return null; // volta no próximo updade
        }
        TudoCarregado = true; //Tudo pronto
    }
}