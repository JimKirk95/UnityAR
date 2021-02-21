// *****           *****//
// Dados para serem serializados e salvos em arquivo binário
// *****           *****//
[System.Serializable] //Para poder serializar para binário
public class ArquivoBinarioJMF //Estrutura Serializável para salvar dados em arquivo binário
{//Por preguiça/facilidade deixei tudo public:
    public string Info; //Informações sobre os dados salvos
    public bool ExemploBool; //Um booleano de teste
    public int ExemploInt; //Um inteiro de teste
    public float ExemploFloat; //Um float de teste
    public string ExemploString; //Uma string de teste
    public ArquivoBinarioJMF() // Construtor vazio 
    {
    Info ="";
    ExemploBool = false;
    ExemploInt = 0;
    ExemploFloat = 0f;
    ExemploString = "";
    }
    public ArquivoBinarioJMF(string newInfo, string newString)//Construtor só string, para .csv
    {
        Info = newInfo;
        ExemploBool = false;
        ExemploInt = 0;
        ExemploFloat = 0f;
        ExemploString = newString;
    }
    public ArquivoBinarioJMF(bool newBool, int newInt, float newFloat, string newString)//Construtor "completo"
    {
        Info = "Default";
        ExemploBool = newBool;
        ExemploInt = newInt;
        ExemploFloat = newFloat;
        ExemploString = newString;
    }
}