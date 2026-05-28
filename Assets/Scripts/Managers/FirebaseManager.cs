using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine.UI;
using System.ComponentModel;
using System;
using System.Diagnostics;

public class FirebaseManager : MonoBehaviour
{
[Header("Firebase")]
public DependencyStatus dependencyStatus;
public FirebaseAuth auth;    
public FirebaseUser User;
public DatabaseReference DBreference;

[Header("Login")]
public Button loginButton;
public TMP_InputField emailLoginField;
public TMP_InputField passwordLoginField;
public TMP_Text warningLoginText;
public TMP_Text confirmLoginText;


[Header("Register")]
public Button registerButton;
public TMP_InputField usernameRegisterField;
public TMP_InputField emailRegisterField;
public TMP_InputField passwordRegisterField;
public TMP_InputField passwordRegisterVerifyField;
public TMP_Text warningRegisterText;
public TMP_Text confirmRegisterText;

[Header ("Medical History")]
public TMP_InputField pacientName;
public TMP_Dropdown pacientSex;
public TMP_InputField pacientAge;
public TMP_InputField pacientHeight;
public TMP_InputField pacientWeight;
public TMP_InputField pacientNotes;
public TMP_InputField scoreField;
public GameObject scoreElement;
public Transform scoreboardContent;


void Awake()
{
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    {
        dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            UnityEngine.Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
    });
}

private void InitializeFirebase()
{
    UnityEngine.Debug.Log("Setting up Firebase Auth");
    auth = FirebaseAuth.DefaultInstance;
    DBreference = FirebaseDatabase.DefaultInstance.RootReference;
}

public void ClearLoginField()
{
    emailLoginField.text = "";
    passwordLoginField.text = "";
}

public void ClearRegisterField()
{
    emailRegisterField.text = "";
    passwordRegisterField.text = "";
    passwordRegisterVerifyField.text = "";
    usernameRegisterField.text = "";

}

public void LoginButton()
{
    if(auth == null)
        {
            warningLoginText.text = "Inicializando sistema. Aguarde...";
        }

        if(loginButton != null)
        loginButton.interactable = false;
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    
}
//Function for the register button
public void RegisterButton()
{
    StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
}

public void SingOutButton()
{
    auth.SignOut();
    UIManager.instance.LoginScreen();
    ClearLoginField();
    ClearRegisterField();
}



public void SaveDataButton()
{
    StartCoroutine(UpdateUsernameAuth(pacientName.text));
    StartCoroutine(UpdateUsernameDatabase(pacientName.text));
    StartCoroutine(UpdateAgeDatabase(int.Parse(pacientAge.text)));
    StartCoroutine(UpdateUserHeightDatabase(float.Parse(pacientHeight.text)));
    StartCoroutine(UpdateUserWeightDatabase(int.Parse(pacientWeight.text)));
    StartCoroutine(UpdateUserNotes(pacientNotes.text));
}






private IEnumerator Login(string _email, string _password)
{
    Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
    yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

    if (LoginTask.IsCanceled)
    {
        if (loginButton != null) loginButton.interactable = true;
        warningLoginText.text = "Login cancelado (Verifique a conexão).";
        yield break;
    }

    if (LoginTask.IsFaulted)
    {
        UnityEngine.Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
        FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        string message = "Login Failed!";
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                message = "Missing Email";
                break;
            case AuthError.MissingPassword:
                message = "Missing Password";
                break;
            case AuthError.WrongPassword:
                message = "Wrong Password";
                break;
            case AuthError.InvalidEmail:
                message = "Invalid Email";
                break;
            case AuthError.UserNotFound:
                message = "Account does not exist";
                break;
        }
        if (loginButton != null) loginButton.interactable = true;
        warningLoginText.text = message;
        yield break;
    }
    
    {
    if (LoginTask.Result != null && LoginTask.Result.User != null)
{
    User = LoginTask.Result.User;
    UnityEngine.Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
        
    if (warningLoginText != null) warningLoginText.text = "";
    if (confirmLoginText != null) confirmLoginText.text = "Logged In";



    yield return new WaitForSeconds(2);
    pacientName.text = User.DisplayName;
    UIManager.instance.UserDataScreen();
    if (confirmLoginText != null) confirmLoginText.text = "Logged In";
    ClearLoginField();
    ClearRegisterField();


}
else
{
    UnityEngine.Debug.LogError("Login successful but User object is null!");
}
    }
}

private IEnumerator Register(string _email, string _password, string _username)
{
    if (_username == "")
    {
        warningRegisterText.text = "Missing Username";
    }
    else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
    {
        //If the password does not match show a warning
        warningRegisterText.text = "Password Does Not Match!";
    }
    else 
    {
        Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

        if (RegisterTask.IsCanceled)
        {
            if (registerButton != null) registerButton.interactable = true;
            warningLoginText.text = "Cadastro Cancelado (Verifique a conexão).";
            yield break;
        }

        if (RegisterTask.IsFaulted)
        {
            UnityEngine.Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Register Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }
            if (registerButton != null) registerButton.interactable = true;
            warningRegisterText.text = message;
        }
        else
        {
            User = RegisterTask.Result.User;

            if (User != null)
            {
                UserProfile profile = new UserProfile{DisplayName = _username};

                    
                Task ProfileTask = User.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                if (ProfileTask.Exception != null)
                {
                    UnityEngine.Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                    FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    warningRegisterText.text = "Username Set Failed!";
                }
                else
                {
                    UIManager.instance.LoginScreen();
                    confirmRegisterText.text = "Cadastro Bem sucedido";
                    ClearLoginField();
                    ClearRegisterField();
                }
            }
        }
    }
}

private IEnumerator UpdateUsernameAuth(string _pacientname)
{
    UserProfile profile = new UserProfile { DisplayName = _pacientname };

    var ProfileTask = User.UpdateUserProfileAsync(profile);
    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

    if (ProfileTask.Exception != null)
    {
        UnityEngine.Debug.LogWarning(message: $"Falha em registrar tarefa com {ProfileTask.Exception}");
    }
    else
    {
        UnityEngine.Debug.LogWarning(message: "Username Atualizado");
    }
}
    private IEnumerator UpdateUsernameDatabase(string _pacientname) {
    
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("pacient name").SetValueAsync(_pacientname);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            UnityEngine.Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            UnityEngine.Debug.LogWarning(message: "Username no banco de dados Atualizado");
        }
    }
    private IEnumerator UpdateAgeDatabase(int _age) {
    
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("age").SetValueAsync(_age);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask != null)
        {
            UnityEngine.Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            UnityEngine.Debug.LogWarning(message: "Username no banco de dados Atualizado");
        }
    }
    private IEnumerator UpdateUserHeightDatabase(float _height) {
    
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("height").SetValueAsync(_height);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            UnityEngine.Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            UnityEngine.Debug.LogWarning(message: "Username no banco de dados Atualizado");
        }
    }
    private IEnumerator UpdateUserWeightDatabase(int _weight) {
    
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("weight").SetValueAsync(_weight);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            UnityEngine.Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            UnityEngine.Debug.LogWarning(message: "Username no banco de dados Atualizado");
        }
    }
    private IEnumerator UpdateUserNotes(string _notes) {
    
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("pacientNotes:").SetValueAsync(_notes);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            UnityEngine.Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            UnityEngine.Debug.LogWarning(message: "Username no banco de dados Atualizado");
        }
    }
}