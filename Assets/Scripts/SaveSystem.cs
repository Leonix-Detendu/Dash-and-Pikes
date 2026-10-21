using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // Emplacement universel et sécurisé (AppData sur Windows, etc.)
    private static readonly string filePath = Path.Combine(Application.persistentDataPath, "DataSaved.json");

    // 1. Fonction pour ENREGISTRER sur le disque
    public static void SaveSettings(SettingsData data)
    {
        try
        {
            // Transforme l'objet C# en texte JSON (true = texte indenté et lisible)
            string json = JsonUtility.ToJson(data, true);

            // Écrit le fichier sur le disque
            File.WriteAllText(filePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur lors de la sauvegarde : {e.Message}");
        }
    }

    // 2. Fonction pour CHARGER depuis le disque
    public static SettingsData LoadSettings()
    {
        // Si le fichier n'existe pas encore (premier lancement), on renvoie les valeurs par défaut
        if (!File.Exists(filePath))
        {
            return new SettingsData();
        }

        try
        {
            // Lit le texte contenu dans le fichier
            string json = File.ReadAllText(filePath);

            // Reconvertit le texte JSON en objet C#
            return JsonUtility.FromJson<SettingsData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur lors du chargement : {e.Message}");
            return new SettingsData();
        }
    }
}
