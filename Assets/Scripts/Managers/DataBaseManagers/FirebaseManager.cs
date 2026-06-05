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

[Header("Dashboard Settings")]
public GameObject patientCardPrefab;
public Transform cardsContainer;
public static string selectedPatientId;


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
public void ClearPatientFields()
{
    pacientName.text = "";
    pacientAge.text = "";
    pacientHeight.text = "";
    pacientWeight.text = "";
    pacientNotes.text = "";
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

public void ExportPatientDataButton(string patientId)
{
    StartCoroutine(FetchPatientAndExport(patientId));
}

public void SaveGameScore(int score, string phaseName)
{
    StartCoroutine(SaveScoreCoroutine(score, phaseName));
}

public void LoadDashboard()
{
    StartCoroutine(FetchAndDisplayPatients());
}

public void ChangePasswordButton(string newPassword)
{
    StartCoroutine(UpdatePasswordCoroutine(newPassword));
}

public void ResetPasswordButton(string emailAddress)
{
    if (string.IsNullOrEmpty(emailAddress))
    {
        warningLoginText.text = "Digite seu e-mail para recuperar a senha.";
        return;
    }

    StartCoroutine(SendPasswordResetEmail(emailAddress));
}

public void SaveDataButton()
{
    if (string.IsNullOrEmpty(pacientAge.text) || string.IsNullOrEmpty(pacientHeight.text) || string.IsNullOrEmpty(pacientWeight.text))
    {
        UnityEngine.Debug.LogWarning("Por favor, preencha todos os campos de idade, peso e altura!");

        return; 
    }

    StartCoroutine(CreatePatient(
        pacientName.text, 
        pacientSex.options[pacientSex.value].text,
        pacientAge.text, 
        pacientHeight.text, 
        pacientWeight.text, 
        pacientNotes.text
    ));
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
        UnityEngine.Debug.LogWarning(message: $"Falha ao conectar usuario: {LoginTask.Exception}");

        FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;

        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        string message = "Login Failed!";
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                message = "Email em falta";
                break;
            case AuthError.MissingPassword:
                message = "Senha em falta";
                break;
            case AuthError.WrongPassword:
                message = "Senha incorreta";
                break;
            case AuthError.InvalidEmail:
                message = "Email inválido";
                break;
            case AuthError.UserNotFound:
                message = "Conta não existe";
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
    if (User.IsEmailVerified){
    UnityEngine.Debug.LogFormat("Usuario conectado: {0} ({1})", User.DisplayName, User.Email);
        
    if (warningLoginText != null) warningLoginText.text = "";

    if (confirmLoginText != null) confirmLoginText.text = "Logged In";
    
    yield return new WaitForSeconds(2);
    pacientName.text = "";

    if (confirmLoginText != null) confirmLoginText.text = "Logged In";

    ClearLoginField();
    ClearRegisterField();
    }
    else
    {
        UnityEngine.Debug.LogWarning("Login negado: E-mail não verificado.");
            
            if (warningLoginText != null) 
            warningLoginText.text = "Por favor, valide seu e-mail antes de entrar.";
            if (loginButton != null) 
            loginButton.interactable = true;
            auth.SignOut();
    }
    }
    }
}

private IEnumerator Register(string _email, string _password, string _username)
{
    DatabaseReference dbreference = FirebaseDatabase.DefaultInstance.RootReference;

    Query usernameQuery = dbreference.Child("users").OrderByChild("username").EqualTo(_username);

    Task<DataSnapshot> checkUsernameTask = usernameQuery.GetValueAsync();

    yield return new WaitUntil(predicate: () => checkUsernameTask.IsCompleted);

    if (checkUsernameTask.IsFaulted)
    {
        UnityEngine.Debug.LogError("Falha de conexão");
        warningRegisterText.text = "Erro de conexão";
        yield break;
    }

    DataSnapshot usernameSnapshot = checkUsernameTask.Result;
    if (usernameSnapshot.Exists)
    {
        warningRegisterText.text = "Nome de usuário já existe";
        yield break;
    }
    else if (_username == "")
    {
        warningRegisterText.text = "Nome de usúario faltando";
        yield break;
    }
    else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
    {
        warningRegisterText.text = "Senhas não coencidem";
    }
    else 
    {
        Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

        if (RegisterTask.IsCanceled)
        {
            if (registerButton != null) registerButton.interactable = true;
            warningRegisterText.text = "Cadastro Cancelado (Verifique a conexão).";
            yield break;
        }

        if (RegisterTask.IsFaulted)
        {
            UnityEngine.Debug.LogWarning(message: $"Erro ao cadastrar. Erro: {RegisterTask.Exception}");

            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Falha no cadastro!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Email em falta";
                    break;
                case AuthError.MissingPassword:
                    message = "Senha em falta";
                    break;
                case AuthError.WeakPassword:
                    message = "Senha fraca";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email já em uso";
                    break;
            }
            if (registerButton != null) registerButton.interactable = true;
            warningRegisterText.text = message;
        }
        else
        {
            // CORREÇÃO 1: Atribuição da variável User antes da verificação condicional
            User = RegisterTask.Result.User;

            if (User != null)
            {
                UserProfile profile = new UserProfile { DisplayName = _username };
            
                Task ProfileTask = User.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                if (ProfileTask.Exception != null)
                {
                    UnityEngine.Debug.LogWarning(message: $"Falha ao registrar tarefa: {ProfileTask.Exception}");
                    FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    warningRegisterText.text = "Registro de nome de usuário falhou!";
                }

                // CORREÇÃO 2: Execução única da tarefa de envio do e-mail de verificação
                Task emailTask = User.SendEmailVerificationAsync();

                yield return new WaitUntil(() => emailTask.IsCompleted);
                
                if (emailTask.Exception != null)
                {
                    UnityEngine.Debug.LogWarning($"Falha ao enviar e-mail: {emailTask.Exception}");
                    warningRegisterText.text = "Erro ao enviar e-mail de verificação.";
                }
                else
                {
                    confirmRegisterText.text = "Cadastro concluído! Verifique seu e-mail para ativar a conta.";
                    UIManager.instance.LoginScreen();
                    ClearLoginField();
                    ClearRegisterField();
                    auth.SignOut();
                }
            }
        }
    }
}

private IEnumerator CreatePatient(string _patientname, string _sex, string _age, string _height, string _weight, string _notes) 
{
    string currentDoctorName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;
    
    PatientData newPatient = new PatientData(currentDoctorName, _patientname, _sex, _age, _height, _weight, _notes);
    
    string json = JsonUtility.ToJson(newPatient);

    UnityEngine.Debug.LogWarning("JSON Gerado: " + json);

    string uniquePatientId = DBreference.Child("users").Child(User.UserId).Child("patients").Push().Key;
    var DBTask = DBreference.Child("users").Child(User.UserId).Child("patients").Child(uniquePatientId).SetRawJsonValueAsync(json);

    yield return new WaitUntil(() => DBTask.IsCompleted);

    if (DBTask.Exception != null)
    {
        UnityEngine.Debug.LogWarning($"Failed to register task with {DBTask.Exception}");
    }
    else
    {
        UnityEngine.Debug.LogWarning("Patient data saved to database");
        ClearPatientFields();
        UIManager.instance.CloseScreen();
    }
}

private IEnumerator FetchPatientAndExport(string patientId)
{
    Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).Child("patients").Child(patientId).GetValueAsync();
    PDFExportManager pdfExportManager = new PDFExportManager();
    yield return new WaitUntil(() => DBTask.IsCompleted);

    if (DBTask.Exception != null)
    {
        UnityEngine.Debug.LogWarning($"Erro ao buscar paciente: {DBTask.Exception}");
        yield break;
    }

    DataSnapshot snapshot = DBTask.Result;

    if (snapshot.Exists)
    {
        string json = snapshot.GetRawJsonValue();
        PatientData patientToExport = JsonUtility.FromJson<PatientData>(json);

        PDFExportManager.GeneratePatientReport(patientToExport);
        PDFExportManager.GeneratePatientReport(patientToExport);
    }
    else
    {
        UnityEngine.Debug.LogWarning("Nenhum paciente encontrado com este ID.");
    }
}

public IEnumerator FetchAndDisplayPatients()
{
    // --- O DETETIVE: Verificando quem está vazio antes de começar ---
    if (cardsContainer == null)
    {
        UnityEngine.Debug.LogError("ERRO: O 'cardsContainer' está vazio! Volte na Unity e arraste o Content do ScrollView para o FirebaseManager.");
        yield break; 
    }

    if (patientCardPrefab == null)
    {
        UnityEngine.Debug.LogError("ERRO: O 'patientCardPrefab' está vazio! Arraste o seu Prefab azul para o FirebaseManager na Unity.");
        yield break;
    }

    if (User == null)
    {
        UnityEngine.Debug.LogError("ERRO: O objeto 'User' está vazio! Você tentou abrir o Dashboard sem fazer login antes.");
        yield break; 
    }

    if (DBreference == null)
    {
        UnityEngine.Debug.LogError("ERRO: 'DBreference' está vazio! O banco de dados não conectou.");
        yield break; 
    }

    foreach (Transform child in cardsContainer)
    {
        Destroy(child.gameObject);
    }

    var DBTask = DBreference.Child("users").Child(User.UserId).Child("patients").GetValueAsync();

    yield return new WaitUntil(() => DBTask.IsCompleted);

    if (DBTask.Exception != null)
    {
        UnityEngine.Debug.LogError($"Erro ao baixar pacientes: {DBTask.Exception}");
        yield break;
    }

    DataSnapshot snapshot = DBTask.Result;

    if (!snapshot.Exists)
    {
        UnityEngine.Debug.LogWarning("Nenhum paciente encontrado para este médico.");
        yield break;
    }

    foreach (DataSnapshot patientRecord in snapshot.Children)
    {
        string jsonText = patientRecord.GetRawJsonValue();
        PatientData loadedData = JsonUtility.FromJson<PatientData>(jsonText);

        GameObject newCard = Instantiate(patientCardPrefab, cardsContainer);

        PatientCardUI cardScript = newCard.GetComponent<PatientCardUI>();
        if (cardScript != null)
        {
            cardScript.SetupCard(loadedData);
        }
    }

    UnityEngine.Debug.Log("Dashboard carregado com sucesso!");
}

private IEnumerator SaveScoreCoroutine(int score, string phaseName)
{
    // Verificação de segurança: O jogo tentou salvar o score, mas nenhum paciente foi selecionado?
    if (string.IsNullOrEmpty(selectedPatientId))
    {
        UnityEngine.Debug.LogError("Não foi possível salvar o score: Nenhum paciente selecionado!");
        yield break;
    }

    // 1. Monta a mochila com os dados do desempenho
    ScoreData newScore = new ScoreData(score, phaseName);
    string json = JsonUtility.ToJson(newScore);

    // 2. Caminho do Banco: users -> id_medico -> patients -> id_paciente_selecionado -> scores -> id_unico_do_score
    // Usamos o .Push() aqui para que cada partida seja salva como um novo item no histórico, sem apagar as anteriores!
    string uniqueScoreId = DBreference.Child("users").Child(User.UserId)
                                      .Child("patients").Child(selectedPatientId)
                                      .Child("scores").Push().Key;

    var DBTask = DBreference.Child("users").Child(User.UserId)
                            .Child("patients").Child(selectedPatientId)
                            .Child("scores").Child(uniqueScoreId).SetRawJsonValueAsync(json);

    yield return new WaitUntil(() => DBTask.IsCompleted);

    if (DBTask.Exception != null)
    {
        UnityEngine.Debug.LogError($"Falha ao salvar pontuação: {DBTask.Exception}");
    }
    else
    {
        UnityEngine.Debug.Log($"Score de {score} pontos salvo com sucesso para o paciente atual!");
    }
}

private IEnumerator UpdatePasswordCoroutine(string newPassword)
{
    if (User == null)
    {
        UnityEngine.Debug.LogWarning("Erro: Nenhum usuário logado para trocar a senha.");
        yield break;
    }

    // Nota: UpdatePasswordAsync retorna apenas Task, e não Task<AuthResult>
    Task updateTask = User.UpdatePasswordAsync(newPassword);

    yield return new WaitUntil(() => updateTask.IsCompleted);

    if (updateTask.IsCanceled)
    {
        UnityEngine.Debug.LogWarning("Troca de senha cancelada.");
        yield break;
    }

    if (updateTask.IsFaulted)
    {
        UnityEngine.Debug.LogError($"Falha ao trocar a senha: {updateTask.Exception}");
        
        // Você pode colocar um texto na UI para avisar o usuário do erro
        // warningSettingsText.text = "Erro ao alterar a senha. Tente deslogar e logar novamente.";
        
        yield break;
    }

    UnityEngine.Debug.Log("Senha alterada com sucesso!");
    // confirmSettingsText.text = "Senha atualizada com sucesso!";
}

private IEnumerator SendPasswordResetEmail(string email)
{
    Task resetTask = auth.SendPasswordResetEmailAsync(email);

    yield return new WaitUntil(() => resetTask.IsCompleted);

    if (resetTask.IsCanceled)
    {
        UnityEngine.Debug.LogWarning("Recuperação de senha cancelada.");
        yield break;
    }

    if (resetTask.IsFaulted)
    {
        UnityEngine.Debug.LogError($"Erro ao enviar e-mail de recuperação: {resetTask.Exception}");
        warningLoginText.text = "Erro ao enviar e-mail. Verifique o endereço digitado.";
        yield break;
    }

    UnityEngine.Debug.Log("E-mail de recuperação enviado com sucesso!");
    confirmLoginText.text = "E-mail de recuperação enviado! Verifique sua caixa de entrada.";
}
}