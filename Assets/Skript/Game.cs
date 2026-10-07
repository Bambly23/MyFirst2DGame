using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Game : MonoBehaviour
{
    [SerializeField] int Score;
    public int[] CostInt;
    private int ClickScore = 1;
    public int[] CostBonus;

    private bool isDataLoaded = false;
    private bool isGameReady = false;
    private float autoSaveTimer = 0f;
    private const float AUTO_SAVE_INTERVAL = 3f;

    private float leaderbordTimer = 0f;
    private const float LEADERBOARD_UPDATE_INTERVAL = 30f;

    public GameObject ShopPan;
    public GameObject BonusPan;
    public GameObject AchievementsPanel;

    [Header("Персонаж")]
    public GameObject petPanel;
    public Image petImage;
    public Slider petProgressBar;
    public Text petLevelText;
    public Text petBonusText;
    public Text petNextLevelText;
    public Button petUpgradeButton;
    public Sprite[] petSpritesUI;

    private int[] petXPNeeded = new int[] { 500, 5000, 50000, 500000, 1000000};
    private int[] petBonusPerLevel = new int[] { 0, 2, 4, 16, 64};
    public string[] petNames = new string[]
    {
        "Яйцо",
        "Детеныш",
        "Юный",
        "Взрослый",
        "Древний",
    };

    public Text[] CostText;
    public Text ScoreText;

    private int Achievement1Max;

    private bool isAchievement1 = true;
    private bool isAchievement2 = true;
    private bool isAchievement2Get = false;
    private bool isAchievement3 = true;
    private bool isAchievement3Get = false;
    private bool isAchievement4Completed = false;
    private bool isAchievement4RewardTaken = false;
    private bool isAchievement5Completed = false;
    private bool isAchievement5RewardTaken = false;
    private bool isAchievement6Completed = false;
    private bool isAchievement6RewardTaken = false;

    private int achievement4Threshold = 45000;
    private int achievement5Threshold = 200000;
    private int achievement6Threshold = 5000000;

    private int achievement4Reward = 55000;
    private int achievement5Reward = 400000;
    private int achievement6Reward = 20000000;

    public Text[] AchievementsText;
    public Text[] AchievementsCost;
    public Text Achievement1NameText;

    private void Start()
    {
        YG2.SwitchLanguage(YG2.lang);

        isGameReady = false;
        isDataLoaded = false;

        YG2.onGetSDKData += OnDataLoaded;

        if (YG2.isSDKEnabled)
        {
            OnDataLoaded();
        }
        else
        {
            StartCoroutine(WaitForSDKAndLoad());
        }
    }

    private IEnumerator WaitForSDKAndLoad()
    {
        float timeout = 5f;
        float elapsed = 0f;

        while (!YG2.isSDKEnabled && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (YG2.isSDKEnabled)
        {
            OnDataLoaded();
        }
        else
        {
            Debug.LogWarning("SDK не инициализировался, использую резервное сохранение");
            TryLoadFromPlayerPrefs();
            isDataLoaded = true;
            isGameReady = true;
            StartCoroutine(BonusShop());
            YG2.GameReadyAPI();
            UpdateAllUITexts();
            UpdatePetUI();
        }
    }

    private void TryLoadFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("backup_score"))
        {
            Debug.Log("Нет резервной копии в PlayerPrefs");
            return;
        }

        int backupScore = PlayerPrefs.GetInt("backup_score", 0);
        int currentScore = Score;

        Debug.Log($"PlayerPrefs Score={backupScore}, текущий Score={currentScore}");

        if (backupScore > currentScore)
        {
            Debug.Log("Загружаем из резервной копии PlayerPrefs (она новее)");

            Score = backupScore;
            ClickScore = PlayerPrefs.GetInt("backup_clickscore", 1);
            Achievement1Max = PlayerPrefs.GetInt("backup_ach1max", 0);

            isAchievement1 = PlayerPrefs.GetInt("backup_ach1", 1) == 1;
            isAchievement2 = PlayerPrefs.GetInt("backup_ach2", 1) == 1;
            isAchievement2Get = PlayerPrefs.GetInt("backup_ach2get", 0) == 1;
            isAchievement3 = PlayerPrefs.GetInt("backup_ach3", 1) == 1;
            isAchievement3Get = PlayerPrefs.GetInt("backup_ach3get", 0) == 1;

            if (CostInt == null) CostInt = new int[2];
            CostInt[0] = PlayerPrefs.GetInt("backup_costint0", 500);
            CostInt[1] = PlayerPrefs.GetInt("backup_costint1", 5000);

            if (CostBonus == null) CostBonus = new int[1];
            CostBonus[0] = PlayerPrefs.GetInt("backup_costbonus0", 0);

            YG2.saves.petLevel = PlayerPrefs.GetInt("backup_petlevel", 0);
            YG2.saves.petXP = PlayerPrefs.GetInt("backup_petxp", 0);
            YG2.saves.petBonus = PlayerPrefs.GetInt("backup_petbonus", 0);

            YG2.saves.Score = Score;
            YG2.saves.ClickScore = ClickScore;
            YG2.saves.Achievement1Max = Achievement1Max;

            UpdateAllUITexts();
            UpdatePetUI();

            Save();
        }
    }

   

    public void OnDataLoaded()
    {
        if (isDataLoaded) return;

        Debug.Log("OnDataLoaded: Данные получены");

        LoadFromYandexSaves();

        TryLoadFromPlayerPrefs();

        isDataLoaded = true;
        isGameReady = true;
        StartCoroutine(BonusShop());
        YG2.GameReadyAPI();
        UpdateAllUITexts();
        UpdatePetUI();
        Save();
    }
    private void LoadFromYandexSaves()
    {
        if (YG2.saves == null)
        {
            Debug.LogError("YG2.saves null!");
            return;
        }

        Score = YG2.saves.Score;
        ClickScore = YG2.saves.ClickScore;
        Achievement1Max = YG2.saves.Achievement1Max;
        isAchievement1 = YG2.saves.isAchievement1;
        isAchievement2 = YG2.saves.isAchievement2;
        isAchievement2Get = YG2.saves.isAchievement2Get;
        isAchievement3 = YG2.saves.isAchievement3;
        isAchievement3Get = YG2.saves.isAchievement3Get;
        isAchievement4Completed = YG2.saves.isAchievement4Completed;
        isAchievement4RewardTaken = YG2.saves.isAchievement4RewardTaken;
        isAchievement5Completed = YG2.saves.isAchievement5Completed;
        isAchievement5RewardTaken = YG2.saves.isAchievement5RewardTaken;
        isAchievement6Completed = YG2.saves.isAchievement6Completed;
        isAchievement6RewardTaken = YG2.saves.isAchievement6RewardTaken;

        if (YG2.saves.CostInt != null && YG2.saves.CostInt.Length > 0)
        {
            CostInt = YG2.saves.CostInt;
        }
        else
        {
            CostInt = new int[] { 500, 5000 };
        }

        if (YG2.saves.CostBonus != null && YG2.saves.CostBonus.Length > 0)
        {
            CostBonus = YG2.saves.CostBonus;
        }
        else
        {
            CostBonus = new int[] { 0 };
        }

        if (CostText != null && CostText.Length >= 2)
        {
            CostText[0].text = CostInt[0] + "$";
            CostText[1].text = CostInt[1] + "$";
        }

        if (YG2.saves.petLevel == 0 && YG2.saves.petXP == 0)
        {
            YG2.saves.petLevel = 0;
            YG2.saves.petXP = 0;
            YG2.saves.petBonus = petBonusPerLevel[0];
        }

        Debug.Log($"Загружено из облака: Score={Score}, ClickScore={ClickScore}");
    }

    private void Save()
    {
        if (!isDataLoaded || YG2.saves == null) return;

        YG2.saves.Score = Score;
        YG2.saves.ClickScore = ClickScore;
        YG2.saves.Achievement1Max = Achievement1Max;
        YG2.saves.isAchievement1 = isAchievement1;
        YG2.saves.isAchievement2 = isAchievement2;
        YG2.saves.isAchievement2Get = isAchievement2Get;
        YG2.saves.isAchievement3 = isAchievement3;
        YG2.saves.isAchievement3Get = isAchievement3Get;
        YG2.saves.isAchievement4Completed = isAchievement4Completed;
        YG2.saves.isAchievement4RewardTaken = isAchievement4RewardTaken;
        YG2.saves.isAchievement5Completed = isAchievement5Completed;
        YG2.saves.isAchievement5RewardTaken = isAchievement5RewardTaken;
        YG2.saves.isAchievement6Completed = isAchievement6Completed;
        YG2.saves.isAchievement6RewardTaken = isAchievement6RewardTaken;
        YG2.saves.CostInt = CostInt;
        YG2.saves.CostBonus = CostBonus;

        YG2.SaveProgress();

        YandexSaveFix.BackupToPlayerPrefs();

        Debug.Log($"Сохранено; Score={Score}");
    }

    public void UpdateAllUITexts()
    {
        string currentLang = YG2.lang;

        if (currentLang == "ru")
        {
            Achievement1NameText.text = "Нажмите " + Achievement1Max + "/500";
            AchievementsCost[0].text = isAchievement1 ? "500$" : "Получено";
            AchievementsCost[1].text = isAchievement2Get ? "Получено" : "600$";
            AchievementsCost[2].text = isAchievement3Get ? "Получено" : "8000$";
            AchievementsCost[3].text = isAchievement4RewardTaken ? "Получено" : $"{achievement4Reward}$";
            AchievementsCost[4].text = isAchievement5RewardTaken ? "Получено" : $"{achievement5Reward}$";
            AchievementsCost[5].text = isAchievement6RewardTaken ? "Получено" : $"{achievement6Reward}$";
        }
        else if (currentLang == "en")
        {
            Achievement1NameText.text = "Click " + Achievement1Max + "/500";
            AchievementsCost[0].text = isAchievement1 ? "500$" : "Received";
            AchievementsCost[1].text = isAchievement2Get ? "Received" : "600$";
            AchievementsCost[2].text = isAchievement3Get ? "Received" : "8000$";
            AchievementsCost[3].text = isAchievement4RewardTaken ? "Received" : $"{achievement4Reward}$";
            AchievementsCost[4].text = isAchievement5RewardTaken ? "Received" : $"{achievement5Reward}$";
            AchievementsCost[5].text = isAchievement6RewardTaken ? "Received" : $"{achievement6Reward}$";
        }

        UpdatePetUITexts(currentLang);
    }

    private void UpdatePetUITexts(string lang)
    {
        if (lang == "ru")
        {
            petNames[0] = "Яйцо";
            petNames[1] = "Детеныш";
            petNames[2] = "Юный";
            petNames[3] = "Взрослый";
            petNames[4] = "Древний";
        }
        else if (lang == "en")
        {
            petNames[0] = "Egg";
            petNames[1] = "Hatchling";
            petNames[2] = "Young";
            petNames[3] = "Adult";
            petNames[4] = "Ancient";
        }

        if (YG2.saves.petLevel < petNames.Length)
        {
            petLevelText.text = petNames[YG2.saves.petLevel];
        }
    }

    private void AddPetXP(int amount)
    {
        if (YG2.saves.petLevel >= petNames.Length - 1) return;

        YG2.saves.petXP += amount;

        UpdatePetUI();
    }

    private void UpdatePetUI()
    {
        int currentLevel = YG2.saves.petLevel;
        bool isMaxLevel = currentLevel >= petNames.Length - 1;
        string currentLang = YG2.lang;

        string maxLevelText = currentLang == "ru" ? "МАКС. УРОВЕНЬ" : "MAX LEVEL";

        if (isMaxLevel)
        {
            petNextLevelText.text = maxLevelText;
            petUpgradeButton.gameObject.SetActive(false);
        }
        else
        {
            petProgressBar.gameObject.SetActive(true);
            int currentXPNeeded = petXPNeeded[YG2.saves.petLevel];
            int currentXP = YG2.saves.petXP;

            petProgressBar.maxValue = currentXPNeeded;
            petProgressBar.value = currentXP;

            petNextLevelText.text = $"{currentXP}/{currentXPNeeded}";
            bool canLevelUp = currentXP >= currentXPNeeded && YG2.saves.petLevel < petXPNeeded.Length - 1;
            petUpgradeButton.gameObject.SetActive(canLevelUp);
        }

        petLevelText.text = $"{petNames[YG2.saves.petLevel]}";
        petBonusText.text = $"+ {YG2.saves.petBonus}";

        if (petSpritesUI != null && petSpritesUI.Length > currentLevel)
        {
            petImage.sprite = petSpritesUI[currentLevel];
        }
    }

    public void OnClickPetUpgrade()
    {
        int currentLevel = YG2.saves.petLevel;
        int neededXP = petXPNeeded[currentLevel];

        if(currentLevel >= petNames.Length - 1) return;

        if (YG2.saves.petXP >= neededXP && currentLevel < petXPNeeded.Length - 1)
        {
            YG2.saves.petLevel++;
            YG2.saves.petXP -= neededXP;

            int newLevel = YG2.saves.petLevel;
            YG2.saves.petBonus = petBonusPerLevel[newLevel];

            ClickScore += (YG2.saves.petBonus - petBonusPerLevel[currentLevel]);

            StartCoroutine(PetEvolutionAnimation());

            Save();

            UpdatePetUI();
        }
    }

    IEnumerator PetEvolutionAnimation()
    {
        petImage.color = Color.white;

        yield return new WaitForSeconds(0.1f);

        float duration = 0.5f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float scale = 1 + Mathf.Sin(elapsed * 20f) * 0.1f;
            petImage.transform.localScale = new Vector3 (scale, scale, 1);
            elapsed += Time.deltaTime;
            yield return null;
        }

        petImage.transform.localScale = Vector3.one;
    }

    public void OnClickButton()
    {
        if (!isGameReady) return;

        Score += ClickScore;

        AddPetXP(ClickScore);

        if (isAchievement1 == true && Achievement1Max < 500)
        {
            Achievement1Max++;
            UpdateAllUITexts();
        }
        Save();
    }

    private void Update()
    {
        if (!isDataLoaded || !isGameReady || YG2.saves == null) return;

        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= AUTO_SAVE_INTERVAL)
        {
            autoSaveTimer = 0f;
            Save();
        }

        ScoreText.text = Score + "$";

        leaderbordTimer += Time.deltaTime;
        if (leaderbordTimer >= LEADERBOARD_UPDATE_INTERVAL)
        {
            leaderbordTimer = 0f;
            YG2.SetLeaderboard("leader", Score);
        }

        string currentLang = YG2.lang;

        if (isAchievement1 == false)
        {
            AchievementsCost[0].text = (currentLang == "ru") ? "Получено" : "Received";
        }

        if (Achievement1Max == 500)
        {
            AchievementsText[0].text = (currentLang == "ru") ? "Выполнено" : "Completed";
        }

        if (isAchievement2Get == true)
        {
            AchievementsCost[1].text = (currentLang == "ru") ? "Получено" : "Received";
        }

        if (isAchievement2 == false)
        {
            AchievementsText[1].text = (currentLang == "ru") ? "Выполнено" : "Completed";
        }

        if (isAchievement3Get == true)
        {
            AchievementsCost[2].text = (currentLang == "ru") ? "Получено" : "Received";
        }

        if (isAchievement3 == false)
        {
            AchievementsText[2].text = (currentLang == "ru") ? "Выполнено" : "Completed";
        }

        if (Score >= achievement4Threshold)
        {
            isAchievement4Completed = true;
        }

        if (isAchievement4RewardTaken || isAchievement4Completed)
        {
            AchievementsText[3].text = (currentLang == "ru") ? "Выполнено" : "Completed";
        }

        if (isAchievement4RewardTaken)
        {
            AchievementsCost[3].text = (currentLang == "ru") ? "Получено" : "Received";
        }

        if (Score >= achievement5Threshold)
        {
            isAchievement5Completed = true;
        }

        if (isAchievement5RewardTaken || isAchievement5Completed)
        {
            AchievementsText[4].text = (currentLang == "ru") ? "Выполнено" : "Completed";
        }

        if (isAchievement5RewardTaken)
        {
            AchievementsCost[4].text = (currentLang == "ru") ? "Получено" : "Received";
        }

        if (Score >= achievement6Threshold)
        {
            isAchievement6Completed = true;
        }

        if (isAchievement6RewardTaken || isAchievement6Completed)
        {
            AchievementsText[5].text = (currentLang == "ru") ? "Выполнено" : "Completed";
        }

        if (isAchievement6RewardTaken)
        {
            AchievementsCost[5].text = (currentLang == "ru") ? "Получено" : "Received";
        }
    }

    public void ShowAndHideShopPan()
    {
        ShopPan.SetActive(!ShopPan.activeSelf);
    }
    public void ShowAndHideBonusPan()
    {
        BonusPan.SetActive(!BonusPan.activeSelf);
    }
    public void ShowAndHideAchievementsPanel()
    {
        AchievementsPanel.SetActive(!AchievementsPanel.activeSelf);
    }

    public void OnClickBuyLevel()
    {
        if (!isGameReady) return;

        if (Score >= CostInt[0])
        {
            Score -= CostInt[0];
            CostInt[0] *= 2;
            ClickScore *= 2;
            CostText[0].text = CostInt[0] + "$";
            isAchievement2 = false;
            Save();
        }
    }

    public void OnClickBuyBonusShop()
    {
        if (!isGameReady) return;

        if (Score >= CostInt[1])
        {
            Score -= CostInt[1];
            CostInt[1] *= 2;
            CostBonus[0] += 2;
            CostText[1].text = CostInt[1] + "$";
            isAchievement3 = false;
            Save();
        }
    }

    IEnumerator BonusShop()
    {
        while (true)
        {
            yield return new WaitUntil(() => isDataLoaded && isGameReady);
            yield return new WaitForSeconds(1);

            Score += CostBonus[0];

            AddPetXP(CostBonus[0]);

            Save();
        }
    }

    public void OnClickAchievement1Button()
    {
        if (!isGameReady) return;

        if (isAchievement1 == true && Achievement1Max == 500)
        {
            Score += 500;
            AddPetXP(500);
            isAchievement1 = false;
            Save();
        }
    }
    public void OnClickAchievement2Button()
    {
        if (isAchievement2 == false && isAchievement2Get == false)
        {
            Score += 600;
            AddPetXP(600);
            isAchievement2Get = true;
            Save();
        }
    }

    public void OnClickAchievement3Button()
    {
        if (isAchievement3 == false && isAchievement3Get == false)
        {
            Score += 8000;
            AddPetXP(8000);
            isAchievement3Get = true;
            Save();
        }
    }

    public void OnClickAchievement4Button()
    {
        if (isAchievement4RewardTaken)
            return;

        if (isAchievement4Completed && !isAchievement4RewardTaken)
        {
            Score += achievement4Reward;

            AddPetXP(achievement4Reward);

            AchievementsCost[3].text = "Получено";
            isAchievement4RewardTaken = true;
            Save();
        }
    }

    public void OnClickAchievement5Button()
    {
        if (isAchievement5RewardTaken)
            return;

        if (isAchievement5Completed && !isAchievement5RewardTaken)
        {
            Score += achievement5Reward;

            AddPetXP(achievement5Reward);

            AchievementsCost[4].text = "Получено";
            isAchievement5RewardTaken = true;
            Save();
        }
    }

    public void OnClickAchievement6Button()
    {
        if (isAchievement6RewardTaken)
            return;

        if (isAchievement6Completed && !isAchievement6RewardTaken)
        {
            Score += achievement6Reward;

            AddPetXP(achievement6Reward);

            AchievementsCost[5].text = "Получено";
            isAchievement6RewardTaken = true;
            Save();
        }
    }

    private void OnDestroy()
    {
        Save();
        YG2.SetLeaderboard("leader", Score);
    }
}