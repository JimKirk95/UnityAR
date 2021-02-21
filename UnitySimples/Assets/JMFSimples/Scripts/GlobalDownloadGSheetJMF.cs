using UnityEngine;
using UnityEngine.Networking; // Para carregar do Google Sheets
using System.Collections; // Para usar IEnumarator com Callback
// *****           *****//
// Método static que faz Download de uma planilha em .csv e retorna uma string em Action callback para processamento
// Também armazena a planilha depois de processada (essa parte poderia estar em outro lugar)
// *****           *****//
public class GlobalDownloadGSheetJMF
{
    // Para usar sua própria planilha Google, COPIE o identificador do link de compartilhamento
    // EXEMPLO:
    //    Link de compartilhamento:
    //    https://docs.google.com/spreadsheets/d/1Dk26trV96k0SpyfmUSybzpSWf-vYrtK2opGDX1cdv8E/edit?usp=sharing
    //    Esse aqui é o id:                      1Dk26trV96k0SpyfmUSybzpSWf-vYrtK2opGDX1cdv8E
    //    Copie o id do compartilhamento de sua planilha para a próxima linha:
    private const string GSheetID = "1Dk26trV96k0SpyfmUSybzpSWf-vYrtK2opGDX1cdv8E";//ID da planilha, copiado do link
    private const string url = "https://docs.google.com/spreadsheets/d/" + GSheetID + "/export?format=csv";//url completa
    //Aqui uma pequena maracutaia, vou aproveitar o mesmo script para armazenar as informações da planilha:
    public static bool TemDados = false; //Indica se tem informações vinda da planilha (atualizado externamente)
    public static string Status = "";
    public static string DataDownload = "Nenhuma atualização detectada!";
    public static string[,] PlanilhaGSheet; //Cópia da planilha do Google Sheets em forma de Matriz de Strings (atualizado externamente)
    internal static IEnumerator BaixarCSV(System.Action<string> onDownloaded) //Download da planilha como corotina
    { //Ao final chama o método passado como parâmetro com a string
        yield return new WaitForEndOfFrame();//Espera uma atualização
        string arquivoCSV = null;//String de retorno
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))//Cria conexão com a Internet
        {
            yield return webRequest.SendWebRequest();//Pede para baixar arquivo csv
            if ((webRequest.result == UnityWebRequest.Result.ConnectionError))//Não conseguiu baixar
            {
                arquivoCSV = null;//vai retornar null
            }
            else //Conseguiu baixar
            {
                arquivoCSV = webRequest.downloadHandler.text; //Copia para variável de retorno
            }
        }
        onDownloaded(arquivoCSV);//Chama a função em callback passando a string (que pode ser null)
    }
}