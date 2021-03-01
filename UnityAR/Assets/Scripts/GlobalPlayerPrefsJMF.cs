using UnityEngine;
// *****           *****//
// Atributos e métodos "globais" para recuperar e salvar PlayerPrefs
// Poderia deixar outrps Scripts chamarem PlayerPrefs() diretamente, mas poderia haver erro de digitação das "chaves"
// Centralizando o PlayerPrefs garantimos que não haverá nenhum erro de chaves e ainda é possível fazer uma pequena
// verificação dos avalores antes de gravá-los.
// Não depende de nenhum outro Script e é usado pelos scripts que precisar ler ou atualizar as preferências
// *****           *****//
public class GlobalPlayerPrefsJMF
{
    private const string keyVolume = "Volume"; //Chave para armazenar o Volume
    private const string keyMudo = "Mudo";  //Chave para armazenar o Mudo
    private const string keyIndiceMusica = "IndiceMusica";  //Chave para armazenar o índice da música
    private const string keyIntervalo = "Intervalo";  //Chave para armazenar o Intervalo
    private const string keyNunca = "Nunca";  //Chave para armazenar o Nunca
    private const string keyExecucoes = "Execucoes";  //Chave para armazenar o Execucoes
    public static float Volume { get; private set; } = .5f; //Volume em tempo de execução
    public static bool Mudo { get; private set; } = false; //Mudo em tempo de execução
    public static int IndiceMusica { get; private set; } = 0; //Indice em tempo de execução
    public static int Intervalo { get; private set; } = 10; //Intervalo em tempo de execução
    public static bool Nunca { get; private set; } = true; //Nunca em tempo de execução
    public static int Execucoes { get; private set; } = 0; //Número de execuções em tempo de execução
    public static void AtualizaVolume(float novoVolume) // Atualiza e salva volume
    {
        if ((novoVolume >= 0f) && (novoVolume <= 1f)) //Verifica se está entre 0 e 1
        {
            Volume = novoVolume; //Atualiza variável
            PlayerPrefs.SetFloat(keyVolume, Volume); //Salva variável
        }
    }
    public static void AtualizaMudo(bool novoMudo) //Atualiza e salva Mudo 
    {
        Mudo = novoMudo;  //Atualiza variável
        PlayerPrefs.SetInt(keyMudo, Mudo ? 1 : 0); //Converte de bool para int e salva variável
    }
    public static void AtualizaIndiceMusica(int novoIndice) //Atualiza e salva Indice
    {
        if (novoIndice >= 0) // Verifica se não é negativo
        {
            IndiceMusica = novoIndice;
            PlayerPrefs.SetInt(keyIndiceMusica, IndiceMusica);
        }
    }
    public static void AtualizaIntervalo(int novoIntervalo) //Atualiza e salva Intervalo
    {
        if (novoIntervalo > 0) // Verifica se é maior que zero
        {
            Intervalo = novoIntervalo;
            PlayerPrefs.SetInt(keyIntervalo, Intervalo);
        }
    }
    public static void AtualizaNunca(bool novoNunca) //Atualiza e salva Nunca 
    {
        Nunca = novoNunca;  //Atualiza variável
        PlayerPrefs.SetInt(keyNunca, Nunca ? 1 : 0); //Converte de bool para int e salva variável
    }
    public static void AtualizaExecucoes(int novoExecucoes) //Atualiza e salva Execucoes
    {
        if (novoExecucoes >= 0) // Verifica se não é negativo
        {
            Execucoes = novoExecucoes;
            PlayerPrefs.SetInt(keyExecucoes, Execucoes);
        }
    }
    public static void RecuperaPlayerPrefs() //Recupera as preferências
    {
        Volume = PlayerPrefs.GetFloat(keyVolume, .5f);
        Mudo = PlayerPrefs.GetInt(keyMudo, 0) != 0; //Converte de int para bool
        IndiceMusica = PlayerPrefs.GetInt(keyIndiceMusica, 0);
        Intervalo = PlayerPrefs.GetInt(keyIntervalo, 10);
        Nunca = PlayerPrefs.GetInt(keyNunca, 1) != 0;  //Converte de int para bool
        Execucoes = PlayerPrefs.GetInt(keyExecucoes, 0);
    }
}