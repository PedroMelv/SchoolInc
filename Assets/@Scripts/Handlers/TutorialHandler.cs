using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialHandler : MonoBehaviour, IBind<TutorialData>
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private Animator tutorialArrow;

    [SerializeField] public TutorialData tutorialData;

    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        if(!tutorialData.tutorialShown)
        {
            StartCoroutine(ShowTutorial());
        }
        else
        {
            tutorialText.text = "";
            tutorialArrow.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowTutorial()
    {
        tutorialText.text = "Aperte no botão no centro para iniciar as aulas na escola, enquanto ela está dando aula você pode apertar o botão para fazer ela ir mais rápido";
        
        yield return new WaitForSeconds(10f);

        tutorialText.text = "Aperte na escola para coletar o dinheiro";

        while (GameCurrency.Instance.Currency < 25) yield return null;

        tutorialText.text = "Abra a loja e compre uma melhoria";

        tutorialArrow.SetTrigger("nextStep");

        SchoolAreaStore store = FindObjectOfType<SchoolAreaStore>();

        while (store == null)
        {
            store = FindObjectOfType<SchoolAreaStore>();
            yield return null;
        }

        while (!store.isOpened) yield return null;

        tutorialText.text = "";
        tutorialArrow.SetTrigger("nextStep");
        tutorialArrow.gameObject.SetActive(false);
        tutorialData.tutorialShown = true;

    }

    public void Bind(TutorialData data)
    {
        this.tutorialData = data;
        this.tutorialData.Id = Id;
    }
}

[System.Serializable]
public class TutorialData : ISaveable
{
    public bool tutorialShown;

    [field: SerializeField]public SerializableGuid Id { get; set; }

    public void Reset()
    {
        tutorialShown = false;
    }

    public void Reset_Ascended()
    {
        
    }
}
