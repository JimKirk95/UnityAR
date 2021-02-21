using System.Collections;
using UnityEngine;
//*****     *****//
// Gerencia o processamento do csv do Google Sheets
//*****     *****//
public class ManagerGSheetJMF : MonoBehaviour
{
    private const string Verifica = "JMFUTGS"; //Conteúdo da primeira célula da planilha
    private const string BackupFilename = "csvlocal.jmf"; //Nome to arquivo de backup local
    public float Progresso { get; private set; } = 0f;//Percentual já carregado para uso externo
    private bool LoadingCSVGSheet = false;// Flag que indica se está carregando 
    public bool Finalizado { get; private set; } = false;//Percentual já carregado para uso externo
    public void CarregaCSV(bool download = true) // Função pública para disparar carregamento e processamento
    {//Passar false para processamento apenas local
        if (!LoadingCSVGSheet) //Se já não estiver carregando
        {
            Finalizado = false;
            LoadingCSVGSheet = true;//Está carregando
            Progresso = 0f; //Inicia progresso
            if (download) //Atualiza a planilha
            {
                GlobalDownloadGSheetJMF.Status = "Fazendo download da planilha..."; //Altera status
                StartCoroutine(GlobalDownloadGSheetJMF.BaixarCSV(VerificaDownload)); //Manda carregar e Verificar
            }
            else //Usa planilha local
            {
                GlobalDownloadGSheetJMF.Status = "Não atualiza! "; //Altera status
                CarregaCSVLocal(); //Chama método de carregamento de arquivo local
            }
        }
    }
    public void VerificaDownload(string data)//Chamado depois do download em Action callback
    {
        if (string.IsNullOrEmpty(data)) // string null, problema no download
        {
            GlobalDownloadGSheetJMF.Status = "Problemas de conexão! "; //Atualiza Status
            CarregaCSVLocal(); //Chama método de carregamento de arquivo local
        }
        else //Tem donwload, mas talvez não seja a planilha correta
        {
            if (CSVValido(data))//Confere, a string corresponde à planilha
            {
                GlobalDownloadGSheetJMF.DataDownload = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");//Pega data do sistema
                ArquivoBinarioJMF CSVParaSalver = new ArquivoBinarioJMF(GlobalDownloadGSheetJMF.DataDownload, data);//cria variável serializável
                GlobalArquivoBinarioJMF.ArquivoSalvar(CSVParaSalver, BackupFilename); //Salva nova versão do csv
                GlobalPlayerPrefsJMF.AtualizaExecucoes(0); //Indica que conseguiu atualizar nessa execução
                GlobalDownloadGSheetJMF.Status = "Planilha nova "; //Atualiza status
                StartCoroutine(ProcessData(ProcessamentoFinalizado, data));//Chama processamento do .csv
            }
            else //download feito, mas o csv não corresponde à planilha esperada
            {
                GlobalDownloadGSheetJMF.Status = "Download corrompido! "; //Atualiza status
                CarregaCSVLocal(); //Chama método de carregamento de arquivo local
            }
        }
    }
    private bool CSVValido(string csv) //verifica se a string corresponde à planilha desejada
    {
        string checkData = csv.Substring(0, 7); //Pega os 7 primeiros caracteres
        return checkData.Equals(Verifica); //Se for igual, retorna true
    }
    private void CarregaCSVLocal()
    {
        GlobalDownloadGSheetJMF.Status += "Cópia local ";
        ArquivoBinarioJMF SavedDada = GlobalArquivoBinarioJMF.ArquivoRecuperar(BackupFilename); //Tenta carregar arquivo
        if (SavedDada == null)//Não tem arquivo local
        {
            LoadingCSVGSheet = false;//Finalizou carregamento (sem carregar)
            GlobalDownloadGSheetJMF.Status += "não encontrada!";
            GlobalDownloadGSheetJMF.DataDownload = "Nenhuma atualização detectada!";
            GlobalDownloadGSheetJMF.PlanilhaGSheet = null;
            GlobalDownloadGSheetJMF.TemDados = false;
            Finalizado = true;
        }
        else//Dados do arquivo local
        {
            GlobalDownloadGSheetJMF.DataDownload = SavedDada.Info; //Pega data do download
            StartCoroutine(ProcessData(ProcessamentoFinalizado, SavedDada.ExemploString));//Chama processamento do .cs
        }
    }
    public IEnumerator ProcessData(System.Action<string[,]> onCompleted, string data) //Separa .csv em matrix
    {
        string[] DataLinhas = data.Split("\n"[0]);//separa em linhas ('\r') ou (["\r\n"]) 
        string[] LinhaColunas = DataLinhas[0].Split(',');//Separa o cabeçalho em colunas para saber o número de colunas
        int NumeroColunas = LinhaColunas.Length;//Variável auxiliar, guarda o número de colunas
        string[,] Planilha = new string[DataLinhas.Length, NumeroColunas];//Cria planilha
        string[] Colunas; //Variável auxiliar para separação em Colunas
        int nColuna; //indicador da coluna atual - poderia ser criada localmente
        int subIndice; //indicador de conjunto de colunas que está sendo processada podria ser criada localmente
        int offsetInicio; //Variável auxiliar para eliminar strings vazias
        int offsetFim; //Variável auxiliar para eliminar strings vaziar
        Progresso = 0.1f;//Já carregou arquivo, progresso de 10%
        for (int nLinha = 0; nLinha < DataLinhas.Length; nLinha++)//Processa cada uma das linhas
        {
            LinhaColunas = DataLinhas[nLinha].Split('"');//separa usando aspas
            if (LinhaColunas.Length == 1)//não tem aspas, então não tem ',' .
            {
                Colunas = DataLinhas[nLinha].Split(',');//separa colunas usando ','.
                for (nColuna = 0; nColuna < NumeroColunas; nColuna++)//Para cada célula separado
                {
                    Planilha[nLinha, nColuna] = Colunas[nColuna];//Copia dados da célula para Planilha
                }
            }
            else//tem aspas, então tem célula com ','
            {
                nColuna = 0; //Inicia coluna em zero
                subIndice = 0; //Inicia subíndice em zero
                while (nColuna < NumeroColunas)//Enquanto não processou todas as colunas
                {
                    if (subIndice % 2 == 1)//célula que contém ',' e gerou as aspas. Copia inteira
                    {
                        Planilha[nLinha, nColuna] = LinhaColunas[subIndice];//Copia célula que tem ','
                        nColuna++;//incrementa indicador da coluna
                    }
                    else //conjunto de celulas (sem ','), antes e/ou depois de uma célula com ','. Precisa separar
                    {
                        if (LinhaColunas[subIndice].Length > 1)//Não é só uma vírgula, se for ignora
                        {
                            Colunas = LinhaColunas[subIndice].Split(',');//separa celulas individuais                                    
                            offsetInicio = (subIndice == 0) ? 0 : 1;//Se não for o primeiro, pula um
                            offsetFim = (subIndice == LinhaColunas.Length - 1) ? 0 : 1; //se não o último pula um
                            for (int i = offsetInicio; i < Colunas.Length - offsetFim; i++)//para cada célula
                            {
                                Planilha[nLinha, nColuna] = Colunas[i]; //copia célula
                                nColuna++; //próxima célula
                            }
                        }
                    }
                    subIndice++;//próximo subconjunto de células
                    Progresso = (1f + nLinha + (1f*nColuna / NumeroColunas)) / DataLinhas.Length;
                    yield return null;
                }
            }
            Progresso = (2f + nLinha) / DataLinhas.Length;
            yield return null;
        } // separa string .csv em matriz de strings (células)
        onCompleted(Planilha);//Retorna string com conteúdo do .csv ou null
    } // Matriz repassada para a função em callback
    public void ProcessamentoFinalizado(string[,] Planilha) //Finaliza processamento do .csv
    {
        Progresso = 1.0f;//Progresso de 100%
        LoadingCSVGSheet = false;//Finalizou carregamento
        GlobalDownloadGSheetJMF.Status += "processada!";
        GlobalDownloadGSheetJMF.PlanilhaGSheet = Planilha;
        GlobalDownloadGSheetJMF.TemDados = true;
        Finalizado = true;
    }
}