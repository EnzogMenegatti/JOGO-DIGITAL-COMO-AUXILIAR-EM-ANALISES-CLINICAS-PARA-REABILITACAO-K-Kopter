using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private FirebaseManager firebaseManager;

    [SerializeField] GameObject loginUI;
    [SerializeField] GameObject registerUI;
    [SerializeField] GameObject userDataUi;
    [SerializeField] GameObject userDashboardUi;
    [SerializeField] GameObject helpBotUi;
    [SerializeField] GameObject OptionsUi;
    [SerializeField] GameObject levelSelector;
    [SerializeField] GameObject gameMenu;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instancia já existe");
            Destroy(this);
        }
    }

    public void LoginScreen()
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }
    public void RegisterScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }

    public void UserDataScreen()
    {
        userDataUi.SetActive(true);
        loginUI.SetActive(false);
        registerUI.SetActive(false);
    }

    public void UserDashboardScreen()
    {
        userDataUi.SetActive(false);
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDashboardUi.SetActive(true);
        if(firebaseManager != null)
        {
            Debug.Log("Iniciando fetch dos pacientes para o dashboard");
            StartCoroutine(firebaseManager.FetchAndDisplayPatients());
        }
        else
        {
            Debug.LogError("FirebaseManager não está atribuído no UIManager!");
        }
        

    }

    /*public void LevelSelectorScreen()
    {
        levelSelector.SetActive(true);
    }*/

    public void OpenLevelSelectionForPatient(string patientId)
    {
        FirebaseManager.selectedPatientId = patientId;
        
        UnityEngine.Debug.Log($"Paciente {patientId} selecionado. Abrindo seleção de fases...");

        CloseScreen();
        levelSelector.SetActive(true);
    }

    public void HelpBotScreen()
    {
        helpBotUi.SetActive(true);
    }

    public void OptionsScreen()
    {
        OptionsUi.SetActive(true);
    }

    public void GameMenuScreen()
    {
        gameMenu.SetActive(true);
    }


    public void CloseScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDataUi.SetActive(false);
        userDashboardUi.SetActive(false);
        helpBotUi.SetActive(false);
        OptionsUi.SetActive(false);
        levelSelector.SetActive(false);
        gameMenu.SetActive(false);
    }


}