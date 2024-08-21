using Lacrimosum.Assets;
using TMPro;
using Random = System.Random;

namespace Lacrimosum.Patching;

internal static class MenuMusicReplacer
{
    private static readonly Random Rnd = new();
    
    public static void Init()
    {
        On.MenuManager.Awake += OnMenuAwake;
        RoR2Plugin.ModConsole.LogInfo("MenuMusicReplacer: MenuManager.Awake hooked!");
    }
    
    private static void OnMenuAwake(On.MenuManager.orig_Awake orig, MenuManager self)
    {
        if (RoR2Plugin.ModConfig.AlwaysReplaceMenuMusic.Value)
        {
            ReplaceMenuMusic(self);
            ChangeMenuColors(self);
        }
        else
        {
            var random = Rnd.Next(0, 100);
            if (random < 2)
            {
                ReplaceMenuMusic(self);
                ChangeMenuColors(self);
            }
        }
        
        AddLilGuy(self);
        
        orig(self);
    }

    private static void ChangeMenuColors(MenuManager menu)
    {
        var blue = new Color(0f, 0.9822383f, 1f, 1);
        var menuContainer = menu.menuAnimator.gameObject;
        
        var versionNum = menuContainer.transform.Find("VersionNum").gameObject;
        var versionNumText = versionNum.GetComponent<TextMeshProUGUI>();
        versionNumText.color = blue;
        
        var mainButtons = menuContainer.transform.Find("MainButtons").gameObject;
        var mainButtonsImage = mainButtons.GetComponent<UnityEngine.UI.Image>();
        mainButtonsImage.color = blue;
        
        var headerImage = mainButtons.transform.Find("HeaderImage").gameObject;
        var headerImageImage = headerImage.GetComponent<UnityEngine.UI.Image>();
        headerImageImage.sprite = ModAssets.BlueLogo;
    }
    
    private static void ReplaceMenuMusic(MenuManager menu)
    {
        menu.menuMusic = ModAssets.RoR2MenuMusic;
        var music = menu.GetComponent<AudioSource>();
        music.Stop();
        music.clip = ModAssets.RoR2MenuMusic;
        music.Play();
    }
    
    private static void AddLilGuy(MenuManager menu)
    {
        var menuContainer = menu.menuAnimator.gameObject;
        var mainButtons = menuContainer.transform.Find("MainButtons").gameObject;
        var lilGuyObj = Object.Instantiate(ModAssets.LilGuyPrefab, mainButtons.transform);
        var lilGuyRect = lilGuyObj.GetComponent<RectTransform>();
        lilGuyRect.localPosition = new Vector3(207.6f, 106.5756f, 0);
        lilGuyRect.anchoredPosition = new Vector2(207.6f, -130.3f);
        lilGuyRect.sizeDelta = new Vector2(130, -130.3f);
    }
}