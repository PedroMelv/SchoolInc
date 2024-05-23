using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeworkHandler : MonoBehaviour, ITimeListener, IBind<HomeworkHandler.HomeworkData>
{
    [SerializeField] private int maxHomeworks;
    private int homeworkCount = 0;

    [SerializeField] private float homeworkFillTimer = 300f;
    [SerializeField] private float homeworkTimer = 1f;

    [SerializeField] private HomeworkData data;

    [SerializeField] private TextMeshProUGUI homeworkCountText;

    [SerializeField] private MinigameHandler[] minigames;

    [field: SerializeField]public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    private void Start()
    {
        
    }

    private void Update()
    {
        homeworkCountText.text = homeworkCount.ToString();

        data.homeworkCount = homeworkCount;
        data.homeworkTimer = homeworkTimer;

        if (homeworkCount >= maxHomeworks) return;
        homeworkTimer -= Time.deltaTime;
        if (homeworkTimer <= 0)
        {
            homeworkCount += 1;
            homeworkTimer = homeworkFillTimer;
        }
    }

    public void TriggerHomework()
    {
        if (homeworkCount <= 0) return;

        minigames[Random.Range(0,minigames.Length)].InitializeMinigame(Mathf.Pow(Random.Range(1f,50f), SchoolsManager.Instance.boughtSchools.Count));

        homeworkCount--;
    }

    public void CalculateTime(double seconds)
    {
        //double timesFilled = (double)seconds / homeworkFillTimer;

        //homeworkCount += Mathf.FloorToInt((float)timesFilled);
        //if(homeworkCount > maxHomeworks)
        //{
        //    homeworkCount = maxHomeworks;
        //}

        //float timePassed = (float)timesFilled - Mathf.FloorToInt((float)timesFilled);

        //homeworkTimer = homeworkFillTimer - (timePassed * homeworkFillTimer);
    }

    public void Bind(HomeworkData data)
    {
        this.data = data;
        this.data.Id = data.Id;


        homeworkCount = data.homeworkCount;
        homeworkTimer = data.homeworkTimer;
    }

    [System.Serializable]
    public class HomeworkData : ISaveable
    {
        public int homeworkCount;
        public float homeworkTimer;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        public void Reset()
        {
            homeworkCount = 0;
            homeworkTimer = 0f;
        }

        public void Reset_Ascended()
        {
        }
    }
}
