//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;

//public class ShareApp : MonoBehaviour
//{
//    public string gameLink = "https://play.google.com/store/apps/details?id=com.drox.bombbash&hl=en";
//    public void Share()
//    {
//        SoundManager.soundManager.PlayButtonSound();
//        StartCoroutine(LoadAndShareImage());
//    }

//    IEnumerator LoadAndShareImage()
//    {
//        // Load the sprite from Resources (appicon is the name of the sprite)
//        Sprite sprite = Resources.Load<Sprite>("appicon");

//        // Check if the sprite was found
//        if (sprite == null)
//        {
//            Debug.LogError("App icon sprite not found in Resources!");
//            yield break;
//        }

//        // Convert the sprite to Texture2D
//        Texture2D texture = SpriteToTexture(sprite);
//        yield return null;

//        // Save texture to temporary cache
//        string filePath = Path.Combine(Application.temporaryCachePath, "appicon.png");
//        File.WriteAllBytes(filePath, texture.EncodeToPNG());

//        // Use NativeShare to share the image
//        new NativeShare()
//            .AddFile(filePath)
//            .SetText($"Check out this super fun game I’ve been playing! 🎮 Download it for FREE on Google Play and start the adventure now! 🚀👇 \n {gameLink}")
//            .Share();
//    }

//    // Convert sprite to Texture2D
//    private Texture2D SpriteToTexture(Sprite sprite)
//    {
//        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
//        Color[] pixels = sprite.texture.GetPixels(
//            (int)sprite.textureRect.x,
//            (int)sprite.textureRect.y,
//            (int)sprite.textureRect.width,
//            (int)sprite.textureRect.height);
//        texture.SetPixels(pixels);
//        texture.Apply();
//        return texture;
//    }


//    public void ScreenShotShare()
//    {
//        SoundManager.soundManager.PlayButtonSound();

//        StartCoroutine(TakeSSAndShare());
//    }


//    IEnumerator TakeSSAndShare()
//    {
//        yield return new WaitForEndOfFrame();
//        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
//        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
//        ss.Apply();

//        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
//        File.WriteAllBytes(filePath, ss.EncodeToPNG());

//        Destroy(ss);
//        new NativeShare().AddFile(filePath).SetText($"Can you beat my high score of {PlayerPrefs.GetInt("HighScore")} in this amazing game? \n {gameLink}").Share();

//    }
//}