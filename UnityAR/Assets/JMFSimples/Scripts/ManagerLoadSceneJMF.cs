using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; //Para usar o gerenciador de Scenes do Unity
//*****     *****//
// Carrega a próxima scene em background
// Espera comando para mudar de cena
// Se tiver um FadeManager, chama o FadeManager para executar a mudança de cena
// Sem FadeManager, muda a cena diretamente.
//*****     *****//
public class ManagerLoadSceneJMF : MonoBehaviour
{
    [SerializeField] private ManagerFadeJMF GerenciadorDeFade = default; //Gerenciador de Fade para transição suave
    private bool carregandoScene = false; //Indica se já está carregando uma Scene
    private bool mudarScene = false; //Indica que já pode mudar de Scene
    public float PercentualCarregado { get; private set; } = 0f;//Percentual já carregado da Scene para uso externo
    public bool CenaCarregada { get; private set; } = false; //Scene pronta para mudar para uso externo
    void Start()
    {
        carregandoScene = false; // Não está carregando a Scene ainda
        PercentualCarregado = 0f; // 0% Carregado
        CenaCarregada = false; // Não foi carregada
        mudarScene = false; // Não é para mudar ainda
    }
    public void AcaoCarregarProximaScene() //Dispara o carregamento da próxima Scene
    {
        if (carregandoScene != true) //Se já não estiver carregando
        {
            carregandoScene = true; //Indica que já está carregando
            PercentualCarregado = 0f; //Só pra garantir
            CenaCarregada = false; //Só pra garantir
            mudarScene = false; //Só por garantia
            StartCoroutine(CorotinaCarregaProximaScene()); //Dispara carregamendo da próxima Scene
        }
    }
    private IEnumerator CorotinaCarregaProximaScene()//Carrega cena principal em paralelo
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {//Verifica se tem mais Scenes pois vai carregar a próxima
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);//Manda carregar cena
            asyncOperation.allowSceneActivation = false;//Não deixa abrir automaticamente
            yield return null; //Deixa para próxima atualização
            while (!asyncOperation.isDone)//Enquanto está carregando
            {
                PercentualCarregado = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)//Cena pronta
                {
                    CenaCarregada = true;
                    if (mudarScene)//Se a variável foi alterada para true
                    {
                        asyncOperation.allowSceneActivation = true; //Deixa a cena carregar
                        if (GerenciadorDeFade != null && !GerenciadorDeFade.Equals(null))//Se o componente existe
                        { 
                            StartCoroutine(GerenciadorDeFade.CorotinaFadeOut()); //Manda fazer FadeOut
                        }
                    }
                }
                yield return null;//Continua no while na próxima iteração
            }
        }
    }
    public void AcaoMudarDeScene()//Dispa a mudança para a próxima scene
    {
        if (GerenciadorDeFade != null && !GerenciadorDeFade.Equals(null))//Se o componente existe
        {   //Manda fazer FadeIn e ao finalizar chamar AcaoMudarSceneDoFade
            StartCoroutine(GerenciadorDeFade.CorotinaFadeIn(AcaoMudarDeSceneDoFade));
        }
        else //se não tem FadeManager
        {
            mudarScene = true; // Altera diretamente o indicador de mudança de Scene
        }
    }
    public void AcaoMudarDeSceneDoFade() //Retorno do FadeIn
    {
        mudarScene = true; //Altera o indicador de mudança de Scene
    }
}