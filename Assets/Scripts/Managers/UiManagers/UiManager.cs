using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //Screen object variables
    [SerializeField] GameObject loginUI;
    [SerializeField] GameObject registerUI;
    [SerializeField] GameObject userDataUi;
    [SerializeField] GameObject userDashboardUi;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instancia já existe, destruindo objeto!");
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
    }

    public void CloseScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        userDataUi.SetActive(false);
        userDashboardUi.SetActive(false);
    }
}