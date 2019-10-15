using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;


public class ButtonController : MonoBehaviour {
    public GameObject[] game;
    public Sprite[] MenuButton;
    public GameObject[] FotoMenu;
    public GameObject [] audios;
    public GameObject volume;
    public GameObject Infopage;
    public GameObject Target;
    public static string ScreenshotName;
    private static string myScreenshotLocation;
    public string url = "";
    private static bool ismute = false;
    private static bool isinfopageactive = false;
    void FixedUpdate()
    {
        if (isinfopageactive)
        if (Input.touchCount > 0)
        {
            Infopage.SetActive(false);
            Target.SetActive(true);
        }
    }
    public void Menu()
    {
        foreach(var button in game[0].GetComponentsInChildren<Image>())
        {
            if (button.name == "Menu")
            {
                if(button.GetComponent<Image>().sprite.name == "Group")
                {
                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("menu");
                }else button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Group");
            }
            else button.enabled = !button.enabled;
        }
    }

    public void MuteButton()
    {
        if (ismute)
        {
            volume.GetComponent<Image>().sprite = Resources.Load<Sprite>("audio_on");
            for (int i = 0; i < audios.Length; i++)
            {
                audios[i].GetComponent<AudioSource>().mute = false;
            }
            ismute = false;
        }
        else
        {
            volume.GetComponent<Image>().sprite = Resources.Load<Sprite>("audio_off");
            for (int i = 0; i < audios.Length; i++)
            {
                audios[i].GetComponent<AudioSource>().mute = true;
            }
            ismute = true;
        }
    }

    public void MakeFoto()
    {
        FindObjectOfType<wolf>().foto = true;
        StartCoroutine(FotoDelay());
    }

    public void Back()
    {
        for (int i = 0; i < game.Length; i++)
        {
            game[i].SetActive(true);
        }

        for (int i = 0; i < FotoMenu.Length; i++)
        {
            FotoMenu[i].SetActive(!FotoMenu[i].activeSelf);
        }
    }
    public void Share()
    {
        string shareText = "";
        string imagePath = myScreenshotLocation;
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
        intentObject.Call<AndroidJavaObject>("setType", "image/png");

        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText + "\n" + url);

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "");
        currentActivity.Call("startActivity", jChooser);
    }

    IEnumerator FotoDelay()
    {
        ScreenshotName = "testAPK_Bondarchuk_" + System.DateTime.Now.Year.ToString()+ DateTime.Now.Month.ToString()+ DateTime.Now.Day.ToString()+ DateTime.Now.Hour.ToString()+
            DateTime.Now.Minute.ToString()+ DateTime.Now.Second.ToString()+ DateTime.Now.Millisecond.ToString() + ".png";
        Debug.Log(ScreenshotName);
        string imagePath = Application.persistentDataPath + "/" + ScreenshotName;
        for (int i = 0; i < game.Length; i++)
        {
            game[i].SetActive(false);
        }
        yield return new WaitForEndOfFrame();
        Texture2D  selfie = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        selfie.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);

        selfie.Apply();
        byte[] bytes = selfie.EncodeToPNG();

        string fileLocation = Path.Combine(Application.persistentDataPath, ScreenshotName);
        File.WriteAllBytes(fileLocation, bytes);

        string myFolderLocation = "/mnt/sdcard/DCIM/Camera/";
        myScreenshotLocation = myFolderLocation + ScreenshotName;
        if(!File.Exists(ScreenshotName))
        System.IO.File.Move(fileLocation, myScreenshotLocation);
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classMedia = new AndroidJavaClass("android.media.MediaScannerConnection");
        classMedia.CallStatic("scanFile", new object[4] { objActivity,
        new string[] { myScreenshotLocation },
        new string[] { "image/png" }, null  });

        yield return new WaitForEndOfFrame();
        for (int i = 0; i < FotoMenu.Length; i++)
        {
            FotoMenu[i].SetActive(!FotoMenu[i].activeSelf);
        }
        game[1].SetActive(true);
        FindObjectOfType<wolf>().foto = false;
        //FileInfo file = new FileInfo(imagePath);
        //File.Copy(imagePath, "/dcim/camera" + ScreenshotName);
        //file.CopyTo("/dcim/camera" + ScreenshotName);
    }

    public void Mail()
    {
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        string[] recipient = { "vb@marevo.vision" };
        string subject = "Test My APP";

        /* fixed, is now ACTION_SEND */
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");

        /* fixed, now has GetStatic<string>("EXTRA_EMAIL") */
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_EMAIL"), recipient);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
    }
    public void Info()
    {
        Infopage.SetActive(true);
        Target.SetActive(false);
        isinfopageactive = true;
    }
}
