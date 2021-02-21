using UnityEngine;
//*****     *****//
// Reajusta a altura dos itens de um elemento de UI Dropdown, com base na altura do Label do Dropdown
// Adicione este script ao Label do Dropdown
// Abra o Template -> Viewport -> Content do Dropdown e arraste o Item para a propriedade Item deste Script no Inspector
//*****     *****//
//[ExecuteInEditMode] //Descomente para que os ajustes sejam feitos em modo de edição também.
public class CircunventDropdownJMF : MonoBehaviour
{
    [Tooltip("Template>Viweport>Content>Item")]
    [SerializeField] private RectTransform Item = default; //RectTransform do Item que será ajustado
    private RectTransform rectTransform = null; //Referência para a RectTransform do Label
    private Vector2 newSizeDelta; // Para ajustar a RectTransform do Item
    private void OnValidate()
    {
        rectTransform = gameObject.GetComponent<RectTransform>(); //Copia referência para o RectTransform do Label
    }
    void Awake() //Inicialização da Scene
    {
        rectTransform = gameObject.GetComponent<RectTransform>();  //Copia referência para o RectTransform do Label
    }
    private void OnRectTransformDimensionsChange() //Quando o RectTransform do Label for alterado
    {
        if ((rectTransform != null) && (Item != null)) //Se já tem o Rectransform e o Item
        {
            newSizeDelta.x = Item.sizeDelta.x; //Copia o comprimento do Item
            newSizeDelta.y = rectTransform.rect.height; //Usa a altura do Label
            Item.sizeDelta = newSizeDelta; //Atribui o novo tamanho para o Item
        }
    }
    private void Start() //Se o ExecuteInEditMode estive habilitado pode não ser necessário
    {
        if ((rectTransform != null) && (Item != null)) //Se já tem o Rectransform e o Item
        {
            newSizeDelta.x = Item.sizeDelta.x; //Copia o comprimento do Item
            newSizeDelta.y = rectTransform.rect.height; //Usa a altura do Label
            Item.sizeDelta = newSizeDelta; //Atribui o novo tamanho para o Item
        }
    }
}
