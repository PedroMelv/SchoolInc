using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppButtonView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI price;

    public void OnProductFetched(Product product)
    {
        if (title != null)
        {
            title.text = product.metadata.localizedTitle;
        }

        if (price != null)
        {
            price.text = product.metadata.localizedPriceString;
        }
    }
}