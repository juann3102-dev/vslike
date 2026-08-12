using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletRewardUI : MonoBehaviour
{
    private const int RemovalItemsPerPage = 4;

    private GameObject canvasObject;
    private RectTransform panel;
    private Text titleText;
    private Font font;
    private readonly List<GameObject> generatedButtons = new List<GameObject>();
    private IReadOnlyList<BulletRewardChoice> rewardChoices;
    private IReadOnlyList<OwnedBulletChoice> ownedBulletChoices;
    private Action<BulletRewardChoice> onResolved;
    private int clearedStage;
    private int removalPage;
    private bool resolved;

    public void Show(
        int clearedStage,
        IReadOnlyList<BulletRewardChoice> choices,
        IReadOnlyList<OwnedBulletChoice> ownedBullets,
        Action<BulletRewardChoice> callback)
    {
        EnsureUi();
        resolved = false;
        this.clearedStage = clearedStage;
        rewardChoices = choices;
        ownedBulletChoices = ownedBullets;
        onResolved = callback;
        ShowRewardChoices();
        canvasObject.SetActive(true);
    }

    private void ShowRewardChoices()
    {
        ClearButtons();
        titleText.text = $"STAGE {clearedStage} CLEAR\n보상을 선택하세요";

        int count = rewardChoices != null ? rewardChoices.Count : 0;
        for (int i = 0; i < count; i++)
        {
            BulletRewardChoice choice = rewardChoices[i];
            float y = 95f - i * 90f;
            CreateRewardButton(choice, y);
        }

        CreateSkipButton(-190f);
    }

    private void OpenRemovalChoices()
    {
        removalPage = 0;
        ShowRemovalChoices();
    }

    private void ShowRemovalChoices()
    {
        ClearButtons();
        int count = ownedBulletChoices != null ? ownedBulletChoices.Count : 0;
        titleText.text = count > 0
            ? "제거할 총알을 선택하세요\n선택한 총알 한 장이 제거됩니다"
            : "제거할 수 있는 총알이 없습니다";

        int pageCount = Mathf.Max(1, Mathf.CeilToInt(count / (float)RemovalItemsPerPage));
        removalPage = Mathf.Clamp(removalPage, 0, pageCount - 1);
        int startIndex = removalPage * RemovalItemsPerPage;
        int endIndex = Mathf.Min(startIndex + RemovalItemsPerPage, count);

        for (int i = startIndex; i < endIndex; i++)
        {
            OwnedBulletChoice ownedBullet = ownedBulletChoices[i];
            float y = 140f - (i - startIndex) * 80f;
            CreateRemovalTargetButton(ownedBullet, y);
        }

        if (removalPage > 0)
        {
            CreatePageButton("◀ 이전", -190f, -145f, removalPage - 1);
        }

        if (removalPage < pageCount - 1)
        {
            CreatePageButton("다음 ▶", -190f, 145f, removalPage + 1);
        }

        CreateBackButton(-280f);
    }

    public void Hide()
    {
        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
    }

    private void Resolve(BulletRewardChoice selectedChoice)
    {
        if (resolved)
        {
            return;
        }

        resolved = true;
        Hide();
        Action<BulletRewardChoice> callback = onResolved;
        onResolved = null;
        callback?.Invoke(selectedChoice);
    }

    private void EnsureUi()
    {
        if (canvasObject != null)
        {
            return;
        }

        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        canvasObject = new GameObject(
            "Bullet Reward Canvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject panelObject = CreateUiObject("Reward Panel", canvasObject.transform);
        panel = panelObject.AddComponent<Image>().rectTransform;
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(720f, 720f);
        panel.anchoredPosition = Vector2.zero;
        panelObject.GetComponent<Image>().color = new Color(0.06f, 0.07f, 0.1f, 0.96f);

        titleText = CreateText("Title", panel, 34, TextAnchor.MiddleCenter);
        RectTransform titleRect = titleText.rectTransform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(650f, 110f);
        titleRect.anchoredPosition = new Vector2(0f, -65f);

        canvasObject.SetActive(false);
    }

    private void CreateRewardButton(BulletRewardChoice choice, float y)
    {
        if (choice.Action == BulletRewardAction.RemoveBullet)
        {
            GameObject removalObject = CreateButtonObject(
                "총알 1개 제거\n현재 덱에서 선택",
                y,
                new Color(0.65f, 0.3f, 0.08f, 1f));
            removalObject.GetComponent<Button>().onClick.AddListener(OpenRemovalChoices);
            generatedButtons.Add(removalObject);
            return;
        }

        BulletDataSC.BulletInfo bullet = choice.Bullet;
        GameObject buttonObject = CreateButtonObject(
            $"{bullet.BulletName}\n{bullet.Rarity}  |  Damage {bullet.BulletDamage:0.##}",
            y,
            GetRarityColor(bullet.Rarity));
        buttonObject.GetComponent<Button>().onClick.AddListener(() => Resolve(choice));
        generatedButtons.Add(buttonObject);
    }

    private void CreateRemovalTargetButton(OwnedBulletChoice ownedBullet, float y)
    {
        BulletDataSC.BulletInfo bullet = ownedBullet.Bullet;
        GameObject buttonObject = CreateButtonObject(
            $"{bullet.BulletName}  × {ownedBullet.Count}\n{bullet.Rarity}  |  Damage {bullet.BulletDamage:0.##}",
            y,
            GetRarityColor(bullet.Rarity));
        buttonObject.GetComponent<Button>().onClick.AddListener(
            () => Resolve(new BulletRewardChoice(
                BulletRewardAction.RemoveBullet,
                bullet)));
        generatedButtons.Add(buttonObject);
    }

    private void CreateSkipButton(float y)
    {
        GameObject buttonObject = CreateButtonObject(
            "선택하지 않기",
            y,
            new Color(0.25f, 0.25f, 0.28f, 1f));
        buttonObject.GetComponent<Button>().onClick.AddListener(() => Resolve(null));
        generatedButtons.Add(buttonObject);
    }

    private void CreateBackButton(float y)
    {
        GameObject buttonObject = CreateButtonObject(
            "뒤로",
            y,
            new Color(0.25f, 0.25f, 0.28f, 1f));
        buttonObject.GetComponent<Button>().onClick.AddListener(ShowRewardChoices);
        generatedButtons.Add(buttonObject);
    }

    private void CreatePageButton(string label, float y, float x, int page)
    {
        GameObject buttonObject = CreateButtonObject(
            label,
            y,
            new Color(0.2f, 0.28f, 0.4f, 1f),
            x,
            270f);
        buttonObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            removalPage = page;
            ShowRemovalChoices();
        });
        generatedButtons.Add(buttonObject);
    }

    private GameObject CreateButtonObject(
        string label,
        float y,
        Color color,
        float x = 0f,
        float width = 570f)
    {
        GameObject buttonObject = CreateUiObject(label, panel);
        Image image = buttonObject.AddComponent<Image>();
        image.color = color;
        Button button = buttonObject.AddComponent<Button>();

        RectTransform rect = image.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, 72f);
        rect.anchoredPosition = new Vector2(x, y);

        Text text = CreateText("Label", rect, 25, TextAnchor.MiddleCenter);
        text.color = Color.white;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = new Vector2(12f, 4f);
        text.rectTransform.offsetMax = new Vector2(-12f, -4f);
        text.text = label;
        return buttonObject;
    }

    private Text CreateText(
        string objectName,
        Transform parent,
        int fontSize,
        TextAnchor alignment)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject result = new GameObject(objectName, typeof(RectTransform));
        result.transform.SetParent(parent, false);
        return result;
    }

    private void ClearButtons()
    {
        foreach (GameObject button in generatedButtons)
        {
            if (button != null)
            {
                button.SetActive(false);
                Destroy(button);
            }
        }

        generatedButtons.Clear();
    }

    private static Color GetRarityColor(BulletRarity rarity)
    {
        return rarity switch
        {
            BulletRarity.Rare => new Color(0.1f, 0.35f, 0.75f, 1f),
            BulletRarity.Epic => new Color(0.5f, 0.18f, 0.7f, 1f),
            _ => new Color(0.3f, 0.32f, 0.36f, 1f)
        };
    }
}
