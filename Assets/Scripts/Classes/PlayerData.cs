using UnityEngine;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct PlayerData {
        public float m_FracHealth;
        public float[] m_Position;
        public float[] m_Rotation;
    }

    public float[] m_CheckpointPosition;
    public float[] m_CheckpointRotation; 
    public PlayerData playerData;

    public string ToJson() {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json) {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveable {
    void PopulateSaveData(SaveData a_saveData);
    void LoadFromSaveData(SaveData a_saveData);
}