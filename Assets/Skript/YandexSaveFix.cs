using UnityEngine;
using YG;

public class YandexSaveFix : MonoBehaviour
{
    private static YandexSaveFix instance;
    private static float lastBackupTime = 0f;
    private const float BACKUP_INTERVAL = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Time.time - lastBackupTime >= BACKUP_INTERVAL)
        {
            lastBackupTime = Time.time;
            if (YG2.saves != null && YG2.saves.Score > 0)
            {
                BackupToPlayerPrefs();
            }
        }
    }

    public static void BackupToPlayerPrefs()
    {
        if (YG2.saves == null) return;

        PlayerPrefs.SetInt("backup_score", YG2.saves.Score);
        PlayerPrefs.SetInt("backup_clickscore", YG2.saves.ClickScore);
        PlayerPrefs.SetInt("backup_ach1max", YG2.saves.Achievement1Max);

        PlayerPrefs.SetInt("backup_ach1", YG2.saves.isAchievement1 ? 1 : 0);
        PlayerPrefs.SetInt("backup_ach2", YG2.saves.isAchievement2 ? 1 : 0);
        PlayerPrefs.SetInt("backup_ach2get", YG2.saves.isAchievement2Get ? 1 : 0);
        PlayerPrefs.SetInt("backup_ach3", YG2.saves.isAchievement3 ? 1 : 0);
        PlayerPrefs.SetInt("backup_ach3get", YG2.saves.isAchievement3Get ? 1 : 0);

        if (YG2.saves.CostInt != null && YG2.saves.CostInt.Length >= 2)
        {
            PlayerPrefs.SetInt("backup_costint0", YG2.saves.CostInt[0]);
            PlayerPrefs.SetInt("backup_costint1", YG2.saves.CostInt[1]);
        }

        if (YG2.saves.CostBonus != null && YG2.saves.CostBonus.Length >= 1)
        {
            PlayerPrefs.SetInt("backup_costbonus0", YG2.saves.CostBonus[0]);
        }

        PlayerPrefs.SetInt("backup_petlevel", YG2.saves.petLevel);
        PlayerPrefs.SetInt("backup_petxp", YG2.saves.petXP);
        PlayerPrefs.SetInt("backup_petbonus", YG2.saves.petBonus);

        PlayerPrefs.Save();
        Debug.Log($"Резервное копирование: Score={YG2.saves.Score}");
    }

    void OnApplicationPause(bool pause)
    {
        if (pause && YG2.saves != null)
        {
            BackupToPlayerPrefs();
        }
    }

    void OnApplicationQuit()
    {
        if (YG2.saves != null)
        {
            BackupToPlayerPrefs();
        }
    }

    void OnDestroy()
    {
        if (YG2.saves != null)
        {
            BackupToPlayerPrefs();
        }
    }
}