using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }

    [SerializeField]private InputField emailInputField;
    [SerializeField]private InputField passwordInputField;
    [SerializeField]private Button signInBtn;

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;

    public static FirebaseUser firebaseUser;

    // Start is called before the first frame update
    void Start()
    {
        signInBtn.interactable = false;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread((task) =>
            {
                var result = task.Result;

                if (result != DependencyStatus.Available)
               {
                    Debug.LogError(message: result.ToString());
                    IsFirebaseReady = false;
                }
                else
                {
                    IsFirebaseReady = true;
                    
                    firebaseApp = FirebaseApp.DefaultInstance;
                    firebaseAuth = FirebaseAuth.DefaultInstance;
                }

                signInBtn.interactable = IsFirebaseReady;
            }
         );
    }

    public void SignIn()
    {
        if (!IsFirebaseReady || IsSignInOnProgress || firebaseUser != null)
        {
            return;
        }
        else
        {

            IsSignInOnProgress = true;
            signInBtn.interactable = false;

            firebaseAuth.SignInWithEmailAndPasswordAsync(emailInputField.text, passwordInputField.text).ContinueWithOnMainThread(
                (task) =>
                {
                    Debug.Log($"Sign in status : {task.Status}");
                    IsSignInOnProgress = false;
                    signInBtn.interactable = true;
                    if (task.IsFaulted)
                    {

                        Debug.LogError(task.Exception);

                    }
                    else if (task.IsCanceled)
                    {
                        Debug.LogError("Sign-in cancled");
                    }
                    else
                    {
                        firebaseUser = task.Result;
                        Debug.Log(firebaseUser.Email);
                        SceneManager.LoadScene("Lobbytest");
                    }
                   

                }
            );
        }
    }
}
