using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

using LT = LibreTranslate.Net;
using UnityEngine.Scripting;

#if ENABLE_WINMD_SUPPORT

using Windows.Media.Ocr;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Globalization;

using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.System.Display;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

//[Preserve]

public class OCRHandler : MonoBehaviour
{
    private String _OCROutcome;
    private String _TranslationResult;

    public GameObject TranslationText;

    public GameObject ReadingNotifyContainer;

    public void PrintFeedback()
    {
        print(" \n >>>>>>>>>>_USER WANTS FEEDBACK___>>>>>>>>>>>  \n ");

        //#if ENABLE_WINMD_SUPPORT
            //DoOCR();
            DoTranslate();
        //#endif
    }


    //#if ENABLE_WINMD_SUPPORT
    [Preserve]
    public async void DoTranslate()
    {

        print(" \n >>>>>>>>>>_TRANSLATION_START_ >>>>>>>>>>>  \n ");


        //#if ENABLE_WINMD_SUPPORT
        var LibreTranslate = new LT.LibreTranslate("http://130.82.30.68:5000");
        //var LibreTranslate = new LT.LibreTranslate("http://127.0.0.1:5000");
        System.Collections.Generic.IEnumerable<LT.SupportedLanguages> SupportedLanguages = (IEnumerable<LT.SupportedLanguages>)await LibreTranslate.GetSupportedLanguagesAsync();
        System.Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(SupportedLanguages, Newtonsoft.Json.Formatting.Indented));
        var englishText = "Please read these oranges and apples carefully. "; 
        print("englishText : " + englishText );

        //englishText = _OCROutcome;

        print("englishText from OCR: " + englishText);

        string spanishText = await LibreTranslate.TranslateAsync(new LT.Translate()
            {
                ApiKey = "MySecretApiKey",
                Source = LT.LanguageCode.English,
                Target = LT.LanguageCode.Spanish,
                Text = englishText
            });
        //print("spanishText : " + spanishText + " \n ");

        _TranslationResult = spanishText;
        print("TranslationResult : " + _TranslationResult + " \n ");
        
        print(" \n >>>>>>>>>>_TRANSLATION_END_ >>>>>>>>>>>  \n ");

        TranslationText.GetComponent<TextMeshPro>().text = _TranslationResult;

        //Make translation container visible
        ReadingNotifyContainer.SetActive(true);

        //#endif
    }

    public String getTranslationResult()
    { 
        return _TranslationResult;
    }


//#endif



    // the directive is important because the Windows libraries don't work in Unity's playmode
#if ENABLE_WINMD_SUPPORT

    MediaCapture mediaCapture;

    public async void DoOCR()
    {
        SoftwareBitmap _softwareBitmap = await CaptureSoftwareBitmap();
        OcrResult _ocrResult = await GetOCRResult(_softwareBitmap);
        print(" __OCR__TEXT__:  " + _ocrResult.Text);
        _OCROutcome = _ocrResult.Text;
    }

    private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
    {
        //Debug.Log("MediaCapture failed!", $"Error:\n{errorEventArgs.Message}");
    }

    // adapted from https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/basic-photo-video-and-audio-capture-with-mediacapture
    private async Task<SoftwareBitmap> CaptureSoftwareBitmap()
    {
        mediaCapture = new MediaCapture();
        await mediaCapture.InitializeAsync();
        mediaCapture.Failed += MediaCapture_Failed;

        // Prepare and capture photo
        var lowLagCapture = await mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));

        var capturedPhoto = await lowLagCapture.CaptureAsync();
        var _softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

        await lowLagCapture.FinishAsync();
        return _softwareBitmap;

    }


    private async Task<OcrResult> GetOCRResult(SoftwareBitmap _softwareBitmap)
    {

        // Also other languages are available
        // see https://docs.microsoft.com/en-us/uwp/api/windows.media.ocr.ocrengine.trycreatefromlanguage?view=winrt-22621#windows-media-ocr-ocrengine-trycreatefromlanguage(windows-globalization-language)
        Language ocrLanguage = new Language("en");
        OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(ocrLanguage);
        OcrResult ocrResult = await ocrEngine.RecognizeAsync(_softwareBitmap);

        return ocrResult;

    }

#endif

}