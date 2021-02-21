using UnityEngine;
using System.IO; // Para salvar e abrir arquivo local
using System.Runtime.Serialization.Formatters.Binary; // Para converter de e para binário
// *****           *****//
// Métodos "globais" para salvar e recupera dados em arquivo binário
// Salva e recupera dados de um objeto ArquivoBinarioJMF para arquivo binário
// Se quiser proteger melhor os dados pode usar um checksum, criptografar, ou
// salvar em outra pasta (inclusive fazendo uma duplicata do arquivo para comparação); 
// *****           *****//
public class GlobalArquivoBinarioJMF
{
    private static string CaminhoArquivo = Application.persistentDataPath + "/"; //Caminho para o arquivo, pode alterar
    public static void ArquivoSalvar(ArquivoBinarioJMF serializableData, string fileName)//Salva dados em serializableData
    {
        BinaryFormatter Formatador = new BinaryFormatter();//Conversor para binário
        FileStream stream = new FileStream(CaminhoArquivo + fileName, FileMode.Create);//Acesso ao arquivo
        Formatador.Serialize(stream, serializableData);//Converte e salva no arquivo
        stream.Close(); // Fecha arquivo
    }
    public static ArquivoBinarioJMF ArquivoRecuperar(string fileName)//Carregar dados
    {
        if (File.Exists(CaminhoArquivo + fileName)) // Se o arquivo existe
        {
            BinaryFormatter Formatador = new BinaryFormatter();//Conversor de e para binário
            FileStream stream = new FileStream(CaminhoArquivo + fileName, FileMode.Open);//Acesso ao arquivo
            ArquivoBinarioJMF deserializedData = Formatador.Deserialize(stream) as ArquivoBinarioJMF;//Converte do arquivo          
            stream.Close();//Fecha arquivo
            return deserializedData; //Retorna dados
        }
        else //arquivo não existe, retorna null
        {
            return null; //Quem chamou deve verificar se é null ou não antes de processar a informação.
        }
    }
}
