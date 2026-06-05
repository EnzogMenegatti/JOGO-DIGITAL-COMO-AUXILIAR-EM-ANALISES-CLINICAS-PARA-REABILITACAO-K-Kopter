using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Database;
using System.Collections;

public class PatientDropdownSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown patientDropdown;
    
    // Parallel list to map dropdown index to Firebase unique ID
    private List<string> patientIdsList = new List<string>();

    private void Start()
    {
        if (patientDropdown != null)
        {
            patientDropdown.onValueChanged.RemoveAllListeners();
            patientDropdown.onValueChanged.AddListener(HandleDropdownValueChanged);
        }
    }

    private void OnEnable()
    {
        InitializeSelector();
    }

    public void InitializeSelector()
    {
        StartCoroutine(FetchAndPopulateDropdown());
    }

    private IEnumerator FetchAndPopulateDropdown()
    {
        patientDropdown.ClearOptions();
        patientIdsList.Clear();

        List<string> options = new List<string>();

        FirebaseManager firebaseManager = Object.FindFirstObjectByType<FirebaseManager>();
        if (firebaseManager == null || firebaseManager.DBreference == null || firebaseManager.User == null)
        {
            Debug.LogError("FirebaseManager or user reference is missing!");
            yield break;
        }

        var DBTask = firebaseManager.DBreference.Child("users").Child(firebaseManager.User.UserId).Child("patients").GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError($"Failed to fetch patients for dropdown: {DBTask.Exception}");
            yield break;
        }

        DataSnapshot snapshot = DBTask.Result;
        if (!snapshot.Exists)
        {
            Debug.LogWarning("No patients found to populate dropdown.");
            yield break;
        }

        foreach (DataSnapshot patientRecord in snapshot.Children)
        {
            string jsonText = patientRecord.GetRawJsonValue();
            PatientData patientData = JsonUtility.FromJson<PatientData>(jsonText);

            options.Add(patientData.patientName);
            patientIdsList.Add(patientRecord.Key);
        }

        patientDropdown.AddOptions(options);

        if (!string.IsNullOrEmpty(FirebaseManager.selectedPatientId))
        {
            int savedIndex = patientIdsList.IndexOf(FirebaseManager.selectedPatientId);
            if (savedIndex != -1)
            {
                patientDropdown.value = savedIndex;
            }
        }
        else if (patientIdsList.Count > 0)
        {
            HandleDropdownValueChanged(0);
        }
    }

    private void HandleDropdownValueChanged(int index)
    {
        if (index >= 0 && index < patientIdsList.Count)
        {
            FirebaseManager.selectedPatientId = patientIdsList[index];
            Debug.Log($"Active session patient changed to ID: {FirebaseManager.selectedPatientId}");
        }
    }
}